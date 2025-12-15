namespace Domain.Entities.Languages;

public interface ILanguageUpsertData
{
    string Name { get; }
    string Code { get; }
    string CultureCode { get; }
    bool IsDefault { get; }
}
