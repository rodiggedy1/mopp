using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using Application.Common.MessageBroker;
using DTO.MessageBroker.Messages.Authenticate;
using FluentValidation;

namespace Application.Features.Users.Commands;

public sealed record class ForgotPasswordCommand(string Email) : ICommand;

public sealed class ForgotPasswordCommandHandler : ICommandHandler<ForgotPasswordCommand>
{
    private readonly IMessagePublisher _messagePublisher;

    public ForgotPasswordCommandHandler(IMessagePublisher messagePublisher)
    {
        _messagePublisher = messagePublisher;
    }
    public async Task Handle(ForgotPasswordCommand command, CancellationToken cancellationToken)
    {
        await _messagePublisher.PublishAsync(new PasswordResetTokenRequestMessage(command.Email), cancellationToken);
    }

    public sealed class ForgotPasswordCommandValidator : AbstractValidator<ForgotPasswordCommand>
    {
        public ForgotPasswordCommandValidator()
        {
            RuleFor(ctx => ctx.Email)
                .NotEmpty()
                .EmailAddress();
        }
    }
}
