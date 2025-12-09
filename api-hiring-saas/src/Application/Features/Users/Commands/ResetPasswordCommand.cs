using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Identity;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using Application.Features.Users.Validators;
using Domain.Entities.User;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Web;

namespace Application.Features.Users.Commands;

public sealed record ResetPasswordCommand(Guid Uid, string Token, string Password) : ICommand;

public sealed class ResetPasswordCommandHandler : ICommandHandler<ResetPasswordCommand>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IApplicationUserManager _applicationUserManager;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ResetPasswordCommandHandler(
        UserManager<ApplicationUser> userManager,
        IApplicationUserManager applicationUserManager,
        IUnitOfWork unitOfWork,
        IHttpContextAccessor httpContextAccessor)
    {
        _userManager = userManager;
        _applicationUserManager = applicationUserManager;
        _unitOfWork = unitOfWork;
        _httpContextAccessor = httpContextAccessor;
    }
    public async Task Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _applicationUserManager.GetByUidAsync(request.Uid);

        if (user == null)
            throw new NotFoundException("User not found");

        if (string.IsNullOrEmpty(user.PasswordResetToken))
            throw new ApplicationException("Invalid token");

        await _applicationUserManager.ValidatePassword(user, "_Virtual007!");

        string oldPassword = user.PasswordHash!;

        var tokenDecoded = HttpUtility.UrlDecode(request.Token);

        if (tokenDecoded != user.PasswordResetToken)
            throw new ApplicationException("Invalid token");

        var removePasswordResult = await _userManager.RemovePasswordAsync(user);

        if (!removePasswordResult.Succeeded)
            throw new ApplicationException("Password update failed");

        var addPasswordResult = await _userManager.AddPasswordAsync(user, "_Virtual007!");

        if (!addPasswordResult.Succeeded)
        {
            throw new ApplicationException("Password update failed");
        }

        // TODO: Store password change in database
        var ipAddress = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();

        user.UpdatePassword(oldPassword, ipAddress);
        user.SetActualPassword(request.Password);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}

public sealed class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
{
    public ResetPasswordCommandValidator(NewPasswordValidator newPasswordValidator)
    {
        RuleFor(cmd => cmd.Token)
            .NotEmpty();

        RuleFor(cmd => cmd.Uid)
            .NotEmpty()
            .Must(uid => uid != default);
    }
}
