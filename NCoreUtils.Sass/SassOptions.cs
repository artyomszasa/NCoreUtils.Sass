using System.Collections.Generic;

namespace NCoreUtils.Sass
{
    public class SassOptions
    {
        /// <summary>
        /// Include paths.
        /// </summary>
        public IReadOnlyList<string> IncludePaths { get; }

        /// <summary>
        /// String to be used for indentation.
        /// </summary>
        public string? Indent { get; }

        /// <summary>
        /// Whether to treat source as sass (as opposed to scss).
        /// </summary>
        public bool IndentedSyntax { get; }

        public string? InputPath { get; }

        /// <summary>
        /// String to be used to for line feeds.
        /// </summary>
        public string? LineFeed { get; }

        /// <summary>
        /// Disable sourceMappingUrl in css output.
        /// </summary>
        public bool? OmitSourceMapUrl { get; }

        public string? OutputPath { get; }

        /// <summary>
        /// Output style for the generated css code.
        /// </summary>
        public SassOutputStyle OutputStyle { get; }

        /// <summary>
        /// Precision for outputting fractional numbers.
        /// </summary>
        public int? Precision { get; }

        /// <summary>
        /// Whether to inline source comments.
        /// </summary>
        public bool? SourceComments { get; }

        /// <summary>
        /// Whether to embed include contents in source maps.
        /// </summary>
        public bool? SourceMapContents { get; }

        /// <summary>
        /// Whether to embed sourceMappingUrl as data uri.
        /// </summary>
        public bool? SourceMapEmbed { get; }

        /// <summary>
        /// Path to source map file.
        /// <para>
        /// Enables the source map generating. Used to create sourceMappingUrl.
        /// </para>
        /// </summary>
        public string? SourceMapFile { get; }

        public bool? SourceMapFileUrls { get; }

        /// <summary>
        /// Pass-through as sourceRoot property.
        /// </summary>
        public string? SourceMapRoot { get; }

        public SassOptions(
            IReadOnlyList<string>? includePaths = default,
            string? indent = default,
            bool indentedSyntax = false,
            string? inputPath = default,
            string? lineFeed = default,
            bool? omitSourceMapUrl = default,
            string? outputPath = default,
            SassOutputStyle outputStyle = SassOutputStyle.Compact,
            int? precision = default,
            bool? sourceComments = default,
            bool? sourceMapContents = default,
            bool? sourceMapEmbed = default,
            string? sourceMapFile = default,
            bool? sourceMapFileUrls = default,
            string? sourceMapRoot = default)
        {
            IncludePaths = includePaths ?? new string[0];
            Indent = indent;
            IndentedSyntax = indentedSyntax;
            InputPath = inputPath;
            LineFeed = lineFeed;
            OmitSourceMapUrl = omitSourceMapUrl;
            OutputPath = outputPath;
            OutputStyle = outputStyle;
            Precision = precision;
            SourceComments = sourceComments;
            SourceMapContents = sourceMapContents;
            SourceMapEmbed = sourceMapEmbed;
            SourceMapFile = sourceMapFile;
            SourceMapFileUrls = sourceMapFileUrls;
            SourceMapRoot = sourceMapRoot;
        }
    }
}