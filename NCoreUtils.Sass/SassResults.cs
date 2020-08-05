namespace NCoreUtils.Sass
{
    public struct SassResults
    {
        public string? Error { get; }

        public string Css { get; }

        public string? SourceMap { get; }

        public SassResults(string? error, string css, string? sourceMap)
        {
            Error = error;
            Css = css;
            SourceMap = sourceMap;
        }

        public void Deconstruct(out string? error, out string css, out string? sourceMap)
        {
            error = Error;
            css = Css;
            sourceMap = SourceMap;
        }
    }
}