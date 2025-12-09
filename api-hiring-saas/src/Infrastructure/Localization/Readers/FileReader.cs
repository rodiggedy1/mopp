namespace Infrastructure.Localization;

using System.Collections.Generic;

public abstract class FileReader
{
    #region Fields

    private string _path;

    #endregion

    protected FileReader(string path)
    {
        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"File '{path}' not found.", path);
        }

        Path = path;
    }

    protected string Path { get => _path; set => _path = value; }

    #region Abstract Methods

    internal abstract Task<Dictionary<string, string>> GetEntries();

    #endregion
}
