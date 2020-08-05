using System;
using System.IO;
using System.Text;

namespace NCoreUtils.Sass.Debug
{
    class Program
    {
        private static void UseTempFolder(Action<string> action)
        {
            var folder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(folder);
            try
            {
                action(folder);
            }
            finally
            {
                Directory.Delete(folder, true);
            }
        }

        static void Main(string[] args)
        {
            UseTempFolder(folder =>
            {
                var input = "@import './import';\n.a { .b { margin: 0; } }";
                var imported = ".a { .c { margin: 1px; } }";
                var mainScss = Path.Combine(folder, "main.scss");
                var importScss = Path.Combine(folder, "_import.scss");
                File.WriteAllText(mainScss, input, new UTF8Encoding(false));
                File.WriteAllText(importScss, imported, new UTF8Encoding(false));
                var compiler = new SassCompiler();
                var res = compiler.CompileFile(mainScss, new SassOptions(
                    outputStyle: SassOutputStyle.Compressed,
                    indent: string.Empty,
                    sourceMapEmbed: false
                ));
                Console.WriteLine(res.Css);
            });
        }
    }
}
