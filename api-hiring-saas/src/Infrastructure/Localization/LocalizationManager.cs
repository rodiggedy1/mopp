using Application.Common.Interfaces;
using Application.Common.Localization;
using Domain.Entities.Languages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Globalization;

namespace Infrastructure.Localization;

public class LocalizationManager : ILocalizationManager
{
    #region Fields
    private readonly Dictionary<CultureInfo, Dictionary<string, string>> _languageEntries = new();
    #endregion

    public Language DefaultLanguage { get; private set; } = null!;
    public Language CurrentLanguage { get; private set; } = null!;
    public List<Language> AvailableLanguages { get; private set; } = new List<Language>();

    public Dictionary<CultureInfo, Dictionary<string, string>> LanguageEntries
    {
        get { return _languageEntries; }
    }

    public string? GetValue(string key, CultureInfo cultureInfo, object[]? parameters = null)
    {
        if (key == null || cultureInfo == null || _languageEntries == null)
            return key;

        if (_languageEntries.TryGetValue(cultureInfo, out var entries) &&  
            entries.TryGetValue(key, out var value) &&  
            value != null)
        {
            return parameters == null ? value : string.Format(value, parameters);
        }

        return key;
    }
    public string? GetValue(string key, object[]? parameters = null)
        => GetValue(key, CultureInfo.GetCultureInfo(CurrentLanguage.CultureCode), parameters);

    public async Task InitializeAsync(IApplicationDbContext dbContext, ILogger logger)
    {
        try
        {
            string localizationPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Localization", "Translations");
            var languages = await dbContext.Language.ToListAsync();

            foreach (var language in languages)
            {
                AvailableLanguages.Add(language);
                if (language.IsDefault)
                {
                    DefaultLanguage = language;
                }

                await AddCulture(CultureInfo.GetCultureInfo(language.CultureCode), 
                    new JsonFileReader(Path.Combine(localizationPath, $"{language.CultureCode}.json")));
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Errow while initializing localization");
        }
    }

    public void SetCurrentLanguage(Language language)
        => CurrentLanguage = language;

    private async Task AddCulture(CultureInfo culture, FileReader reader)
    {
        if (_languageEntries.ContainsKey(culture))
        {
            // If the culture is already registered, re-set its values
            _languageEntries[culture] = await reader.GetEntries();
        }
        else
        {
            _languageEntries.Add(culture, await reader.GetEntries());
        }
    }
}
