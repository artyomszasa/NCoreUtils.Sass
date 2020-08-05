using System;

namespace NCoreUtils.Sass.Internal
{
    public class InteropSassFileContext : InteropSassContext
    {
        public InteropSassFileContext(IntPtr handle, bool ownsHandle) : base(handle, ownsHandle) { }

        protected override IntPtr CreateCompilerInternal()
            => Interop.sass_make_file_compiler(handle);

        protected override IntPtr GetInnerContextInternal()
            => Interop.sass_file_context_get_context(handle);

        protected override IntPtr GetOptionsInternal()
            => Interop.sass_file_context_get_options(handle);

        protected override bool ReleaseHandle()
        {
            Interop.sass_delete_file_context(handle);
            return base.ReleaseHandle();
        }

        protected override void SetOptionsInternal(IntPtr optionsHandle)
            => Interop.sass_file_context_set_options(handle, optionsHandle);

        public override int Compile()
            => Interop.sass_compile_file_context(handle);
    }
}