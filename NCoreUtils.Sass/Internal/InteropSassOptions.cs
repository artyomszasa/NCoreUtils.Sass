using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;

namespace NCoreUtils.Sass.Internal
{
    public class InteropSassOptions : SafeHandle, IInteropSassOptionsMemory
    {
        public static InteropSassOptions Create()
        {
            var handle = Interop.sass_make_options();
            if (handle == IntPtr.Zero)
            {
                throw new InvalidOperationException($"Unable to create SassOptions.");
            }
            return new InteropSassOptions(handle, true, default);
        }

        private readonly IInteropSassOptionsMemory _pinnedMemoryOwner;

        private PinnedUtf8String? _pinnedIndent;

        private PinnedUtf8String? _pinnedLineFeed;

        public override bool IsInvalid => handle == IntPtr.Zero;

        public InteropSassOptionIncludePaths IncludePaths
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new InteropSassOptionIncludePaths(this);
        }

        /// <summary>
        /// NOTE: indent string is not copied, but used as pointer directly thus we must keep memory pinned.
        /// </summary>
        public string Indent
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Interop.ReadUtf8String(Interop.sass_option_get_indent(handle));
            set => Interop.sass_option_set_indent(handle, _pinnedMemoryOwner.PinIndentString(value));
        }

        public string? InputPath
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Interop.ReadOptionalUtf8String(Interop.sass_option_get_input_path(handle));
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                if (value is null)
                {
                    Interop.sass_option_set_input_path(handle, IntPtr.Zero);
                }
                else
                {
                    Interop.PassUtf8String(value, ptr => Interop.sass_option_set_input_path(handle, ptr));
                }
            }
        }

        public bool IsIndentedSyntaxSrc
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Interop.sass_option_get_is_indented_syntax_src(handle);
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => Interop.sass_option_set_is_indented_syntax_src(handle, value);
        }

        /// <summary>
        /// NOTE: indent string is not copied, but used as pointer directly thus we must keep memory pinned.
        /// </summary>
        public string LineFeed
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Interop.ReadUtf8String(Interop.sass_option_get_linefeed(handle));
            set => Interop.sass_option_set_linefeed(handle, _pinnedMemoryOwner.PinLineFeedString(value));
        }

        public bool OmitSourceMapUrl
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Interop.sass_option_get_omit_source_map_url(handle);
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => Interop.sass_option_set_omit_source_map_url(handle, value);
        }

        public string? OutputPath
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Interop.ReadOptionalUtf8String(Interop.sass_option_get_output_path(handle));
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                if (value is null)
                {
                    Interop.sass_option_set_output_path(handle, IntPtr.Zero);
                }
                else
                {
                    Interop.PassUtf8String(value, ptr => Interop.sass_option_set_output_path(handle, ptr));
                }
            }
        }

        public SassOutputStyle OutputStyle
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (SassOutputStyle)Interop.sass_option_get_output_style(handle);
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => Interop.sass_option_set_output_style(handle, (int)value);
        }

        public int Precision
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Interop.sass_option_get_precision(handle);
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => Interop.sass_option_set_precision(handle, value);
        }

        public bool SourceComments
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Interop.sass_option_get_source_comments(handle);
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => Interop.sass_option_set_source_comments(handle, value);
        }

        public bool SourceMapContents
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Interop.sass_option_get_source_map_contents(handle);
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => Interop.sass_option_set_source_map_contents(handle, value);
        }

        public bool SourceMapEmbed
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Interop.sass_option_get_source_map_embed(handle);
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => Interop.sass_option_set_source_map_embed(handle, value);
        }

        public string? SourceMapFile
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Interop.ReadOptionalUtf8String(Interop.sass_option_get_source_map_file(handle));
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                if (value is null)
                {
                    Interop.sass_option_set_source_map_file(handle, IntPtr.Zero);
                }
                else
                {
                    Interop.PassUtf8String(value, ptr => Interop.sass_option_set_source_map_file(handle, ptr));
                }
            }
        }

        public bool SourceMapFileUrls
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Interop.sass_option_get_source_map_file_urls(handle);
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => Interop.sass_option_set_source_map_file_urls(handle, value);
        }

        public string? SourceMapRoot
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Interop.ReadOptionalUtf8String(Interop.sass_option_get_source_map_root(handle));
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                if (value is null)
                {
                    Interop.sass_option_set_source_map_root(handle, IntPtr.Zero);
                }
                else
                {
                    Interop.PassUtf8String(value, ptr => Interop.sass_option_set_source_map_root(handle, ptr));
                }
            }
        }

        internal InteropSassOptions(IntPtr handle, bool ownsHandle, IInteropSassOptionsMemory? pinnedMemoryOwner)
            : base(IntPtr.Zero, ownsHandle)
        {
            _pinnedMemoryOwner = pinnedMemoryOwner ?? this;
            SetHandle(handle);
        }

        protected override bool ReleaseHandle()
        {
            Interop.sass_delete_options(handle);
            _pinnedIndent?.Dispose();
            _pinnedLineFeed?.Dispose();
            return true;
        }

        IntPtr IInteropSassOptionsMemory.PinIndentString(string input)
        {
            var pinned = new PinnedUtf8String(input);
            Interlocked.Exchange(ref _pinnedIndent, pinned)?.Dispose();
            return pinned.Pointer;
        }

        IntPtr IInteropSassOptionsMemory.PinLineFeedString(string input)
        {
            var pinned = new PinnedUtf8String(input);
            Interlocked.Exchange(ref _pinnedLineFeed, pinned)?.Dispose();
            return pinned.Pointer;
        }
    }
}