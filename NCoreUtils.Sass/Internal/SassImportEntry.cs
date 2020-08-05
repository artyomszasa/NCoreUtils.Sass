using System;

namespace NCoreUtils.Sass.Internal
{
    public struct SassImportEntry
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

        public string ImportPath { get; }

        public string AbsolutePath { get; }

        public string Source { get; }

        public string SourceMap { get; }

        public string Error { get; }

        public int Line { get; }

        public int Column { get; }

        public SassImportEntry(
            string importPath,
            string absolutePath,
            string source,
            string sourceMap,
            string error,
            int line,
            int column)
        {
            ImportPath = importPath;
            AbsolutePath = absolutePath;
            Source = source;
            SourceMap = sourceMap;
            Error = error;
            Line = line;
            Column = column;
        }
    }
}