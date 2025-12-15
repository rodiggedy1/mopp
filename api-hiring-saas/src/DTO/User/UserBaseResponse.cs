namespace DTO.User;

public record UserBaseResponse
{
    public int Id { get; init; }
    public string FirstName { get; init; } = null!;
    public string LastName { get; init; } = null!;
    public string? Picture { get; set; }
    public string? CalendlyProfileUrl { get; init; }
    public string? ExternalCalendarUrl { get; init; }
}
