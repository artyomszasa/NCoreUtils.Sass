using System;
using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using System.Threading;

namespace NCoreUtils.Sass.Internal;

internal static partial class Interop
{
    #region marshalling

    /// <summary>
    /// Reads string owned by the library into managed string.
    /// </summary>
    [CustomMarshaller(typeof(string), MarshalMode.ManagedToUnmanagedOut, typeof(NativeToManaged))]
    [CustomMarshaller(typeof(string), MarshalMode.UnmanagedToManagedIn, typeof(NativeToManaged))]
    internal static class SassNotOwningUtf8StringMarshaller
    {
        public static class NativeToManaged
        {
            public static string ConvertToManaged(IntPtr unmanaged)
                => ReadUtf8String(unmanaged);

            public static void Free(IntPtr _) { /* noop */ }
        }
    }

    /// <summary>
    /// Reads string owned by the library into managed string (string maybe null).
    /// </summary>
    [CustomMarshaller(typeof(string), MarshalMode.ManagedToUnmanagedOut, typeof(NativeToManaged))]
    [CustomMarshaller(typeof(string), MarshalMode.UnmanagedToManagedIn, typeof(NativeToManaged))]
    internal static class SassNotOwningOptionalUtf8StringMarshaller
    {
        public static class NativeToManaged
        {
            public static string? ConvertToManaged(IntPtr unmanaged)
                => ReadOptionalUtf8String(unmanaged);

            public static void Free(IntPtr _) { /* noop */ }
        }
    }

    [CustomMarshaller(typeof(InteropSassOptions), MarshalMode.ManagedToUnmanagedIn, typeof(ManagedToNative))]
    [CustomMarshaller(typeof(InteropSassOptions), MarshalMode.UnmanagedToManagedOut, typeof(ManagedToNative))]
    internal static class SassOptionsMarshaller
    {
        public static class ManagedToNative
        {
            /// <summary>
            /// Converts a managed type to an unmanaged representation. May throw exceptions.
            /// </summary>
            public static nint ConvertToUnmanaged(InteropSassOptions managed)
                => managed.DangerousGetHandle();

            /// <summary>
            /// Optional.
            /// Frees any unmanaged resources associated with the marshalling of the managed type.
            /// Must not throw exceptions.
            /// </summary>
            public static void Free(nint _) { /* noop */ }
        }
    }

    [CustomMarshaller(typeof(InteropSassContext), MarshalMode.ManagedToUnmanagedIn, typeof(ManagedToNative))]
    internal static class SassContextMarshaller
    {
        public static class ManagedToNative
        {
            /// <summary>
            /// Converts a managed type to an unmanaged representation. May throw exceptions.
            /// </summary>
            public static nint ConvertToUnmanaged(InteropSassContext managed)
                => managed.DangerousGetHandle();

            /// <summary>
            /// Optional.
            /// Frees any unmanaged resources associated with the marshalling of the managed type.
            /// Must not throw exceptions.
            /// </summary>
            public static void Free(nint _) { /* noop */ }
        }
    }

