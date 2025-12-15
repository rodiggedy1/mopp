using Application.Common.Localization.Extensions;
using DTO.Enums.User;
using FluentValidation;

namespace Application.Features.Users.Validators;

public sealed record AllowedUserStatusValidatorData(UserStatus Status);

public sealed class AllowedUserStatusValidator : AbstractValidator<AllowedUserStatusValidatorData>
{
    private static readonly UserStatus[] AllowedStatuses = new[]
    {
        UserStatus.Active,
        UserStatus.Deactivated
    };

    public AllowedUserStatusValidator()
    {
        RuleFor(data => data)
            .Must(data => AllowedStatuses.Contains(data.Status))
            .WithLocalizationKey("userStatusAllowed.message", data => new object[] { data.Status, string.Join(", ", AllowedStatuses) });
    }
}
