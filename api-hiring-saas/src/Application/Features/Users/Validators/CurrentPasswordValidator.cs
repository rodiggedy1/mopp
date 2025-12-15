using Application.Common.Localization.Extensions;
using Domain.Entities.User;
using FluentValidation;
using Microsoft.AspNetCore.Identity;

namespace Application.Features.Users.Validators;

public sealed record CurrentPasswordValidatorData(string OldPassword, int UserId);
public sealed class CurrentPasswordValidator : AbstractValidator<CurrentPasswordValidatorData>
{
    public CurrentPasswordValidator(
        UserManager<ApplicationUser> userManager)
    {
        RuleFor(data => data)
            .MustAsync(
                async (data, _) =>
                {
                    var user = await userManager.FindByIdAsync(data.UserId.ToString());
                    return await userManager.CheckPasswordAsync(user!, data.OldPassword);
                })
            .WithErrorCode("Invalid credentials")
            .WithLocalizationKey("currentPasswordValidator.message");
    }
}
