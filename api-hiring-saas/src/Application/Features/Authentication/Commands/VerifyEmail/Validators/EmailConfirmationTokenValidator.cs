using Application.Common.Interfaces.Identity;
using Application.Common.Localization.Extensions;
using Domain.Entities.User;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;

namespace Application.Features.Authentication.Commands.VerifyEmail.Validators;

public sealed record EmailConfirmationTokenValidatorData(string Token, Guid Uid);
public sealed class EmailConfirmationTokenValidator: AbstractValidator<EmailConfirmationTokenValidatorData>
{
    public EmailConfirmationTokenValidator (
        UserManager<ApplicationUser> userManager,
        IApplicationUserManager applicationUserManager)
    {
        RuleFor(data => data)
           .MustAsync(
               async (data, _) =>
               {
                   var user = await applicationUserManager.GetByUidAsync(data.Uid);

                   if (user != null)
                   {
                       var tokenDecodedBytes = WebEncoders.Base64UrlDecode(data.Token);
                       var tokenDecoded = Encoding.UTF8.GetString(tokenDecodedBytes);

                       return user.EmailVerificationToken == tokenDecoded;
                   }
                   return false;
               })
            .WithLocalizationKey("emailConfirmationValidator.message");
    }
}
