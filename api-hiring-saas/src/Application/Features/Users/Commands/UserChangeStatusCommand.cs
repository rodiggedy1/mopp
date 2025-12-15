using Application.Common.Caching;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using Application.Common.Localization;
using Application.Features.Users.Validators;
using DTO.Enums.User;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Users.Commands;

public sealed record UserChangeStatusCommand(
    UserStatus Status) : ICommand;

public sealed class UserChangeStatusCommandHandler : ICommandHandler<UserChangeStatusCommand>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly ICacheService _cacheService;
    private readonly ILocalizationService _localizationService;


    public UserChangeStatusCommandHandler(
        IApplicationDbContext dbContext,
        IUnitOfWork unitOfWork, 
        ICurrentUserService currentUserService,
        ICacheService cacheService,
        ILocalizationService localizationService)
    {
        _dbContext = dbContext;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _cacheService = cacheService;
        _localizationService = localizationService;
    }
    public async Task Handle(UserChangeStatusCommand command, CancellationToken cancellationToken)
    {
        var user = await _dbContext.User
            .FirstOrDefaultAsync(u => u.Id == _currentUserService.UserId, cancellationToken);

        if (user == null)
            throw new NotFoundException(_localizationService.GetValue("user.notFound.error.message"));

        switch (command.Status)
        {
            case UserStatus.Active:
                user.Activate();
                break;
            case UserStatus.Deactivated:
                user.Deactivate();
                break;
        }

        _dbContext.User.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}

public sealed class UserChangeStatusCommandValidator : AbstractValidator<UserChangeStatusCommand>
{
    public UserChangeStatusCommandValidator(AllowedUserStatusValidator allowedStatusValidator)
    {
        RuleFor(cmd => cmd.Status)
           .NotEmpty()
           .IsInEnum()
           .DependentRules(() =>
           {
               RuleFor(cmd => new AllowedUserStatusValidatorData(cmd.Status))
                   .SetValidator(allowedStatusValidator)
                   .OverridePropertyName(nameof(UserChangeStatusCommand.Status));
           });
    }
}
