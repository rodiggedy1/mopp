using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Search.Configuration;

public sealed class SearchConfig
{
    public const string SectionName = "SearchConfig";

    [Required]
    public string IndexPrefix { get; set; } = null!;
    [Required]
    public string Url { get; set; } = null!;
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
    [Required]
    public int TimeoutInMilliSec { get; set; }
    [Required]
    public bool EnsureIndexesOnStart { get; set; }

    /// <summary>
    /// Chunk size of documents during indexing all documents
    /// </summary>
    [Required]
    public int IndexDocumentsChunkSize { get; set; }

    public TimeSpan Timeout => TimeSpan.FromMilliseconds(TimeoutInMilliSec);
}