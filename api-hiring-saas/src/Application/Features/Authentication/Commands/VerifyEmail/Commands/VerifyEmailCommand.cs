using Application.Common.Interfaces;
using Application.Common.Interfaces.Identity;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using Application.Features.Authentication.Commands.VerifyEmail.Validators;
using FluentValidation;

namespace Application.Features.Authentication.Commands.VerifyEmail.Commands;

public sealed record VerifyEmailCommand(string Token, Guid Uid) : ICommand;

public sealed class VerifyEmailCommandHandler : ICommandHandler<VerifyEmailCommand>
{
    private readonly IApplicationUserManager _applicationUserManager;
    private readonly IUnitOfWork _unitOfWork;

    public VerifyEmailCommandHandler(
        IApplicationUserManager applicationUserManager,
        IUnitOfWork unitOfWork)
    {
        _applicationUserManager = applicationUserManager;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(VerifyEmailCommand request, CancellationToken cancellationToken)
    {
        var user = await _applicationUserManager.GetByUidAsync(request.Uid);
        user!.SetEmailConfirmed();
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}

public sealed class VerifyEmailCommandValidator : AbstractValidator<VerifyEmailCommand>
{
    public VerifyEmailCommandValidator(EmailConfirmationTokenValidator emailConfirmationTokenValidator)
    {
        RuleFor(cmd => cmd.Uid)
            .NotEmpty()
            .Must(uid => uid != default)
            .WithMessage(cmd => $"Invalid value {cmd.Uid} for Uid");

        RuleFor(cmd => cmd.Token)
            .NotEmpty()
            .DependentRules(
                () =>
                {
                    RuleFor(cmd => new EmailConfirmationTokenValidatorData(cmd.Token, cmd.Uid))
                        .SetValidator(emailConfirmationTokenValidator)
                        .OverridePropertyName(nameof(VerifyEmailCommand.Token));
                });
    }
}
