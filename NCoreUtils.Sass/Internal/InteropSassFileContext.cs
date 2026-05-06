using System;

namespace NCoreUtils.Sass.Internal;

public class InteropSassFileContext(IntPtr handle, bool ownsHandle)
    : InteropSassContext(handle, ownsHandle)
{
    protected override IntPtr CreateCompilerInternal()
        => Interop.sass_make_file_compiler(handle);

    // protected override IntPtr GetInnerContextInternal()
    //     => Interop.sass_file_context_get_context(handle);

    protected override IntPtr GetOptionsInternal()
        => Interop.sass_file_context_get_options(this);

    protected override bool ReleaseHandle()
    {
        Interop.sass_delete_file_context(this);
        return base.ReleaseHandle();
    }

    protected override void SetOptionsInternal(IntPtr optionsHandle)
        => Interop.sass_file_context_set_options(this, optionsHandle);

    public override int Compile()
        => Interop.sass_compile_file_context(this);
}