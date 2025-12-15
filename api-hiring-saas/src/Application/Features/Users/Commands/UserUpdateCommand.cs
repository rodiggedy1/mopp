using Application.Common.Exceptions;
using Application.Common.Interfaces.Identity;
using Application.Common.Interfaces.Request.Handlers;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces;
using Application.Features.Users.Validators;
using AutoMapper;
using Domain.Entities.User;
using Domain.Entities.Users;
using DTO.User;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Application.Common.Localization;

namespace Application.Features.Users.Commands;

public sealed record UserUpdateCommand(
    string FirstName,
    string LastName,
    string Email,
    string? PhoneNumber
    ) : IUserUpdateData, ICommand<UserResponse>;


public sealed class UserUpdateCommandHandler : ICommandHandler<UserUpdateCommand, UserResponse>
{
    private readonly IApplicationUserManager _applicationUserManager;
    private readonly IMapper _mapper;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILocalizationService _localizationService;

    public UserUpdateCommandHandler(
        IApplicationUserManager applicationUserManager,
        IMapper mapper,
        UserManager<ApplicationUser> userManager,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        ILocalizationService localizationService)
    {
        _applicationUserManager = applicationUserManager;
        _mapper = mapper;
        _userManager = userManager;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _localizationService = localizationService;
    }

    public async Task<UserResponse> Handle(UserUpdateCommand command, CancellationToken cancellationToken)
    {
        var user = await _applicationUserManager.GetAsync((int)_currentUserService.UserId!);

        if (user == null)
            throw new NotFoundException(_localizationService.GetValue("user.notFound.error.message"));

        user.Update(command);

        await _userManager.UpdateAsync(user);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<UserResponse>(user);
    }
}

public sealed class UserUpdateCommandValidator : AbstractValidator<UserUpdateCommand>
{
    public UserUpdateCommandValidator(
        UserEmailUniqueValidator emailUniqueValidator,
        ICurrentUserService currentUserService)
    {
        RuleFor(cmd => cmd.LastName)
            .NotEmpty()
            .MaximumLength(30);

        RuleFor(cmd => cmd.PhoneNumber)
            .MaximumLength(15);

        RuleFor(cmd => cmd.Email)
            .NotEmpty()
            .EmailAddress()
            .DependentRules(
                () =>
                {
                    RuleFor(cmd => new UserEmailUniqueValidatorData(cmd.Email, currentUserService.UserId))
                        .SetValidator(emailUniqueValidator)
                        .OverridePropertyName(nameof(UserUpdateCommand.Email));
                });
    }
}
