using Application.Common.Localization;
using Domain.Entities.Languages;
using Infrastructure.Localization.Extensions;
using System.Globalization;

namespace Infrastructure.Localization;

public class LocalizationService : ILocalizationService
{
    private readonly ILocalizationManager _localizationManager;

    public LocalizationService(ILocalizationManager localizationManager)
    {
        _localizationManager = localizationManager;
    }

    public Language CurrentLanguage { get; private set; } = null!;
    public CultureInfo CurrentCulture => CultureInfo.GetCultureInfo(CurrentLanguage.CultureCode);
    public List<Language> AvailableLanguages => _localizationManager.AvailableLanguages;

    public string GetValue(string key, object[]? parameters = null)
    {
        try
        {
            return _localizationManager.GetValue(key, CurrentCulture, parameters) ?? key;
        }
        catch (Exception)
        {
            return key;
        }
    }

    public string GetValue(Enum enumValue)
    {
        string localizationKey = enumValue.GetLocalizationKey();

        if (CurrentLanguage == null)
        {
            return enumValue.ToString();
        }

        try
        {
            return _localizationManager.GetValue(localizationKey, CurrentCulture);
        }
        catch (Exception)
        {

            return enumValue.ToString();
        }
    }

    public void SetCulture(string? languageCode)
    {
        if (CurrentLanguage?.Code == languageCode)
            return;

        CurrentLanguage = 
            AvailableLanguages.FirstOrDefault(l => l.Code == languageCode) ?? 
            AvailableLanguages.FirstOrDefault(l => l.IsDefault) ?? 
            AvailableLanguages.First();

        _localizationManager.SetCurrentLanguage(CurrentLanguage);
    }
}
