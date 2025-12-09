using System.Net;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Application.Common.Extensions;
using Application.Common.MediaStorage;
using Application.Common.MediaStorage.Interfaces;
using Domain.Entities.Medias;
using Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Infrastructure.MediaStorage;

public class CloudflareR2Storage : IMediaStorage
{
    private readonly CloudflareR2Config _config;
    private readonly IMediaStorageHelper _mediaStorageHelper;
    private readonly IMediaStorage _fallbackStorage;
    private readonly HttpClient _httpClient;
    private bool _useFallback = false;

    public CloudflareR2Storage(
        IOptions<MediaConfig> configOpt,
        IMediaStorageHelper mediaStorageHelper,
        IFileSystemProvider fileSystemProvider)
    {
        _config = configOpt.Value.CloudflareR2!;
        _mediaStorageHelper = mediaStorageHelper;

        var fallbackConfig = new MediaFileSystemConfig
        {
            HostUrl = "https://api.mopphiring.com",
            UploadMediaPath = "media",
            UploadDocumentPath = "documents"
        };
        
        _fallbackStorage = new MediaFileSystemStorage(
            fileSystemProvider,
            Microsoft.Extensions.Options.Options.Create(new MediaConfig { FileSystem = fallbackConfig }),
            mediaStorageHelper);

        Console.WriteLine($"CloudflareR2Storage: Bucket={_config.BucketName}");

        var handler = new SocketsHttpHandler
        {
            SslOptions = new SslClientAuthenticationOptions
            {
                EnabledSslProtocols = SslProtocols.Tls12,
                RemoteCertificateValidationCallback = (_, _, _, _) => true,
                CertificateRevocationCheckMode = X509RevocationMode.NoCheck
            }
        };

        _httpClient = new HttpClient(handler) { Timeout = TimeSpan.FromMinutes(2) };
    }

