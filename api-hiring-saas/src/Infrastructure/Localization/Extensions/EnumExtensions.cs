using DTO.Attributes;
using System.Reflection;

namespace Infrastructure.Localization.Extensions;

public static class EnumExtensions
{
    public static string GetLocalizationKey(this Enum enumValue)
    {
        FieldInfo fi = enumValue.GetType().GetField(enumValue.ToString())!;

        LocalizationKeyAttribute[] attributes =
            (LocalizationKeyAttribute[])fi.GetCustomAttributes(
            typeof(LocalizationKeyAttribute),
            false);

        if (attributes != null &&
            attributes.Length > 0)
            return attributes[0].LocalizationKey;
        else
            return enumValue.ToString();
    }
}
