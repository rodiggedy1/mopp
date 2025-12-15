using Application.Common.Interfaces;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using DTO.Enums.Notification;
using DTO.Notification;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Notifications.Queries;

public sealed record NotificationGetUnreadCountQuery : IQuery<NotificationUnreadCountResponse>;

public sealed class NotificationGetUnreadCountQueryHandler : IQueryHandler<NotificationGetUnreadCountQuery, NotificationUnreadCountResponse>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IApplicationDbContext _dbContext;

    public NotificationGetUnreadCountQueryHandler(
        ICurrentUserService currentUserService,
        IApplicationDbContext dbContext)
    {
        _currentUserService = currentUserService;
        _dbContext = dbContext;
    }

    public async Task<NotificationUnreadCountResponse> Handle(NotificationGetUnreadCountQuery query, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;

        var count = await _dbContext.Notification
            .Where(n => n.UserId == userId && n.Status == NotificationStatus.Unread)
            .CountAsync();

        return new NotificationUnreadCountResponse() { Count = count };
    }
}
