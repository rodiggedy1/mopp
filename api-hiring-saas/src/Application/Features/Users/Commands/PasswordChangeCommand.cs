using Application.Common.Interfaces.Request.Handlers;
using Application.Common.Interfaces;
using Application.Features.Users.Validators;
using Domain.Entities.User;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Application.Common.Interfaces.Request;

namespace Application.Features.Users.Commands;

public sealed record PasswordChangeCommand(string OldPassword, string NewPassword) : ICommand;

public sealed class ChangePasswordCommandHandler : ICommandHandler<PasswordChangeCommand>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IApplicationDbContext _applicationDbContext;

    public ChangePasswordCommandHandler(
        ICurrentUserService currentUserService,
        UserManager<ApplicationUser> userManager,
        IUnitOfWork unitOfWork,
        IHttpContextAccessor httpContextAccessor,
        IApplicationDbContext applicationDbContext)
    {
        _currentUserService = currentUserService;
        _userManager = userManager;
        _unitOfWork = unitOfWork;
        _httpContextAccessor = httpContextAccessor;
        _applicationDbContext = applicationDbContext;
    }
    public async Task Handle(PasswordChangeCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(_currentUserService.UserId!.ToString()!);

        if (user == null)
            throw new UnauthorizedAccessException();

        if (user.ActualPassword != request.OldPassword)
            throw new UnauthorizedAccessException();

        user.SetActualPassword(request.NewPassword);

        _applicationDbContext.User.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public sealed class PasswordChangeCommandValidator : AbstractValidator<PasswordChangeCommand>
    {
        public PasswordChangeCommandValidator(
            ICurrentUserService currentUserService,
            CurrentPasswordValidator currentPasswordValidator,
            NewPasswordValidator newPasswordValidator)
        {
        }
    }
}
