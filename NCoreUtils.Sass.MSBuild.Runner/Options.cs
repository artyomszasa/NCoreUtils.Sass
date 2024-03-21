namespace NCoreUtils.Sass;

public record Options(
    string? OutputStyle,
    string Source,
    string Target,
    IReadOnlyList<string> IncludePaths,
    bool SourceMaps
);