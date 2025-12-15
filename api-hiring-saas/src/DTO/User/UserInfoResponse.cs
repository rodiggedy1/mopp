using DTO.User.CalendlyDetails;

namespace DTO.User;

public record UserInfoResponse : UserResponse
{
    public string Type { get; set; } = null!;
    public CalendlyDetailsResponse CalendlyDetails { get; init; } = new();
}
