using Application.Common.Search;
using DTO.Notification;

namespace Application.Features.Notifications.Search;

public sealed record NotificationSearchable : NotificationResponse, ISearchable
{
}
