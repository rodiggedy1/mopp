namespace DTO.User;

public sealed record MeResponse : UserInfoResponse
{
    public string? ProfilePicture { get; set; }
}
