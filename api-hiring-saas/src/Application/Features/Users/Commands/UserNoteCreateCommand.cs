using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Identity;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using Application.Common.Localization;
using Application.Common.MessageBroker;
using DTO.Enums.User;
using DTO.MessageBroker.Messages.Users;

namespace Application.Features.Users.Commands;

public sealed record UserNoteCreateCommand(
    UserNoteType NoteType,
    string Message) : ICommand;

public sealed class UserNoteCreateCommandHandler : ICommandHandler<UserNoteCreateCommand>
{
    private readonly IMessagePublisher _messagePublisher;
    private readonly IApplicationUserManager _applicationUserManager;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILocalizationService _localizationService;

    public UserNoteCreateCommandHandler(
        IMessagePublisher messagePublisher,
        IApplicationUserManager applicationUserManager,
        ICurrentUserService currentUserService,
        ILocalizationService localizationService)
    {
        _messagePublisher = messagePublisher;
        _applicationUserManager = applicationUserManager;
        _currentUserService = currentUserService;
        _localizationService = localizationService;
    }
    public async Task Handle(UserNoteCreateCommand command, CancellationToken cancellationToken)
    {
        var user = await _applicationUserManager.GetAsync((int)_currentUserService.UserId!);

        if (user == null)
            throw new NotFoundException(_localizationService.GetValue("user.notFound.error.message"));

        await _messagePublisher.PublishAsync(new NoteFromCustomerMessage(
            _localizationService.GetValue(command.NoteType),
            user.FirstName,
            user.Email!,
            command.Message));
    }
}