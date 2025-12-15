using Application.Common.Localization;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Api.Filters;

public class CultureAwareOperationFilter : IOperationFilter
{
    private readonly ILocalizationService _localizationService;

    public CultureAwareOperationFilter(ILocalizationService localizationService)
    {
        _localizationService = localizationService;
    }
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {

        if (operation.Parameters == null)
        {
            operation.Parameters = new List<OpenApiParameter>();
        }

        var defaultLanguageCode = _localizationService.AvailableLanguages
            .Where(l => l.IsDefault)
            .Select(l => l.Code.ToLower())
            .First();

        var languageParameter = new OpenApiParameter
        {
            Name = "Language",
            In = ParameterLocation.Header,
            Description = "Language",
            Required = true,
            Schema = new OpenApiSchema
            {
                Type = "string",
                Enum = _localizationService.AvailableLanguages.Select(l => new OpenApiString(l.Code)).Cast<IOpenApiAny>().ToList()
            },
            Example = new OpenApiString(defaultLanguageCode)
        };

        operation.Parameters.Add(languageParameter);
    }
}
