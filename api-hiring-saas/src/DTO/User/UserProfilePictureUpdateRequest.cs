using Microsoft.AspNetCore.Http;

namespace DTO.User;

public record UserProfilePictureUpdateRequest
{
    public IFormFile Picture { get; init; } = null!;
}
