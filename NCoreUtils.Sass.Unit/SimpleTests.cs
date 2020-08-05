using System;
using System.IO;
using System.Text;
using Xunit;

namespace NCoreUtils.Sass.Unit
{
    public class SimpleTests
    {
        private void UseTempFolder(Action<string> action)
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

        [Fact]
        public void SimpleString()
        {
            var input = ".a { .b { margin: 0; } .c { margin: 1px; } }";
            var expected = ".a .b{margin:0}.a .c{margin:1px}\n";
            var compiler = new SassCompiler();
            var res = compiler.CompileCode(input, new SassOptions(
                outputStyle: SassOutputStyle.Compressed,
                indent: string.Empty,
                sourceMapEmbed: false
            ));
            Assert.Equal(expected, res.Css);
        }

        [Fact]
        public void SimpleFile() => UseTempFolder(folder =>
        {
            var input = ".a { .b { margin: 0; } .c { margin: 1px; } }";
            var expected = ".a .b{margin:0}.a .c{margin:1px}\n";
            var mainScss = Path.Combine(folder, "main.scss");
            File.WriteAllText(mainScss, input, new UTF8Encoding(false));
            var compiler = new SassCompiler();
            var res = compiler.CompileFile(mainScss, new SassOptions(
                outputStyle: SassOutputStyle.Compressed,
                indent: string.Empty,
                sourceMapEmbed: false
            ));
            Assert.Equal(expected, res.Css);
        });

        [Fact]
        public void ImportsFile() => UseTempFolder(folder =>
        {
            var input = "@import './import';\n.a { .b { margin: 0; } }";
            var imported = ".a { .c { margin: 1px; } }";
            var expected = ".a .c{margin:1px}.a .b{margin:0}\n";
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
            Assert.Equal(expected, res.Css);
        });
    }
}