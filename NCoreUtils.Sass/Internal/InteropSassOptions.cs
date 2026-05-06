using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;

namespace NCoreUtils.Sass.Internal;

public class InteropSassOptions : SafeHandle, IInteropSassOptionsMemory
{
    public static InteropSassOptions Create()
    {
        var handle = Interop.sass_make_options();
        if (handle == IntPtr.Zero)
        {
            throw new InvalidOperationException($"Failed to create SassOptions.");
        }
        return new InteropSassOptions(handle, true, default);
    }

    private readonly IInteropSassOptionsMemory _pinnedMemoryOwner;

    private PinnableUtf8String? _pinnedIndent;

    private PinnableUtf8String? _pinnedLineFeed;

    public override bool IsInvalid => handle == IntPtr.Zero;

    public InteropSassOptionIncludePaths IncludePaths
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => new(this);
    }

    /// <summary>
    /// NOTE: indent string is not copied, but used as pointer directly thus we must keep memory pinned.
    /// </summary>
    public string Indent
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Interop.sass_option_get_indent(this);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => _pinnedMemoryOwner.UpdateIndent(value, pinnable => Interop.sass_option_set_indent(this, pinnable));
    }

    public string? InputPath
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Interop.sass_option_get_input_path(this);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => Interop.sass_option_set_input_path(this, value);
    }

    public bool IsIndentedSyntaxSrc
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Interop.sass_option_get_is_indented_syntax_src(this);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => Interop.sass_option_set_is_indented_syntax_src(this, value);
    }

    /// <summary>
    /// NOTE: indent string is not copied, but used as pointer directly thus we must keep memory pinned.
    /// </summary>
    public string LineFeed
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Interop.sass_option_get_linefeed(this);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => _pinnedMemoryOwner.UpdateLineFeed(value, pinnable => Interop.sass_option_set_linefeed(this, pinnable));
    }

    public bool OmitSourceMapUrl
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Interop.sass_option_get_omit_source_map_url(this);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => Interop.sass_option_set_omit_source_map_url(this, value);
    }

    public string? OutputPath
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Interop.sass_option_get_output_path(this);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => Interop.sass_option_set_output_path(this, value);
    }

    public SassOutputStyle OutputStyle
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => (SassOutputStyle)Interop.sass_option_get_output_style(this);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => Interop.sass_option_set_output_style(this, (int)value);
    }

    public int Precision
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Interop.sass_option_get_precision(this);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => Interop.sass_option_set_precision(this, value);
    }

    public bool SourceComments
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Interop.sass_option_get_source_comments(this);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => Interop.sass_option_set_source_comments(this, value);
    }

    public bool SourceMapContents
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Interop.sass_option_get_source_map_contents(this);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => Interop.sass_option_set_source_map_contents(this, value);
    }

    public bool SourceMapEmbed
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Interop.sass_option_get_source_map_embed(this);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => Interop.sass_option_set_source_map_embed(this, value);
    }

    public string? SourceMapFile
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Interop.sass_option_get_source_map_file(this);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => Interop.sass_option_set_source_map_file(this, value);
    }

    public bool SourceMapFileUrls
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Interop.sass_option_get_source_map_file_urls(this);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => Interop.sass_option_set_source_map_file_urls(this, value);
    }

    public string? SourceMapRoot
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Interop.sass_option_get_source_map_root(this);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => Interop.sass_option_set_source_map_root(this, value);
    }

    internal InteropSassOptions(IntPtr handle, bool ownsHandle, IInteropSassOptionsMemory? pinnedMemoryOwner)
        : base(IntPtr.Zero, ownsHandle)
    {
        _pinnedMemoryOwner = pinnedMemoryOwner ?? this;
        SetHandle(handle);
    }

    protected override bool ReleaseHandle()
    {
        Interop.sass_delete_options(this);
        _pinnedIndent?.Dispose();
        _pinnedLineFeed?.Dispose();
        return true;
    }

    void IInteropSassOptionsMemory.UpdateIndent(string indent, Action<PinnableUtf8String> action)
        => GenericExtensions.UpdatePinnable(ref _pinnedIndent, indent, action);

    void IInteropSassOptionsMemory.UpdateLineFeed(string lineFeed, Action<PinnableUtf8String> action)
        => GenericExtensions.UpdatePinnable(ref _pinnedLineFeed, lineFeed, action);
}