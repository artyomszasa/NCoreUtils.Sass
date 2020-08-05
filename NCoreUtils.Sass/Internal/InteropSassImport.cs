using System;
using System.Runtime.InteropServices;

namespace NCoreUtils.Sass.Internal
{
    [StructLayout(LayoutKind.Sequential, Pack=0)]
    public struct InteropSassImport
    {
        public IntPtr ImportPath;

        public IntPtr AbsolutePath;

        public IntPtr Source;

        public IntPtr SourceMap;

        public IntPtr Error;

        public int Line;

        public int Column;
    }
}