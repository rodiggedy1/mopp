using Application.Common.Interfaces;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using DTO.Enums.Notification;
using Application.Common.MessageBroker;
using DTO.MessageBroker.Messages.Notification;

namespace Application.Features.Notifications.Commands;

public sealed record NotificationMarkAllAsReadCommand() : ICommand;

public sealed class NotificationMarkAllAsReadCommandHandler : ICommandHandler<NotificationMarkAllAsReadCommand>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMessagePublisher _messagePublisher;

    public NotificationMarkAllAsReadCommandHandler(
        IApplicationDbContext dbContext,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        IMessagePublisher messagePublisher)
    {
        _dbContext = dbContext;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _messagePublisher = messagePublisher;
    }
    public async Task Handle(NotificationMarkAllAsReadCommand command, CancellationToken cancellationToken)
    {
        var notifications = await _dbContext.Notification.Where(x => 
        x.UserId == _currentUserService.UserId
        && x.Status == NotificationStatus.Unread).ToListAsync();

        notifications.ForEach(notification => notification.UpdateStatus(NotificationStatus.Read, false));

        _dbContext.Notification.UpdateRange(notifications);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var notificationIds = notifications.Select(x => x.Id).ToList();
        await _messagePublisher.PublishAsync(new NotificationsMarkAllAsReadForUserMessage(notificationIds));
    }
}
