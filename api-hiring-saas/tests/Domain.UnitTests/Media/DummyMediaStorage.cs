using Domain.Entities.Medias;
using Domain.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Domain.UnitTests.Media;

internal class DummyMediaStorage : IMediaStorage
{
    public Task Delete(MediaItemKey itemKey)
    {
        throw new NotImplementedException();
    }

    public Task<Stream> Download(MediaItemKey itemKey)
    {
        throw new NotImplementedException();
    }

    public Task<MediaItemKey> ParseUrl(string url)
    {
        throw new NotImplementedException();
    }

    public Task<string> Upload(MediaItemKey itemKey, IFormFile file)
    {
        throw new NotImplementedException();
    }

    public Task<string> Upload(MediaItemKey itemKey, Stream file, long length)
    {
        throw new NotImplementedException();
    }
}
