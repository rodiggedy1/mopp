namespace Application.Features.Notifications.Search;

public interface INotificationForUserFullSearchCriteria : INotificationFullSearchCriteria
{
    int UserId { get; }
}
