using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace NCoreUtils.Sass
{
    public class FileInfoProvider
    {
        private static readonly Regex _delimeter = new Regex("[\\/]", RegexOptions.Compiled);

        public IReadOnlyList<string> Segments { get; }

        public FileInfoProvider(string path)
        {
            Segments = _delimeter.Split(path);
        }

        public override string ToString()
        {
            return string.Join(Path.DirectorySeparatorChar.ToString(), Segments);
        }
    }
}
