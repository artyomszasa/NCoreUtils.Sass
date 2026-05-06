using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NCoreUtils.Sass.Internal;

public abstract class InteropSassContext : SafeHandle, IInteropSassOptionsMemory
{
    public static InteropSassFileContext CreateFromFile(string path)
    {
        var handle = Interop.sass_make_file_context(path);
        if (IntPtr.Zero == handle)
        {
            throw new InvalidOperationException($"Failed to create file context from \"{path}\".");
        }
        return new InteropSassFileContext(handle, true);
    }

    public unsafe static InteropSassDataContext CreateFromData(string sassCode)
    {
        ArgumentNullException.ThrowIfNull(sassCode);
        var sassCodeCopy = Interop.sass_copy_c_string(sassCode);
        var handle = Interop.sass_make_data_context(sassCodeCopy);
        if (IntPtr.Zero == handle)
        {
            throw new InvalidOperationException($"Failed to create file context from sass source.");
        }
        return new InteropSassDataContext(handle, true);
    }

    private PinnableUtf8String? _pinnableIndent;

    private PinnableUtf8String? _pinnableLineFeed;

    public override bool IsInvalid => IntPtr.Zero == handle;

    public int ErrorColumn
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Interop.sass_context_get_error_column(this);
    }

    public string? ErrorFile
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Interop.sass_context_get_error_file(this);
    }

    public string? ErrorJson
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Interop.sass_context_get_error_json(this);
    }

    public int ErrorLine
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Interop.sass_context_get_error_line(this);
    }

    public string? ErrorMessage
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Interop.sass_context_get_error_message(this);
    }

    public string? ErrorSrc
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Interop.sass_context_get_error_src(this);
    }

    public int ErrorStatus
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Interop.sass_context_get_error_status(this);
    }

    public string? ErrorText
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Interop.sass_context_get_error_text(this);
    }

    public string? OutputString
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Interop.sass_context_get_output_string(this);
    }

    public InteropSassOptions Options
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => new(GetOptionsInternal(), false, this);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => SetOptionsInternal(value.DangerousGetHandle());
    }

    public string? SourceMapString
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Interop.sass_context_get_source_map_string(this);
    }

    protected InteropSassContext(IntPtr handle, bool ownsHandle)
        : base(IntPtr.Zero, ownsHandle)
        => SetHandle(handle);

    internal SassResults GetResults(int compilationExitCode)
    {
        if (0 == compilationExitCode)
        {
            return new SassResults(default, OutputString!, SourceMapString);
        }
        return new SassResults(ErrorMessage, default!, default);
    }

    protected abstract IntPtr CreateCompilerInternal();

    protected abstract IntPtr GetOptionsInternal();

    protected override bool ReleaseHandle()
    {
        _pinnableIndent?.Dispose();
        _pinnableLineFeed?.Dispose();
        return true;
    }

    protected abstract void SetOptionsInternal(IntPtr optionsHandle);

    public InteropSassCompiler CreateCompiler()
    {
        var compilerHandle = CreateCompilerInternal();
        if (IntPtr.Zero == compilerHandle)
        {
            throw new InvalidOperationException("Unable to create sass compiler.");
        }
        return new InteropSassCompiler(compilerHandle, true);
    }

    public IReadOnlyList<GenericPath> GetIncludedFiles()
        => Interop.ReadUtf8FileInfoProviderList(Interop.sass_context_get_included_files(this));

    public abstract int Compile();

    void IInteropSassOptionsMemory.UpdateIndent(string indent, Action<PinnableUtf8String> action)
        => GenericExtensions.UpdatePinnable(ref _pinnableIndent, indent, action);

    void IInteropSassOptionsMemory.UpdateLineFeed(string lineFeed, Action<PinnableUtf8String> action)
        => GenericExtensions.UpdatePinnable(ref _pinnableLineFeed, lineFeed, action);
}