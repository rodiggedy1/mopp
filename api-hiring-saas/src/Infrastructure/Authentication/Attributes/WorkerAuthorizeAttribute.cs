using Domain.Interfaces;
using Infrastructure.Identity.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace Infrastructure.Authentication.Attributes;

public class WorkerAuthorizeAttribute : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var config = context.HttpContext.RequestServices.GetService<IOptions<WorkerAuthConfig>>()?.Value;
        var dateTimeProvider = context.HttpContext.RequestServices.GetService<IDateTime>();

        if (config == null || dateTimeProvider == null)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        var token = context.HttpContext.Request.Headers["WorkerAuth"].ToString().Replace("Bearer ", "");

        if (string.IsNullOrEmpty(token) || !IsValidToken(token, config, dateTimeProvider))
        {
            context.Result = new UnauthorizedResult();
        }
    }

    private bool IsValidToken(string token, WorkerAuthConfig config, IDateTime dateTimeProvider)
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
            ValidateLifetime = true,
            LifetimeValidator = (notBefore, expires, token, param) =>
                expires.HasValue && expires.Value > dateTimeProvider.Now,
        };

        try
        {
            var claimsPrincipal = handler.ValidateToken(token, validationParameters, out var validatedToken);
            var apiKeyClaim = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == "ApiKey");

            return apiKeyClaim != null && apiKeyClaim.Value == config.ApiKey;
        }
        catch
        {
            return false;
        }
    }
}
