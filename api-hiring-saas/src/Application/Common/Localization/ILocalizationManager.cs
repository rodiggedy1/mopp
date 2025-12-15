using Application.Common.Interfaces;
using Domain.Entities.Languages;
using Microsoft.Extensions.Logging;
using System.Globalization;

namespace Application.Common.Localization
{
    public interface ILocalizationManager
    {
        Language DefaultLanguage { get; }
        Language CurrentLanguage { get; }
        List<Language> AvailableLanguages { get; }
        Dictionary<CultureInfo, Dictionary<string, string>> LanguageEntries { get; }
        string? GetValue(string key, CultureInfo cultureInfo, object[]? parameters = null);
        string? GetValue(string key, object[]? parameters = null);
        Task InitializeAsync(IApplicationDbContext dbContext, ILogger logger);
        void SetCurrentLanguage(Language language);
    }
}
