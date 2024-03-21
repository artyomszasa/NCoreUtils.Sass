using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using NCoreUtils.Sass.CommandLine;

namespace NCoreUtils.Sass;

internal static partial class Program
{
    private sealed class OptionInfoByLangNameEqualityComparer : IEqualityComparer<OptionInfo>
    {
        public bool Equals(OptionInfo? x, OptionInfo? y)
            => ReferenceEquals(x, y) || (x is not null && y is not null && StringComparer.InvariantCulture.Equals(x.LongName, y.LongName));

        public int GetHashCode(OptionInfo obj)
            => obj is null ? default : StringComparer.InvariantCulture.GetHashCode(obj.LongName);
    }

    private static OptionInfo OutputStyleOption { get; } = new OptionInfo("output-style", "Output style.");

    private static OptionInfo SourceOption { get; } = new OptionInfo("source", "s", "Source to read input from.");

    private static OptionInfo TargetOption { get; } = new OptionInfo("target", "t", "Target to write output to.");

    private static OptionInfo IncludePathsOption { get; } = new OptionInfo("include", "i", "Include paths.");

    private static OptionInfo SourceMapOption { get; } = new OptionInfo("source-maps", "Whether to generate source map.");

    private static IReadOnlyList<OptionInfo> Options { get; } =
    [
        OutputStyleOption,
        SourceOption,
        TargetOption,
        IncludePathsOption,
        SourceMapOption
    ];

    private static void WriteHelp()
    {
        Console.WriteLine("Usage: [BINARY] [OPTIONS]");
        Console.WriteLine("Options:");
        foreach (var option in Options)
        {
            Console.Write($"\t--{option.LongName}");
            if (option.ShortName is string shortName)
            {
                Console.Write($", -{shortName}");
            }
            Console.WriteLine();
            if (option.Description is string description)
            {
                Console.WriteLine($"\t\t{description}");
            }
        }
        Console.WriteLine("\t--help, -h");
        Console.WriteLine("\t\tShow this message.");
    }

    private static Options ParseOptions(string[] args)
    {
        var opts = new Dictionary<OptionInfo, OptionValue>(new OptionInfoByLangNameEqualityComparer());
        {
            (bool IsShort, string Name)? key = default;
            foreach (var arg in args)
            {
                if (key.HasValue)
                {
                    AddOption(opts, arg, key.Value.Name, key.Value.IsShort, arg);
                    key = null;
                }
                else if (arg.StartsWith("--"))
                {
                    var indexOfEq = arg.IndexOf('=');
                    if (indexOfEq != -1)
                    {
                        var name = arg[2 .. indexOfEq];
                        var value = arg[(indexOfEq + 1) ..];
                        AddOption(opts, arg, name, false, value);
                    }
                    else
                    {
                        key = (false, arg[2 ..]);
                    }
                }
                else if (arg.StartsWith('-'))
                {
                    key = (true, arg[1 ..]);
                }
                else
                {
                    throw new InvalidOperationException($"Invalid argument: {arg}.");
                }
            }
            if (key.HasValue)
            {
                AddOption(opts, (key.Value.IsShort ? "-" : "--") + key.Value.Name, key.Value.Name, key.Value.IsShort, string.Empty);
                key = null;
            }
        }
        return new Options(
            OutputStyle: Supply(GetLastOrDefault(opts, OutputStyleOption), "compressed"),
            Source: GetLastOrDefault(opts, SourceOption) ?? throw new InvalidOperationException("No source specified."),
            Target: GetLastOrDefault(opts, TargetOption) ?? throw new InvalidOperationException("No target specified."),
            IncludePaths: opts.TryGetValue(IncludePathsOption, out var v) ? v.Value : [],
            SourceMaps: opts.ContainsKey(SourceMapOption)
        );

        static string? GetLastOrDefault(Dictionary<OptionInfo, OptionValue> opts, OptionInfo option)
        {
            return opts.TryGetValue(option, out var value)
                ? value.Value[^1]
                : default;
        }

        static string Supply(string? source, string placeholder)
            => string.IsNullOrEmpty(source) ? placeholder : source;

        static void AddOption(Dictionary<OptionInfo, OptionValue> opts, string arg, string name, bool isShort, string value)
        {
            OptionInfo? matchedOption = default;
            foreach (var option in Options)
            {
                if (isShort ? option.ShortName == name : option.LongName == name)
                {
                    matchedOption = option;
                    break;
                }
            }
            if (matchedOption is null)
            {
                throw new InvalidOperationException($"Invalid argument: {arg}.");
            }
            ref var optionValue = ref CollectionsMarshal.GetValueRefOrAddDefault(opts, matchedOption, out var exists);
            if (!exists)
            {
                optionValue = new OptionValue(matchedOption, [value]);
            }
            else
            {
                optionValue = new OptionValue(matchedOption, [ .. optionValue.Value, value ]);
            }
        }
    }
}