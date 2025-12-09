using Application.Common.MediaStorage.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace Application.Common.MediaStorage;

public class CloudflareR2Config : IMediaConfigStorageProvider
{
    [Required]
    public string AccountId { get; set; } = null!;

    [Required]
    public string AccessKeyId { get; set; } = null!;

    [Required]
    public string SecretAccessKey { get; set; } = null!;

    [Required]
    public string BucketName { get; set; } = null!;

    [Required]
    public string PublicUrl { get; set; } = null!;

    public string Region { get; set; } = "auto";

    public MediaStorageProviderType StorageProviderType => MediaStorageProviderType.CloudflareR2;
}