using System.Collections.Generic;

namespace NCoreUtils.Sass;

public class SassOptions(
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
    /// <summary>
    /// Include paths.
    /// </summary>
    public IReadOnlyList<string> IncludePaths { get; } = includePaths ?? new string[0];

    /// <summary>
    /// String to be used for indentation.
    /// </summary>
    public string? Indent { get; } = indent;

    /// <summary>
    /// Whether to treat source as sass (as opposed to scss).
    /// </summary>
    public bool IndentedSyntax { get; } = indentedSyntax;

    public string? InputPath { get; } = inputPath;

    /// <summary>
    /// String to be used to for line feeds.
    /// </summary>
    public string? LineFeed { get; } = lineFeed;

    /// <summary>
    /// Disable sourceMappingUrl in css output.
    /// </summary>
    public bool? OmitSourceMapUrl { get; } = omitSourceMapUrl;

    public string? OutputPath { get; } = outputPath;

    /// <summary>
    /// Output style for the generated css code.
    /// </summary>
    public SassOutputStyle OutputStyle { get; } = outputStyle;

    /// <summary>
    /// Precision for outputting fractional numbers.
    /// </summary>
    public int? Precision { get; } = precision;

    /// <summary>
    /// Whether to inline source comments.
    /// </summary>
    public bool? SourceComments { get; } = sourceComments;

    /// <summary>
    /// Whether to embed include contents in source maps.
    /// </summary>
    public bool? SourceMapContents { get; } = sourceMapContents;

    /// <summary>
    /// Whether to embed sourceMappingUrl as data uri.
    /// </summary>
    public bool? SourceMapEmbed { get; } = sourceMapEmbed;

    /// <summary>
    /// Path to source map file.
    /// <para>
    /// Enables the source map generating. Used to create sourceMappingUrl.
    /// </para>
    /// </summary>
    public string? SourceMapFile { get; } = sourceMapFile;

    public bool? SourceMapFileUrls { get; } = sourceMapFileUrls;

    /// <summary>
    /// Pass-through as sourceRoot property.
    /// </summary>
    public string? SourceMapRoot { get; } = sourceMapRoot;
}