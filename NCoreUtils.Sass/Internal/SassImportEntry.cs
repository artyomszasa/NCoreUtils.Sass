using System;

namespace NCoreUtils.Sass.Internal;

public readonly struct SassImportEntry(
    string importPath,
    string absolutePath,
    string source,
    string sourceMap,
    string error,
    int line,
    int column)
{
    internal unsafe static SassImportEntry Create(IntPtr ptr)
    {
        var p = (InteropSassImport*)ptr;
        return new SassImportEntry(
            importPath: Interop.ReadOptionalUtf8String(p->ImportPath)!,
            absolutePath: Interop.ReadOptionalUtf8String(p->AbsolutePath)!,
            source: Interop.ReadOptionalUtf8String(p->Source)!,
            sourceMap: Interop.ReadOptionalUtf8String(p->SourceMap)!,
            error: Interop.ReadOptionalUtf8String(p->Error)!,
            line: p->Line,
            column: p->Column
        );
    }

    public string ImportPath { get; } = importPath;

    public string AbsolutePath { get; } = absolutePath;

    public string Source { get; } = source;

    public string SourceMap { get; } = sourceMap;

    public string Error { get; } = error;

    public int Line { get; } = line;

    public int Column { get; } = column;
}