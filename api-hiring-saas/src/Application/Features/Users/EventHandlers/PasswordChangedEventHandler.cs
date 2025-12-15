using Application.Common.MessageBroker;
using Domain.Events.Users;
using DTO.MessageBroker.Messages.Users;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Users.EventHandlers;

public sealed class PasswordChangedEventHandler : INotificationHandler<PasswordChangedEvent>
{
    private readonly IMessagePublisher _messagePublisher;
    private readonly ILogger<UserCreatedEventHandler> _logger;

    public PasswordChangedEventHandler(
        IMessagePublisher messagePublisher,
        ILogger<UserCreatedEventHandler> logger)
    {
        _messagePublisher = messagePublisher;
        _logger = logger;
    }

    public async Task Handle(PasswordChangedEvent eventData, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Password changed for user: {user}", eventData.User);

        // TODO: Store password change in history table

        await _messagePublisher.PublishAsync(new PasswordChangedMessage(
            eventData.User.FirstName,
            eventData.User.LastName,
            eventData.User.Email!));
    }
}
