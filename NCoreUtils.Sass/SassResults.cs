namespace NCoreUtils.Sass;

public readonly struct SassResults(string? error, string css, string? sourceMap)
{
    public string? Error { get; } = error;

    public string Css { get; } = css;

    public string? SourceMap { get; } = sourceMap;

    public void Deconstruct(out string? error, out string css, out string? sourceMap)
    {
        error = Error;
        css = Css;
        sourceMap = SourceMap;
    }
}