using Application.Common.Search;
using DTO.Enums.Notification;
using DTO.Notification.Search;

namespace Application.Features.Notifications.Search;

public interface INotificationFullSearchCriteria : IFullSearchCriteria<NotificationFullSearchSortField>
{
    public NotificationStatus? Status { get;  }
}
