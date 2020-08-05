using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NCoreUtils.Sass.Internal
{
    public class InteropSassCompiler : SafeHandle
    {
        public InteropSassCompilerImportEntries Imports
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new InteropSassCompilerImportEntries(this);
        }

        public override bool IsInvalid => IntPtr.Zero == handle;

        public InteropSassCompiler(IntPtr handle, bool ownsHandle)
            : base(IntPtr.Zero, ownsHandle)
            => SetHandle(handle);

        protected override bool ReleaseHandle()
        {
            Interop.sass_delete_compiler(handle);
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Parse()
            => Interop.sass_compiler_parse(handle);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Execute()
            => Interop.sass_compiler_execute(handle);
    }
}