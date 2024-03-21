using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace NCoreUtils.Sass;

public class FileInfoProvider(string path)
{
    private static readonly Regex _delimeter = new("[\\/]", RegexOptions.Compiled);

    public IReadOnlyList<string> Segments { get; } = _delimeter.Split(path);

    public override string ToString()
    {
        return string.Join(Path.DirectorySeparatorChar.ToString(), Segments);
    }
}