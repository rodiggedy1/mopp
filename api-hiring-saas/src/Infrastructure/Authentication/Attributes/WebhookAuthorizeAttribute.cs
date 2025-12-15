using Application.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace Infrastructure.Authentication.Attributes;

public class WebhookAuthorizeAttribute : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var config = context.HttpContext.RequestServices.GetService<IOptions<IdentityConfig>>()?.Value;

        if (config == null)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        var token = context.HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

        if (string.IsNullOrEmpty(token) || !IsValidToken(token, config))
        {
            context.Result = new UnauthorizedResult();
        }
    }

    private bool IsValidToken(string token, IdentityConfig config)
    {
        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.Secret));

        var handler = new JwtSecurityTokenHandler();
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = authSigningKey,
            ValidateIssuer = true,
            ValidIssuer = config.ValidIssuer,
            ValidateAudience = true,
            ValidAudience = config.ValidAudience,
            ValidateLifetime = false,
        };

        try
        {
            var claimsPrincipal = handler.ValidateToken(token, validationParameters, out var validatedToken);

            return true;
        }
        catch
        {
            return false;
        }
    }
}
