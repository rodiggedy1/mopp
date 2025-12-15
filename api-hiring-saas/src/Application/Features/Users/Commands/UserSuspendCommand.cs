using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using Application.Common.Localization;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Users.Commands;

public sealed record UserSuspendCommand(
    int UserId,
    string SuspensionReason) : ICommand;

public sealed class UserSuspendCommandHandler : ICommandHandler<UserSuspendCommand>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILocalizationService _localizationService;

    public UserSuspendCommandHandler(
        IApplicationDbContext dbContext,
        IUnitOfWork unitOfWork,
        ILocalizationService localizationService)
    {
        _dbContext = dbContext;
        _unitOfWork = unitOfWork;
        _localizationService = localizationService;
    }

    public async Task Handle(UserSuspendCommand command, CancellationToken cancellationToken)
    {
        var user = await _dbContext.User.FirstOrDefaultAsync(u => u.Id == command.UserId, cancellationToken);

        if (user == null)
            throw new NotFoundException(_localizationService.GetValue("user.notFound.error.message"));

        user.Suspend(command.SuspensionReason);

        _dbContext.User.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}

public sealed class UserSuspendCommandValidator : AbstractValidator<UserSuspendCommand>
{
    public UserSuspendCommandValidator()
    {
        RuleFor(cmd => cmd.UserId)
            .NotEmpty();

        RuleFor(cmd => cmd.SuspensionReason)
            .NotEmpty()
            .MaximumLength(100);
    }
}
