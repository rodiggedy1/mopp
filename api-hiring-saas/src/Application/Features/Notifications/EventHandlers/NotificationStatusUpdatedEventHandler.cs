using Application.Common.Caching;
using Application.Features.Notifications.Commands;
using Domain.Events.Notifications;
using MediatR;

namespace Application.Features.Notifications.EventHandlers;

public sealed class NotificationStatusUpdatedEventHandler : INotificationHandler<NotificationStatusUpdatedEvent>
{
    private readonly ICacheService _cacheService;
    private readonly IMediator _mediatr;

    public NotificationStatusUpdatedEventHandler(
        ICacheService cacheService,
        IMediator mediatr)
    {
        _cacheService = cacheService;
        _mediatr = mediatr;
    }

    public async Task Handle(NotificationStatusUpdatedEvent notification, CancellationToken cancellationToken)
    {
        await _mediatr.Send(new NotificationIndexCommand(notification.Notification.Id));
    }
}
