namespace NCoreUtils.Sass.CommandLine;

public record OptionInfo(
    string LongName,
    string? ShortName,
    string? Description
) {
    public OptionInfo(string LongName, string? Description)
        : this(LongName, default, Description)
    { }
}