using Domain.Entities.Base;
using DTO.Enums;

namespace Domain.Entities.Languages;

public class Language : BaseAuditableEntity
{
    public string Name { get; private set; } = null!;
    public string Code { get; private set; } = null!;
    public string CultureCode { get; private set; } = null!;
    public Status Status { get; private set; }
    public bool IsDefault { get; private set; }

    private Language()
    {

    }

    private Language(string name, string code, string cultureCode)
    {
        Name = name;
        Code = code;
        CultureCode = cultureCode;
        Status = Status.Active;
    }

    public static Language Create(ILanguageUpsertData data)
    {
        Language language = new(data.Name, data.Code, data.CultureCode);
        language.SetIsDefault(data.IsDefault);
        return language;
    }

    public void Update(ILanguageUpsertData data)
    {
        Name = data.Name;
        Code = data.Code;
        CultureCode = data.CultureCode;

        if (IsDefault != data.IsDefault)
        {
            SetIsDefault(data.IsDefault);
        }
    }

    public void SetIsDefault(bool isDefault)
    {
        IsDefault = isDefault;
        // Add Domain event and handle it to set the other (if any) language set as default false
    }
}
