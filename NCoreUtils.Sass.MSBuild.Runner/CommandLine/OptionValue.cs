using System.Diagnostics.CodeAnalysis;

namespace NCoreUtils.Sass.CommandLine;

public readonly struct OptionValue(OptionInfo option, IReadOnlyList<string> value)
{
    public OptionInfo Option { get; } = option;

    public IReadOnlyList<string> Value { get; } = value;

    [MemberNotNullWhen(true, nameof(Value))]
    public bool IsPresent => Value is not null;
}