    public async Task<Stream> Download(MediaItemKey itemKey)
    {
        if (_useFallback)
        {
            return await _fallbackStorage.Download(itemKey);
        }

        try
        {
            var key = GetObjectKey(itemKey);
            var url = $"https://{_config.AccountId}.r2.cloudflarestorage.com/{_config.BucketName}/{key}";
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            
            AddSimpleAuth(request, key, HttpMethod.Get);
            
            var response = await _httpClient.SendAsync(request);
            
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStreamAsync();
            }
            else
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"CloudflareR2Storage: Download failed {response.StatusCode}: {errorBody}");
                throw new HttpRequestException($"Download failed: {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"CloudflareR2Storage: Download error, using fallback: {ex.Message}");
            _useFallback = true;
            return await _fallbackStorage.Download(itemKey);
        }
    }

    public async Task<string> Upload(MediaItemKey itemKey, IFormFile file)
    {
        if (_useFallback)
        {
            return await _fallbackStorage.Upload(itemKey, file);
        }

        try
        {
            var key = GetObjectKey(itemKey);
            var url = $"https://{_config.AccountId}.r2.cloudflarestorage.com/{_config.BucketName}/{key}";
            
            using var stream = file.OpenReadStream();
            var content = new StreamContent(stream);
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);
            
            var request = new HttpRequestMessage(HttpMethod.Put, url) { Content = content };
            AddSimpleAuth(request, key, HttpMethod.Put);
            
            var response = await _httpClient.SendAsync(request);
            
            if (response.IsSuccessStatusCode)
            {
                var publicUrl = GetPublicUrl(key);
                Console.WriteLine($"CloudflareR2Storage: Upload successful! {publicUrl}");
                return publicUrl;
            }
            else
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"CloudflareR2Storage: Upload failed {response.StatusCode}: {errorBody}");
                throw new HttpRequestException($"Upload failed: {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"CloudflareR2Storage: Upload error, using fallback: {ex.Message}");
            _useFallback = true;
            return await _fallbackStorage.Upload(itemKey, file);
        }
    }

    public async Task<string> Upload(MediaItemKey itemKey, Stream file, long length)
    {
        if (_useFallback)
        {
            return await _fallbackStorage.Upload(itemKey, file, length);
        }

        try
        {
            var key = GetObjectKey(itemKey);
            var url = $"https://{_config.AccountId}.r2.cloudflarestorage.com/{_config.BucketName}/{key}";
            
            var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            memoryStream.Position = 0;

            var content = new StreamContent(memoryStream);
            var request = new HttpRequestMessage(HttpMethod.Put, url) { Content = content };
            AddSimpleAuth(request, key, HttpMethod.Put);
            
            var response = await _httpClient.SendAsync(request);
            
            if (response.IsSuccessStatusCode)
            {
                var publicUrl = GetPublicUrl(key);
                Console.WriteLine($"CloudflareR2Storage: Stream upload successful! {publicUrl}");
                return publicUrl;
            }
            else
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"CloudflareR2Storage: Stream upload failed {response.StatusCode}: {errorBody}");
                throw new HttpRequestException($"Stream upload failed: {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"CloudflareR2Storage: Stream upload error, using fallback: {ex.Message}");
            _useFallback = true;
            return await _fallbackStorage.Upload(itemKey, file, length);
        }
    }

    public Task<MediaItemKey> ParseUrl(string url) => _mediaStorageHelper.ParseUrl(url).AsTask();

    public async Task Delete(MediaItemKey itemKey)
    {
        if (_useFallback)
        {
            await _fallbackStorage.Delete(itemKey);
            return;
        }

        try
        {
            var key = GetObjectKey(itemKey);
            var url = $"https://{_config.AccountId}.r2.cloudflarestorage.com/{_config.BucketName}/{key}";
            var request = new HttpRequestMessage(HttpMethod.Delete, url);
            
            AddSimpleAuth(request, key, HttpMethod.Delete);
            
            var response = await _httpClient.SendAsync(request);
            
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"CloudflareR2Storage: Deleted {key}");
            }
            else
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"CloudflareR2Storage: Delete failed {response.StatusCode}: {errorBody}");
                throw new HttpRequestException($"Delete failed: {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"CloudflareR2Storage: Delete error, using fallback: {ex.Message}");
            _useFallback = true;
            await _fallbackStorage.Delete(itemKey);
        }
    }

    private void AddSimpleAuth(HttpRequestMessage request, string key, HttpMethod method)
    {
        var utcNow = DateTime.UtcNow;
        var dateStamp = utcNow.ToString("yyyyMMdd");
        var timeStamp = utcNow.ToString("yyyyMMddTHHmmssZ");
        var host = _config.AccountId + ".r2.cloudflarestorage.com";
        var contentHash = "UNSIGNED-PAYLOAD";
        
        request.Headers.Clear();
        request.Headers.Host = host;
        request.Headers.Add("X-Amz-Date", timeStamp);
        request.Headers.Add("X-Amz-Content-Sha256", contentHash);
        
        var canonicalUri = "/" + _config.BucketName + "/" + key;
        var canonicalHeaders = "host:" + host + "\n" + "x-amz-content-sha256:" + contentHash + "\n" + "x-amz-date:" + timeStamp + "\n";
        var signedHeaders = "host;x-amz-content-sha256;x-amz-date";
        
        var canonicalRequest = method.Method + "\n" + canonicalUri + "\n\n" + canonicalHeaders + "\n" + signedHeaders + "\n" + contentHash;
        var credentialScope = dateStamp + "/" + _config.Region + "/s3/aws4_request";
        var stringToSign = "AWS4-HMAC-SHA256\n" + timeStamp + "\n" + credentialScope + "\n" + Sha256Hash(canonicalRequest);
        
        var signature = CalculateSignature(stringToSign, dateStamp);
        var authHeader = "AWS4-HMAC-SHA256 Credential=" + _config.AccessKeyId + "/" + credentialScope + ", SignedHeaders=" + signedHeaders + ", Signature=" + signature;
        
        request.Headers.TryAddWithoutValidation("Authorization", authHeader);
    }

    private string CalculateSignature(string stringToSign, string dateStamp)
    {
        var kDate = HmacSha256(Encoding.UTF8.GetBytes("AWS4" + _config.SecretAccessKey), dateStamp);
        var kRegion = HmacSha256(kDate, _config.Region);
        var kService = HmacSha256(kRegion, "s3");
        var kSigning = HmacSha256(kService, "aws4_request");
        
        return ToHexString(HmacSha256(kSigning, stringToSign));
    }

    private byte[] HmacSha256(byte[] key, string data)
    {
        using var hmac = new HMACSHA256(key);
        return hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
    }

    private string Sha256Hash(string data)
    {
        using var sha256 = SHA256.Create();
        var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(data));
        return ToHexString(hash);
    }

    private string ToHexString(byte[] bytes)
    {
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }

    private string GetObjectKey(MediaItemKey itemKey) =>
        $"{itemKey.EntityType}/{itemKey.EntityId}/{itemKey.FileName}";

    private string GetPublicUrl(string key) =>
        $"{_config.PublicUrl.TrimEnd('/')}/{key}";

    public void Dispose()
    {
        _httpClient?.Dispose();
    }
}
