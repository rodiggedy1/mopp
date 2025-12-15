namespace DTO.Attributes;

// Here because it shold be accessed from the DTO project
public class LocalizationKeyAttribute : Attribute
{
    public string LocalizationKey { get; private set; }
    public LocalizationKeyAttribute(string localizationKey)
    {
        LocalizationKey = localizationKey;
    }
}
