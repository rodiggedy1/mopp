using Domain.Entities.User;
using FluentValidation;
using Microsoft.AspNetCore.Identity;

namespace Application.Features.Users.Validators;

public sealed record NewPasswordValidatorData(string NewPassword);
public sealed class NewPasswordValidator : AbstractValidator<NewPasswordValidatorData>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public NewPasswordValidator(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;

        RuleFor(data => data)
            .CustomAsync(ValidateNewPassword);
    }

    private async Task ValidateNewPassword(NewPasswordValidatorData data, ValidationContext<NewPasswordValidatorData> context, CancellationToken token)
    {
        var passwordValidator = new PasswordValidator<ApplicationUser>();

        var newPasswordValidationResult = await passwordValidator.ValidateAsync(
            _userManager,
            null,
            data.NewPassword);

        if (!newPasswordValidationResult.Succeeded)
        {
            foreach (var error in newPasswordValidationResult.Errors)
            {
                context.AddFailure(error.Code, error.Description);
            }
        }
    }
}
