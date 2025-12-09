using Application.Common.MessageBroker;
using Domain.Entities.User;
using Domain.Events.Users;
using DTO.MessageBroker.Messages.Search;
using DTO.MessageBroker.Messages.Users;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using System.Text;

namespace Application.Features.Users.EventHandlers;

public sealed class UserCreatedEventHandler : INotificationHandler<UserCreatedEvent>
{
    private readonly IMessagePublisher _messagePublisher;
    private readonly ILogger<UserCreatedEventHandler> _logger;
    private readonly UserManager<ApplicationUser> _userManager;

    public UserCreatedEventHandler(
        IMessagePublisher messagePublisher,
        ILogger<UserCreatedEventHandler> logger,
        UserManager<ApplicationUser> userManager)
    {
        _messagePublisher = messagePublisher;
        _logger = logger;
        _userManager = userManager;
    }

    public async Task Handle(UserCreatedEvent eventData, CancellationToken cancellationToken)
    {
        _logger.LogInformation("User created {user}", eventData.User);

        //Skip sending email for now

        //var token = await _userManager.GenerateEmailConfirmationTokenAsync(eventData.User);

        //eventData.User!.SetEmailVerificationToken(token);
        //await _userManager.UpdateAsync(eventData.User);

        //byte[] tokenBytes = Encoding.UTF8.GetBytes(token);
        //var tokenEncoded = WebEncoders.Base64UrlEncode(tokenBytes);

        //await _messagePublisher.PublishAsync(new UserCreatedMessage
        //{
        //    FirstName = eventData.User.FirstName,
        //    LastName = eventData.User.LastName,
        //    Email = eventData.User.Email!,
        //    EmailVerificationCode = tokenEncoded,
        //    Uid = eventData.User.Uid
        //});

        await _messagePublisher.PublishAsync(new IndexUserMessage(eventData.User.Id));
    }
}
