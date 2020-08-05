using NCoreUtils.Sass.Internal;

namespace NCoreUtils.Sass
{
    public class SassCompiler
    {
        private static void ApplyOptions(InteropSassContext context, SassOptions options)
        {
            var contextOptions = context.Options;
            foreach (var includePath in options.IncludePaths)
            {
                contextOptions.IncludePaths.Add(includePath);
            }
            if (!(options.Indent is null))
            {
                contextOptions.Indent = options.Indent;
            }
            contextOptions.IsIndentedSyntaxSrc = options.IndentedSyntax;
            if (!(options.InputPath is null))
            {
                contextOptions.InputPath = options.InputPath;
            }
            if (!(options.LineFeed is null))
            {
                contextOptions.LineFeed = options.LineFeed;
            }
            if (options.OmitSourceMapUrl.HasValue)
            {
                contextOptions.OmitSourceMapUrl = options.OmitSourceMapUrl.Value;
            }
            if (!(options.OutputPath is null))
            {
                contextOptions.OutputPath = options.OutputPath;
            }
            contextOptions.OutputStyle = options.OutputStyle;
            if (options.Precision.HasValue)
            {
                contextOptions.Precision = options.Precision.Value;
            }
            if (options.SourceComments.HasValue)
            {
                contextOptions.SourceComments = options.SourceComments.Value;
            }
            if (options.SourceMapContents.HasValue)
            {
                contextOptions.SourceMapContents = options.SourceMapContents.Value;
            }
            if (options.SourceMapEmbed.HasValue)
            {
                contextOptions.SourceMapEmbed = options.SourceMapEmbed.Value;
            }
            if (!(options.SourceMapFile is null))
            {
                contextOptions.SourceMapFile = options.SourceMapFile;
            }
            if (options.SourceMapFileUrls.HasValue)
            {
                contextOptions.SourceMapFileUrls = options.SourceMapFileUrls.Value;
            }
            if (!(options.SourceMapRoot is null))
            {
                contextOptions.SourceMapRoot = options.SourceMapRoot;
            }
        }

        public SassResults CompileFile(string path, SassOptions? options = default)
        {
            using var context = InteropSassContext.CreateFromFile(path);
            if (!(options is null))
            {
                ApplyOptions(context, options);
            }
            return context.GetResults(context.Compile());
        }

        public SassResults CompileCode(string sassCode, SassOptions? options = default)
        {
            using var context = InteropSassContext.CreateFromData(sassCode);
            if (!(options is null))
            {
                ApplyOptions(context, options);
            }
            var indent = context.Options.Indent;
            System.Console.WriteLine(indent);
            return context.GetResults(context.Compile());
        }

        public SassCompilation ProcessFile(string path, SassOptions? options = default)
        {
            var context = InteropSassContext.CreateFromFile(path);
            if (!(options is null))
            {
                ApplyOptions(context, options);
            }
            var compiler = context.CreateCompiler();
            compiler.Parse();
            return new SassCompilation(context, compiler);
        }

        public SassCompilation ProcessData(string sassCode, SassOptions? options = default)
        {
            var context = InteropSassContext.CreateFromData(sassCode);
            if (!(options is null))
            {
                ApplyOptions(context, options);
            }
            var compiler = context.CreateCompiler();
            compiler.Parse();
            return new SassCompilation(context, compiler);
        }
    }
}