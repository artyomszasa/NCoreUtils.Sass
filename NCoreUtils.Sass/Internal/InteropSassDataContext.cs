using System;
using System.Buffers;

namespace NCoreUtils.Sass.Internal
{
    public class InteropSassDataContext : InteropSassContext
    {
        internal InteropSassDataContext(IntPtr handle, bool ownsHandle)
            : base(handle, ownsHandle)
        { }

        protected override IntPtr CreateCompilerInternal()
            => Interop.sass_make_data_compiler(handle);

        protected override IntPtr GetInnerContextInternal()
            => Interop.sass_data_context_get_context(handle);

        protected override IntPtr GetOptionsInternal()
            => Interop.sass_data_context_get_options(handle);

        protected override bool ReleaseHandle()
        {
            Interop.sass_delete_data_context(handle);
            return base.ReleaseHandle();
        }

        protected override void SetOptionsInternal(IntPtr optionsHandle)
            => Interop.sass_data_context_set_options(handle, optionsHandle);

        public override int Compile()
            => Interop.sass_compile_data_context(handle);
    }
}