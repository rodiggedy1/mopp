using DTO.Enums.Notification;
using DTO.Notification.Search;
using DTO.Pagination;
using DTO.Sorting;

namespace Application.Features.Notifications.Search;

public sealed record NotificationForUserFullSearchRequest : INotificationForUserFullSearchCriteria
{
    public int UserId { get; init; }
    public NotificationStatus? Status { get; init; }
    public PaginationOptions Paging { get; init; } = null!;
    public SortOptions<NotificationFullSearchSortField>? Sorting { get; init; }
    public string? Query { get; init; }
}
