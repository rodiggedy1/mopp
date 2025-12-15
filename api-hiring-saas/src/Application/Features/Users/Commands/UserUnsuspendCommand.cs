using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using Application.Common.Localization;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Users.Commands;

public sealed record UserUnsuspendCommand(
    int UserId) : ICommand;

public sealed class UserUnsuspendCommandHandler : ICommandHandler<UserUnsuspendCommand>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILocalizationService _localizationService;

    public UserUnsuspendCommandHandler(
        IApplicationDbContext dbContext,
        IUnitOfWork unitOfWork,
        ILocalizationService localizationService)
    {
        _dbContext = dbContext;
        _unitOfWork = unitOfWork;
        _localizationService = localizationService;
    }
    public async Task Handle(UserUnsuspendCommand command, CancellationToken cancellationToken)
    {
        var user = await _dbContext.User.FirstOrDefaultAsync(u => u.Id == command.UserId, cancellationToken);

        if (user == null)
            throw new NotFoundException(_localizationService.GetValue("user.notFound.error.message"));

        user.RemoveSuspension();

        _dbContext.User.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}

public sealed class UserUnsuspendCommandValidator : AbstractValidator<UserUnsuspendCommand>
{
    public UserUnsuspendCommandValidator()
    {
        RuleFor(cmd => cmd.UserId)
            .NotEmpty();
    }
}
