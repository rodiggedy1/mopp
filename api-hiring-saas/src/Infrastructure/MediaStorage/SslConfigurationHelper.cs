using System.Net;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

namespace Infrastructure.MediaStorage;

/// <summary>
/// Helper class for basic SSL/TLS settings - minimal global configuration
/// </summary>
public static class SslConfigurationHelper
{
    private static bool _isConfigured = false;
    private static readonly object _lockObject = new object();

    /// <summary>
    /// Initializes minimal global SSL/TLS settings for the application
    /// This method is thread-safe and will only configure settings once
    /// </summary>
    public static void ConfigureGlobalSslSettings()
    {
        if (_isConfigured) return;

        lock (_lockObject)
        {
            if (_isConfigured) return;

            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls13;
                
                ServicePointManager.DefaultConnectionLimit = 100;
                ServicePointManager.Expect100Continue = false;
                ServicePointManager.UseNagleAlgorithm = false;

                _isConfigured = true;
                Console.WriteLine("SSL Configuration: Basic global SSL/TLS settings configured");
                Console.WriteLine($"SSL Configuration: Current SecurityProtocol = {ServicePointManager.SecurityProtocol}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SSL Configuration: Failed to configure global SSL settings: {ex.Message}");
                throw;
            }
        }
    }
    
    /// <summary>
    /// Gets the current SSL configuration status
    /// </summary>
    public static bool IsConfigured => _isConfigured;
    
    /// <summary>
    /// Forces reconfiguration of SSL settings (use with caution)
    /// </summary>
    public static void ForceReconfigure()
    {
        lock (_lockObject)
        {
            _isConfigured = false;
        }
        ConfigureGlobalSslSettings();
    }

    /// <summary>
    /// Tests connectivity to Cloudflare R2 with TLS 1.2 only
    /// </summary>
    public static async Task<bool> TestCloudflareR2Connectivity(string accountId)
    {
        try
        {
            var endpoint = $"https://{accountId}.r2.cloudflarestorage.com";
            Console.WriteLine($"SSL Configuration: Testing R2 connectivity to {endpoint} with TLS 1.2 ONLY");

            using var client = new HttpClient(new SocketsHttpHandler
            {
                SslOptions = new SslClientAuthenticationOptions
                {
                    EnabledSslProtocols = SslProtocols.Tls12,
                    RemoteCertificateValidationCallback = (_, _, _, _) => true,
                    CertificateRevocationCheckMode = X509RevocationMode.NoCheck
                }
            });

            client.Timeout = TimeSpan.FromSeconds(30);
            
            var response = await client.GetAsync(endpoint);
            
            Console.WriteLine($"SSL Configuration: R2 connectivity test - Status: {response.StatusCode}, TLS handshake successful!");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"SSL Configuration: R2 connectivity test failed: {ex.Message}");
            return false;
        }
    }
}