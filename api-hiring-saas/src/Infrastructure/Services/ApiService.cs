using Application.Common.Services;
using DTO.Enums.Media;
using Infrastructure.ApiClient.Clients;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

public class ApiService : IApiService
{
    private readonly IMediaApi _mediaApi;
    private readonly ILogger<ApiService> _logger;

    public ApiService(
        IMediaApi mediaApi,
        ILogger<ApiService> logger)
    {
        _mediaApi = mediaApi;
        _logger = logger;
    }
    public async Task<Stream?> DownloadFile(MediaEntityType mediaEntityType, int entityId, Guid mediaItemId)
    {
        try
        {
            return await _mediaApi.Download(mediaEntityType, entityId, mediaItemId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while executing IMediaApi.Download with: EntityType {0}, EntityId {1}, MediaItemId {2}", mediaEntityType, entityId, mediaItemId);
            return null;
        }
    }
}