    [CustomMarshaller(typeof(string), MarshalMode.ManagedToUnmanagedIn, typeof(ManagedToNative))]
    [CustomMarshaller(typeof(string), MarshalMode.UnmanagedToManagedOut, typeof(ManagedToNative))]
    static class ViewOnlyUtf8StringMarshaller
    {
        public unsafe ref struct ManagedToNative
        {
            private const int MaxPooledBufferSize = 8 * 1024;

            private static ReadOnlySpan<byte> EmptyString => [ 0 ];

            private byte[]? _buffer;

            private GCHandle _gcHandle;

            private ReadOnlySpan<byte> _span;

            private bool _pooled;

            // /// <summary>
            // /// Optional constructor.
            // /// May throw exceptions.
            // /// </summary>
            // public ManagedToNative();

            public readonly int DataSize
                => _span.Length;

            /// <summary>
            /// Takes a managed type to be converted to an unmanaged representation in ToUnmanaged or GetPinnableReference.
            /// </summary>
            public void FromManaged(string? source)
            {
                if (source is not null)
                {
                    if (source.Length == 0)
                    {
                        _span = EmptyString;
                    }
                    else
                    {
                        var bufferSize = _utf8.GetByteCount(source) + 1;
                        if (bufferSize <= MaxPooledBufferSize)
                        {
                            _buffer = ArrayPool<byte>.Shared.Rent(bufferSize);
                            _pooled = true;
                        }
                        else
                        {
                            _buffer = new byte[bufferSize];
                            _pooled = false;
                        }
                        var dataSize = _utf8.GetBytes(source, _buffer);
                        _buffer[dataSize] = 0;
                        _span = new(_buffer, 0, dataSize + 1);
                        _gcHandle = GCHandle.Alloc(_buffer, GCHandleType.Pinned);
                    }
                }
            }

            /// <summary>
            /// Converts the managed type to an unmanaged representation.
            /// May throw exceptions.
            /// </summary>
            public readonly nint ToUnmanaged()
            {
                if (_span.IsEmpty)
                {
                    return IntPtr.Zero;
                }
                if (_gcHandle.IsAllocated)
                {
                    return _gcHandle.AddrOfPinnedObject();
                }
                return (nint)Unsafe.AsPointer(ref MemoryMarshal.GetReference(_span));
            }

            // /// <summary>
            // /// Optional.
            // /// In managed to unmanaged stubs, this method is called after call to the unmanaged code.
            // /// Must not throw exceptions.
            // /// </summary>
            // public void OnInvoked();

            /// <summary>
            /// Optional.
            /// Frees any unmanaged resources associated with the marshalling of the managed type.
            /// Must not throw exceptions.
            /// </summary>
            public void Free()
            {
                if (_gcHandle.IsAllocated)
                {
                    _gcHandle.Free();
                }
                if (_buffer is byte[] buffer)
                {
                    if (_pooled)
                    {
                        ArrayPool<byte>.Shared.Return(buffer);
                    }
                    _buffer = default;
                }
                _span = default;
            }
        }
    }

    [CustomMarshaller(typeof(PinnableUtf8String), MarshalMode.ManagedToUnmanagedIn, typeof(ManagedToNative))]
    [CustomMarshaller(typeof(PinnableUtf8String), MarshalMode.UnmanagedToManagedOut, typeof(ManagedToNative))]
    internal static class PinnableUtf8StringMarshaller
    {
        public ref struct ManagedToNative
        {
            private PinnableUtf8String _source;

            /// <summary>
            /// Takes a managed type to be converted to an unmanaged representation in ToUnmanaged or GetPinnableReference.
            /// </summary>
            public void FromManaged(PinnableUtf8String source)
                => _source = source;

            /// <summary>
            /// Converts the managed type to an unmanaged representation.
            /// May throw exceptions.
            /// </summary>
            public readonly nint ToUnmanaged()
                => _source.Pin();

            /// <summary>
            /// Optional.
            /// Frees any unmanaged resources associated with the marshalling of the managed type.
            /// Must not throw exceptions.
            /// </summary>
            public readonly void Free()
            {
                _source.Unpin();
            }
        }
    }

    [CustomMarshaller(typeof(IInteropSassManagedMemory), MarshalMode.ManagedToUnmanagedIn, typeof(ManagedToNative))]
    internal static class SassManagerMemoryFreeMarshaller
    {
        public ref struct ManagedToNative
        {
            private IInteropSassManagedMemory _managed;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void FromManaged(IInteropSassManagedMemory managed)
                => _managed = managed;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public readonly nint ToUnmanaged()
                => _managed.Ptr;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public readonly void Free()
                => _managed.Ptr = nint.Zero;
        }
    }

    [CustomMarshaller(typeof(InteropSassManagedString), MarshalMode.ManagedToUnmanagedIn, typeof(ManagedToNative))]
    internal static class SassManagedStringUnownMarshaller
    {
        public ref struct ManagedToNative
        {
            private InteropSassManagedString _managed;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void FromManaged(InteropSassManagedString managed)
                => _managed = managed;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public readonly nint ToUnmanaged()
                => ((IInteropSassManagedMemory)_managed).Ptr;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public readonly void OnInvoked()
                => _managed.MarkAsNotOwned();

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public readonly void Free() { }
        }
    }

    #endregion

    [LibraryImport(Libname, EntryPoint = "sass_copy_c_string", SetLastError = false)]
    private static partial nint sass_copy_c_string_impl(nint @string);

