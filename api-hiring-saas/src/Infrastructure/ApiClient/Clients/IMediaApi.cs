using DTO.Enums.Media;
using Refit;

namespace Infrastructure.ApiClient.Clients
{
    public interface IMediaApi : IApiClient
    {
        [Get("/api/v1/media/{mediaEntityType}/{entityId}/{mediaItemId}/download/worker")]
        Task<Stream> Download(MediaEntityType mediaEntityType, int entityId, Guid mediaItemId);
    }
}
