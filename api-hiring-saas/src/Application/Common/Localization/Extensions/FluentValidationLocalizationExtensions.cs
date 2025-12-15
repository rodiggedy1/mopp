using Application.Common.Helpers;
using FluentValidation;

namespace Application.Common.Localization.Extensions;

public static class FluentValidationLocalizationExtensions
{
    public static IRuleBuilderOptions<T, TProperty> WithLocalizationKey<T, TProperty>(
        this IRuleBuilderOptions<T, TProperty> rule,
        string localizationKey,
        Func<T, object[]>? argsProvider = null)
    {
        return rule.WithMessage((instance, propertyValue) =>
        {
            var args = argsProvider?.Invoke(instance) ?? Array.Empty<object>();

            var localiztionService = ServiceResolver.GetRequiredService<ILocalizationService>();
            return localiztionService.GetValue(localizationKey, args);
        });
    }
}