    [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Interop convention: must match target function name")]
    [return: NotNullIfNotNull(nameof(@string))]
    public static InteropSassManagedString? sass_copy_c_string(string? @string)
    {
        nint __retVal = default;
        // Setup - Perform required setup.
        scoped ViewOnlyUtf8StringMarshaller.ManagedToNative __string_native__marshaller = new();
        int size = 0;
        try
        {
            // Marshal - Convert managed data to native data.
            __string_native__marshaller.FromManaged(@string);
            size = __string_native__marshaller.DataSize;
            {
                // PinnedMarshal - Convert managed data to native data that requires the managed data to be pinned.
                nint __string_native = __string_native__marshaller.ToUnmanaged();
                __retVal = sass_copy_c_string_impl(__string_native);
            }
        }
        finally
        {
            // CleanupCallerAllocated - Perform cleanup of caller allocated resources.
            __string_native__marshaller.Free();
        }

        return __retVal == nint.Zero
            ? default
            : new InteropSassManagedString(__retVal, size, true);
    }

    [LibraryImport(Libname, SetLastError = false)]
    public static partial void sass_free_memory([MarshalUsing(typeof(SassManagerMemoryFreeMarshaller))] IInteropSassManagedMemory managed);

    #region SassOptions

    [LibraryImport(Libname, SetLastError = false)]
    public static partial IntPtr sass_make_options();

    [LibraryImport(Libname, SetLastError = false)]
    public static partial void sass_delete_options([MarshalUsing(typeof(SassOptionsMarshaller))] InteropSassOptions options);

    [LibraryImport(Libname, SetLastError = false)]
    public static partial int sass_option_get_precision([MarshalUsing(typeof(SassOptionsMarshaller))] InteropSassOptions options);

    [LibraryImport(Libname, SetLastError = false)]
    public static partial int sass_option_get_output_style([MarshalUsing(typeof(SassOptionsMarshaller))] InteropSassOptions options);

    [LibraryImport(Libname, SetLastError = false)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool sass_option_get_source_comments([MarshalUsing(typeof(SassOptionsMarshaller))] InteropSassOptions options);

    [LibraryImport(Libname, SetLastError = false)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool sass_option_get_source_map_embed([MarshalUsing(typeof(SassOptionsMarshaller))] InteropSassOptions options);

    [LibraryImport(Libname, SetLastError = false)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool sass_option_get_source_map_contents([MarshalUsing(typeof(SassOptionsMarshaller))] InteropSassOptions options);

    [LibraryImport(Libname, SetLastError = false)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool sass_option_get_source_map_file_urls([MarshalUsing(typeof(SassOptionsMarshaller))] InteropSassOptions options);

    [LibraryImport(Libname, SetLastError = false)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool sass_option_get_omit_source_map_url([MarshalUsing(typeof(SassOptionsMarshaller))] InteropSassOptions options);

    [LibraryImport(Libname, SetLastError = false)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool sass_option_get_is_indented_syntax_src([MarshalUsing(typeof(SassOptionsMarshaller))] InteropSassOptions options);

    [LibraryImport(Libname, SetLastError = false)]
    [return: MarshalUsing(typeof(SassNotOwningUtf8StringMarshaller))]
    public static partial string sass_option_get_indent([MarshalUsing(typeof(SassOptionsMarshaller))] InteropSassOptions options);

    [LibraryImport(Libname, SetLastError = false)]
    [return: MarshalUsing(typeof(SassNotOwningUtf8StringMarshaller))]
    public static partial string sass_option_get_linefeed([MarshalUsing(typeof(SassOptionsMarshaller))] InteropSassOptions options);

    [LibraryImport(Libname, SetLastError = false)]
    [return: MarshalUsing(typeof(SassNotOwningUtf8StringMarshaller))]
    public static partial string sass_option_get_input_path([MarshalUsing(typeof(SassOptionsMarshaller))] InteropSassOptions options);

    [LibraryImport(Libname, SetLastError = false)]
    [return: MarshalUsing(typeof(SassNotOwningUtf8StringMarshaller))]
    public static partial string sass_option_get_output_path([MarshalUsing(typeof(SassOptionsMarshaller))] InteropSassOptions options);

    [LibraryImport(Libname, SetLastError = false)]
    [return: MarshalUsing(typeof(SassNotOwningUtf8StringMarshaller))]
    public static partial string sass_option_get_source_map_file([MarshalUsing(typeof(SassOptionsMarshaller))] InteropSassOptions options);

    [LibraryImport(Libname, SetLastError = false)]
    [return: MarshalUsing(typeof(SassNotOwningUtf8StringMarshaller))]
    public static partial string sass_option_get_source_map_root([MarshalUsing(typeof(SassOptionsMarshaller))] InteropSassOptions options);

    [LibraryImport(Libname, SetLastError = false)]
    public static partial IntPtr sass_option_get_c_functions([MarshalUsing(typeof(SassOptionsMarshaller))] InteropSassOptions options);

    [LibraryImport(Libname, SetLastError = false)]
    public static partial IntPtr sass_option_get_importer([MarshalUsing(typeof(SassOptionsMarshaller))] InteropSassOptions options);

    [LibraryImport(Libname, SetLastError = false)]
    public static partial int sass_option_get_include_path_size([MarshalUsing(typeof(SassOptionsMarshaller))] InteropSassOptions options);

    [LibraryImport(Libname, SetLastError = false)]
    [return: MarshalUsing(typeof(SassNotOwningUtf8StringMarshaller))]
    public static partial string sass_option_get_include_path([MarshalUsing(typeof(SassOptionsMarshaller))] InteropSassOptions options, int index);

    [LibraryImport(Libname, SetLastError = false)]
    public static partial int sass_option_get_plugin_path_size([MarshalUsing(typeof(SassOptionsMarshaller))] InteropSassOptions options);

    [LibraryImport(Libname, SetLastError = false)]
    [return: MarshalUsing(typeof(SassNotOwningUtf8StringMarshaller))]
    public static partial string sass_option_get_plugin_path([MarshalUsing(typeof(SassOptionsMarshaller))] InteropSassOptions options, int index);

    [LibraryImport(Libname, SetLastError = false)]
    public static partial void sass_option_set_precision([MarshalUsing(typeof(SassOptionsMarshaller))] InteropSassOptions options, int precision);

    [LibraryImport(Libname, SetLastError = false)]
    public static partial void sass_option_set_output_style([MarshalUsing(typeof(SassOptionsMarshaller))] InteropSassOptions options, int output_style);

    [LibraryImport(Libname, SetLastError = false)]
    public static partial void sass_option_set_source_comments(
        [MarshalUsing(typeof(SassOptionsMarshaller))] InteropSassOptions options,
        [MarshalAs(UnmanagedType.Bool)] bool source_comments
    );

    [LibraryImport(Libname, SetLastError = false)]
    public static partial void sass_option_set_source_map_embed(
        [MarshalUsing(typeof(SassOptionsMarshaller))] InteropSassOptions options,
        [MarshalAs(UnmanagedType.Bool)] bool source_map_embed
    );

    [LibraryImport(Libname, SetLastError = false)]
    public static partial void sass_option_set_source_map_contents(
        [MarshalUsing(typeof(SassOptionsMarshaller))] InteropSassOptions options,
        [MarshalAs(UnmanagedType.Bool)] bool source_map_contents
    );

    [LibraryImport(Libname, SetLastError = false)]
    public static partial void sass_option_set_source_map_file_urls(
        [MarshalUsing(typeof(SassOptionsMarshaller))] InteropSassOptions options,
        [MarshalAs(UnmanagedType.Bool)] bool source_map_file_urls
    );

    [LibraryImport(Libname, SetLastError = false)]
    public static partial void sass_option_set_omit_source_map_url(
        [MarshalUsing(typeof(SassOptionsMarshaller))] InteropSassOptions options,
        [MarshalAs(UnmanagedType.Bool)] bool omit_source_map_url
    );

    [LibraryImport(Libname, SetLastError = false)]
    public static partial void sass_option_set_is_indented_syntax_src(
        [MarshalUsing(typeof(SassOptionsMarshaller))] InteropSassOptions options,
        [MarshalAs(UnmanagedType.Bool)] bool is_indented_syntax_src
    );

    [LibraryImport(Libname, SetLastError = false)]
    public static partial void sass_option_set_indent(
        [MarshalUsing(typeof(SassOptionsMarshaller))] InteropSassOptions options,
        [MarshalUsing(typeof(PinnableUtf8StringMarshaller))] PinnableUtf8String indent
    );

    [LibraryImport(Libname, SetLastError = false)]
    public static partial void sass_option_set_linefeed(
        [MarshalUsing(typeof(SassOptionsMarshaller))] InteropSassOptions options,
        [MarshalUsing(typeof(PinnableUtf8StringMarshaller))] PinnableUtf8String linefeed
    );

    [LibraryImport(Libname, SetLastError = false)]
    public static partial void sass_option_set_input_path(
        [MarshalUsing(typeof(SassOptionsMarshaller))] InteropSassOptions options,
        [MarshalUsing(typeof(ViewOnlyUtf8StringMarshaller))] string? input_path
    );

    [LibraryImport(Libname, SetLastError = false)]
    public static partial void sass_option_set_output_path(
        [MarshalUsing(typeof(SassOptionsMarshaller))] InteropSassOptions options,
        [MarshalUsing(typeof(ViewOnlyUtf8StringMarshaller))] string? output_path
    );

    [LibraryImport(Libname, SetLastError = false)]
    public static partial void sass_option_set_plugin_path(
        [MarshalUsing(typeof(SassOptionsMarshaller))] InteropSassOptions options,
        [MarshalUsing(typeof(ViewOnlyUtf8StringMarshaller))] string? plugin_path
    );

    [LibraryImport(Libname, SetLastError = false)]
    public static partial void sass_option_set_include_path(
        [MarshalUsing(typeof(SassOptionsMarshaller))] InteropSassOptions options,
        [MarshalUsing(typeof(ViewOnlyUtf8StringMarshaller))] string? include_path
    );

    [LibraryImport(Libname, SetLastError = false)]
    public static partial void sass_option_set_source_map_file(
        [MarshalUsing(typeof(SassOptionsMarshaller))] InteropSassOptions options,
        [MarshalUsing(typeof(ViewOnlyUtf8StringMarshaller))] string? source_map_file
    );

    [LibraryImport(Libname, SetLastError = false)]
    public static partial void sass_option_set_source_map_root(
        [MarshalUsing(typeof(SassOptionsMarshaller))] InteropSassOptions options,
        [MarshalUsing(typeof(ViewOnlyUtf8StringMarshaller))] string? source_map_root
    );

    [LibraryImport(Libname, SetLastError = false)]
    public static partial void sass_option_set_c_functions(
        [MarshalUsing(typeof(SassOptionsMarshaller))] InteropSassOptions options,
        IntPtr c_functions
    );

    [LibraryImport(Libname, SetLastError = false)]
    public static partial void sass_option_set_importer(
        [MarshalUsing(typeof(SassOptionsMarshaller))] InteropSassOptions options,
        IntPtr importer
    );

    [LibraryImport(Libname, SetLastError = false)]
    public static partial void sass_option_push_plugin_path(
        [MarshalUsing(typeof(SassOptionsMarshaller))] InteropSassOptions options,
        [MarshalUsing(typeof(ViewOnlyUtf8StringMarshaller))] string path
    );

    [LibraryImport(Libname, SetLastError = false)]
    public static partial void sass_option_push_include_path(
        [MarshalUsing(typeof(SassOptionsMarshaller))] InteropSassOptions options,
        [MarshalUsing(typeof(ViewOnlyUtf8StringMarshaller))] string path
    );

    #endregion

    #region Context

    [LibraryImport(Libname, SetLastError = false)]
    // public static partial IntPtr sass_make_file_context(IntPtr input_path);
    public static partial IntPtr sass_make_file_context([MarshalUsing(typeof(ViewOnlyUtf8StringMarshaller))] string input_path);

    /// <summary>
    /// NOTE: <c>source_string</c> is mutable!
    /// </summary>
    /// <param name="source_string">Sass source.</param>
    [LibraryImport(Libname, SetLastError = false)]
    public static partial IntPtr sass_make_data_context([MarshalUsing(typeof(SassManagedStringUnownMarshaller))] InteropSassManagedString source_string);

    [LibraryImport(Libname, SetLastError = false)]
    public static partial void sass_delete_file_context([MarshalUsing(typeof(SassContextMarshaller))] InteropSassContext ctx);

    [LibraryImport(Libname, SetLastError = false)]
    public static partial void sass_delete_data_context([MarshalUsing(typeof(SassContextMarshaller))] InteropSassContext ctx);

    [LibraryImport(Libname, SetLastError = false)]
    public static partial int sass_compile_file_context([MarshalUsing(typeof(SassContextMarshaller))] InteropSassContext ctx);

    [LibraryImport(Libname, SetLastError = false)]
    public static partial int sass_compile_data_context([MarshalUsing(typeof(SassContextMarshaller))] InteropSassContext ctx);

    [LibraryImport(Libname, SetLastError = false)]
    public static partial IntPtr sass_file_context_get_options([MarshalUsing(typeof(SassContextMarshaller))] InteropSassContext ctx);

    [LibraryImport(Libname, SetLastError = false)]
    public static partial IntPtr sass_data_context_get_options([MarshalUsing(typeof(SassContextMarshaller))] InteropSassContext ctx);

    [LibraryImport(Libname, SetLastError = false)]
    public static partial void sass_file_context_set_options([MarshalUsing(typeof(SassContextMarshaller))] InteropSassContext ctx, IntPtr options);

    [LibraryImport(Libname, SetLastError = false)]
    public static partial void sass_data_context_set_options([MarshalUsing(typeof(SassContextMarshaller))] InteropSassContext ctx, IntPtr options);

    // NOTE: implementation returns passed pointer
    // [LibraryImport(Libname, SetLastError = false)]
    // public static partial IntPtr sass_file_context_get_context(IntPtr file_ctx);

    // NOTE: implementation returns passed pointer
    // [LibraryImport(Libname, SetLastError = false)]
    // public static partial IntPtr sass_data_context_get_context(IntPtr file_ctx);

    [LibraryImport(Libname, SetLastError = false)]
    [return: MarshalUsing(typeof(SassNotOwningOptionalUtf8StringMarshaller))]
    public static partial string sass_context_get_output_string([MarshalUsing(typeof(SassContextMarshaller))] InteropSassContext ctx);

    [LibraryImport(Libname, SetLastError = false)]
    public static partial int sass_context_get_error_status([MarshalUsing(typeof(SassContextMarshaller))] InteropSassContext ctx);

    [LibraryImport(Libname, SetLastError = false)]
    [return: MarshalUsing(typeof(SassNotOwningOptionalUtf8StringMarshaller))]
    public static partial string? sass_context_get_error_json([MarshalUsing(typeof(SassContextMarshaller))] InteropSassContext ctx);

    [LibraryImport(Libname, SetLastError = false)]
    [return: MarshalUsing(typeof(SassNotOwningOptionalUtf8StringMarshaller))]
    public static partial string? sass_context_get_error_text([MarshalUsing(typeof(SassContextMarshaller))] InteropSassContext ctx);

    [LibraryImport(Libname, SetLastError = false)]
    [return: MarshalUsing(typeof(SassNotOwningOptionalUtf8StringMarshaller))]
    public static partial string? sass_context_get_error_message([MarshalUsing(typeof(SassContextMarshaller))] InteropSassContext ctx);

    [LibraryImport(Libname, SetLastError = false)]
    [return: MarshalUsing(typeof(SassNotOwningOptionalUtf8StringMarshaller))]
    public static partial string? sass_context_get_error_file([MarshalUsing(typeof(SassContextMarshaller))] InteropSassContext ctx);

    [LibraryImport(Libname, SetLastError = false)]
    [return: MarshalUsing(typeof(SassNotOwningOptionalUtf8StringMarshaller))]
    public static partial string? sass_context_get_error_src([MarshalUsing(typeof(SassContextMarshaller))] InteropSassContext ctx);

    [LibraryImport(Libname, SetLastError = false)]
    public static partial int sass_context_get_error_line([MarshalUsing(typeof(SassContextMarshaller))] InteropSassContext ctx);

    [LibraryImport(Libname, SetLastError = false)]
    public static partial int sass_context_get_error_column([MarshalUsing(typeof(SassContextMarshaller))] InteropSassContext ctx);

    [LibraryImport(Libname, SetLastError = false)]
    [return: MarshalUsing(typeof(SassNotOwningOptionalUtf8StringMarshaller))]
    public static partial string? sass_context_get_source_map_string([MarshalUsing(typeof(SassContextMarshaller))] InteropSassContext ctx);

    [LibraryImport(Libname, SetLastError = false)]
    public static partial IntPtr sass_context_get_included_files([MarshalUsing(typeof(SassContextMarshaller))] InteropSassContext ctx);

    #endregion

    #region Compiler

    [LibraryImport(Libname, SetLastError = false)]
    public static partial IntPtr sass_make_file_compiler(IntPtr file_ctx);

    [LibraryImport(Libname, SetLastError = false)]
    public static partial IntPtr sass_make_data_compiler(IntPtr data_ctx);

    [LibraryImport(Libname, SetLastError = false)]
    public static partial int sass_compiler_parse(IntPtr compiler);

    [LibraryImport(Libname, SetLastError = false)]
    public static partial int sass_compiler_execute(IntPtr compiler);

    [LibraryImport(Libname, SetLastError = false)]
    public static partial void sass_delete_compiler(IntPtr compiler);

    [LibraryImport(Libname, SetLastError = false)]
    public static partial int sass_compiler_get_import_stack_size(IntPtr compiler);

    [LibraryImport(Libname, SetLastError = false)]
    public static partial IntPtr sass_compiler_get_import_entry(IntPtr compiler, int idx);

    #endregion


}