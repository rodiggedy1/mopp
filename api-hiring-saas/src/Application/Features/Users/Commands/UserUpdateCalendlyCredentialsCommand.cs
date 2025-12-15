using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Identity;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using Application.Common.Localization;
using Domain.Entities.User;
using Domain.Entities.Users;
using Microsoft.AspNetCore.Identity;

namespace Application.Features.Users.Commands;

public sealed record UserUpdateCalendlyCredentialsCommand(
    string? CalendlyClientId,
    string? CalendlyClientSecret,
    string? CalendlyAccessToken,
    string? CalendlyCode,
    string? CalendlyRedirectUri,
    string? CalendlyRefreshToken,
    string? CalendlyTokenExpiresAt) : IUserCalendlyCredentialsData, ICommand;


public sealed class UserUpdateCalendlyCredentialsCommandHandler : ICommandHandler<UserUpdateCalendlyCredentialsCommand>
{
    private readonly IApplicationUserManager _applicationUserManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILocalizationService _localizationService;

    public UserUpdateCalendlyCredentialsCommandHandler(
        IApplicationUserManager applicationUserManager,
        UserManager<ApplicationUser> userManager,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        ILocalizationService localizationService)
    {
        _applicationUserManager = applicationUserManager;
        _userManager = userManager;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _localizationService = localizationService;
    }

    public async Task Handle(UserUpdateCalendlyCredentialsCommand command, CancellationToken cancellationToken)
    {
        var user = await _applicationUserManager.GetAsync((int)_currentUserService.UserId!);

        if (user == null)
            throw new NotFoundException(_localizationService.GetValue("user.notFound.error.message"));

        user.UpdateCalendlyDetails(command);

        await _userManager.UpdateAsync(user);
        await _unitOfWork.SaveChangesAsync();
    }
}