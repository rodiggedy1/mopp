namespace DTO.User;

public sealed record UpdateCustomerRequest
{
    public string FirstName { get; init; } = null!;
    public string LastName { get; init; } = null!;
    public string? PhoneNumber { get; init; }
}
