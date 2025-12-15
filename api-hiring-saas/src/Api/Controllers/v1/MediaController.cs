using Application.Features.Enums.Queries;
using Application.Features.Medias.Queries;
using DTO.Enums.Media;
using DTO.Response;
using Infrastructure.Authentication.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

public class MediaController : ApiControllerBase
{

    /// <summary>
    /// Downloads a media file associated with the specified entity and media item.
    /// </summary>
    /// <remarks>This method retrieves the media file based on the specified entity type, entity ID, and media
    /// item ID. Ensure that the provided identifiers are valid and correspond to an existing media item.</remarks>
    /// <param name="mediaEntityType">The type of the media entity to which the media item belongs.</param>
    /// <param name="entityId">The unique identifier of the entity associated with the media item.</param>
    /// <param name="mediaItemId">The unique identifier of the media item to download.</param>
    /// <returns>A <see cref="FileStreamResult"/> containing the media file to be downloaded.</returns>
    [HttpGet("{mediaEntityType}/{entityId}/{mediaItemId}/download")]
    public async Task<FileStreamResult> Download(MediaEntityType mediaEntityType, int entityId, Guid mediaItemId)
    {
        var result = await Mediator.Send(new MediaDownloadQuery(mediaEntityType, entityId, mediaItemId));

        return File(result);
    }

    /// <summary>
    /// Retrieves a collection of media entity types as a list of key-value pairs.
    /// </summary>
    /// <remarks>This method returns a read-only collection of media entity types, where each entity type is
    /// represented  as a key-value pair. The key is the name of the entity type, and the value is its corresponding
    /// identifier.</remarks>
    /// <returns>A task that represents the asynchronous operation. The task result contains a read-only collection of  <see
    /// cref="ListItemBaseResponse"/> objects, each representing a media entity type.</returns>
    [HttpGet("entity-types")]
    public async Task<IReadOnlyCollection<ListItemBaseResponse>> GetMediaEntityTypes()
    {
        return await Mediator.Send(new GetEnumValuesQuery(typeof(MediaEntityType)));
    }

    /// <summary>
    /// Downloads a media item from the worker service.
    /// </summary>
    /// <remarks>This method retrieves the specified media item from the worker service based on the provided
    /// identifiers. Ensure that the caller has the necessary permissions to access the requested media item.</remarks>
    /// <param name="mediaEntityType">The type of the media entity to download, such as video or image.</param>
    /// <param name="entityId">The unique identifier of the entity associated with the media item.</param>
    /// <param name="mediaItemId">The unique identifier of the media item to download.</param>
    /// <returns>A <see cref="Stream"/> containing the content of the downloaded media item.</returns>
    [AllowAnonymous]
    [WorkerAuthorize]
    [HttpGet("{mediaEntityType}/{entityId}/{mediaItemId}/download/worker")]
    public async Task<Stream> DownloadFromWorker(MediaEntityType mediaEntityType, int entityId, Guid mediaItemId)
    {
        var result = await Mediator.Send(new MediaDownloadQuery(mediaEntityType, entityId, mediaItemId));
        return result.Content;
    }
}
