namespace Infrastructure.Localization;

using System.Collections.Generic;
using System.Text.Json;

public class JsonFileReader : FileReader
{
    public JsonFileReader(string path)
        : base(path)
    {
    }

    #region Public Methods

    internal override async Task<Dictionary<string, string>> GetEntries()
    {
        return JsonSerializer.Deserialize<Dictionary<string, string>>(await File.ReadAllTextAsync(Path)) ?? new();
    }

    #endregion
}
