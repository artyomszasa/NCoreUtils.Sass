using System.Runtime.CompilerServices;
using System.Text;

namespace NCoreUtils.Sass;

internal static partial class Program
{
    private static readonly UTF8Encoding _utf8 = new(false);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool Eqi(string a, string b)
        => StringComparer.InvariantCultureIgnoreCase.Equals(a, b);

    private static SassOutputStyle ParseOutputStyle(string source)
    {
        if (Eqi(nameof(SassOutputStyle.Compressed), source))
        {
            return SassOutputStyle.Compressed;
        }
        if (Eqi(nameof(SassOutputStyle.Nested), source))
        {
            return SassOutputStyle.Nested;
        }
        if (Eqi(nameof(SassOutputStyle.Compact), source))
        {
            return SassOutputStyle.Compact;
        }
        if (Eqi(nameof(SassOutputStyle.Expanded), source))
        {
            return SassOutputStyle.Expanded;
        }
        return SassOutputStyle.Compressed; // default
    }

    private static int Run(Options options)
    {
        try
        {
            var sassCompiler = new SassCompiler();
            var res = sassCompiler.CompileFile(options.Source, new SassOptions(
                includePaths: options.IncludePaths,
                inputPath: options.Source,
                outputStyle: ParseOutputStyle(options.OutputStyle ?? "compressed"),
                outputPath: options.Target,
                sourceComments: false,
                sourceMapContents: options.SourceMaps,
                sourceMapEmbed: options.SourceMaps
            ));
            if (!string.IsNullOrEmpty(res.Error))
            {
                Console.Error.WriteLine(res.Error);
                return -1;
            }
            File.WriteAllText(options.Target, res.Css, _utf8);
            return 0;
        }
        catch (Exception exn)
        {
            Console.Error.WriteLine(exn);
            return -1;
        }
    }

    private static int Main(string[] args)
    {
        if (args.Contains("--help") || args.Contains("-h"))
        {
            WriteHelp();
            return -1;
        }
        try
        {
            var options = ParseOptions(args);
            return Run(options);
        }
        catch (Exception exn)
        {
            Console.Error.WriteLine(exn);
            return -1;
        }
    }
}