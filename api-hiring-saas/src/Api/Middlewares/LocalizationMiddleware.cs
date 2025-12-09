using Application.Common.Localization;

namespace Api.Middlewares;

public class LocalizationMiddleware
{
    private readonly RequestDelegate _next;
    public LocalizationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext httpContext)
    {
        var localizationService = httpContext.RequestServices.GetRequiredService<ILocalizationService>();
        localizationService.SetCulture(httpContext.Request.Headers["Language"]);

        await _next(httpContext);
    }
}
