# NCoreUtils.Sass
libsass wrapper for .NET Standard.

native libraries included for `win-x64`, `osx-x64`, `linux-x64` and `linux-arm64`.

## Usage

### Basic usage

Basic case is pertty straightforward:

From file:

```csharp
var res = new SassCompiler().CompileFile("/path/to/input.scss", new SassOptions(
    includePaths: new string[] { /* ... */ },
    inputPath: "/path/to/input.scss",
    outputPath: "/path/to/output.css",
    outputStyle: SassOutputStyle.Compressed
));
```

From sass/scss code:

```csharp
var res = new SassCompiler().CompileCode("...scss/sass code...", new SassOptions(
    includePaths: new string[] { /* ... */ },
    inputPath: "/path/to/input.scss",
    outputPath: "/path/to/output.css",
    outputStyle: SassOutputStyle.Compressed
));
```

In both cases `inputPath` and `outputPath` in options are only used internally (e.g. path resolving, output generation), neither is tells libsass to read nor to write files.
The `res` object has the following structure:

```csharp
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
```

All options reflect the underlying libsass options, refer to libsass documentation for further details.

## Advanced usage

In advanced scenario compilation is split into two steps: parsing and executing. It is useful to get dependencies without performing execution:

```csharp
// Initialize compilation (calls parse internally).
using var compilation = new SassCompiler().ProcessFile(path, new SassOptions(
    includePaths: new string[] { /* ... */ },
    inputPath: "/path/to/input.scss",
    outputPath: "/path/to/output.css",
    sourceMapContents: true,
    sourceMapEmbed: true,
    outputStyle: SassOutputStyle.Compressed
));
// Get all dependencies aka included files
foreach (var fpath in compilation.Context.GetIncludedFiles())
{
    Console.WriteLine(fpath);
}
// Optionally generate output (same result as in basic case).
var res = compilation.Execute();
```

**NOTE**: In contrast with `CompileXxxx` methods `ProcessXxxx` methods leave some libsass objects alive in order to perform further actions on the context/compiler. Consequently the returned object must be disposed to free those objects.

