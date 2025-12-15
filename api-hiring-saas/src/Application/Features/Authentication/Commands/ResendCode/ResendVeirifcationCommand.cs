using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using Application.Common.MessageBroker;
using DTO.MessageBroker.Messages.Authenticate;
using FluentValidation;

namespace Application.Features.Authentication.Commands.ResendCode;

public sealed record ResendVeirifcationCommand(string Email) : ICommand;

public sealed class ResendVeirifcationCommandHandler : ICommandHandler<ResendVeirifcationCommand>
{
    private readonly IMessagePublisher _messagePublisher;
    public ResendVeirifcationCommandHandler(IMessagePublisher messagePublisher)
    {
        _messagePublisher = messagePublisher;
    }

    public async Task Handle(ResendVeirifcationCommand command, CancellationToken cancellationToken)
    {
        await _messagePublisher.PublishAsync(new GenerateNewVerificationCodeMessage(command.Email));
    }
}

public sealed class ResendVeirifcationCommandValidator : AbstractValidator<ResendVeirifcationCommand>
{
    public ResendVeirifcationCommandValidator()
    {
        RuleFor(cmd => cmd.Email)
            .NotEmpty()
            .EmailAddress();
    }
}
