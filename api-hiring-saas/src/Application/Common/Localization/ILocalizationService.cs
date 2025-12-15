using Domain.Entities.Languages;
using System.Globalization;

namespace Application.Common.Localization;

public interface ILocalizationService
{
    Language CurrentLanguage { get; }
    CultureInfo CurrentCulture { get; }
    List<Language> AvailableLanguages { get; }
    void SetCulture(string? culture);
    string GetValue(string key, object[]? parameters = null);
    string GetValue(Enum enumValue);
}