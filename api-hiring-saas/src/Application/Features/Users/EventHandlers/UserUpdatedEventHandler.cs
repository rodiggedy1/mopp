using Application.Common.Caching;
using Application.Features.Users.Commands;
using Domain.Events.Users;
using MediatR;

namespace Application.Features.Users.EventHandlers;

public sealed class UserUpdatedEventHandler : INotificationHandler<UserUpdatedEvent>
{
    private readonly ICacheService _cacheService;
    private readonly IMediator _mediatr;

    public UserUpdatedEventHandler(
        ICacheService cacheService,
        IMediator mediatr)
    {
        _cacheService = cacheService;
        _mediatr = mediatr;
    }
    public async Task Handle(UserUpdatedEvent notification, CancellationToken cancellationToken)
    {
        await _cacheService.RemoveAsync($"{CacheKeys.User}-me-{notification.User.Id}", cancellationToken);
        await _cacheService.RemoveAsync($"{CacheKeys.User}-{notification.User.Id}", cancellationToken);
        await _mediatr.Send(new UserIndexCommand(notification.User.Id));
    }
}
