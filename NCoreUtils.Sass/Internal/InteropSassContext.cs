using System;
using System.Buffers;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;

namespace NCoreUtils.Sass.Internal
{
    public abstract class InteropSassContext : SafeHandle, IInteropSassOptionsMemory
    {
        public static InteropSassFileContext CreateFromFile(string path)
        {
            var handle = Interop.PassUtf8String(path, ptr => Interop.sass_make_file_context(ptr));
            if (IntPtr.Zero == handle)
            {
                throw new InvalidOperationException($"Failed to create file context from \"{path}\".");
            }
            return new InteropSassFileContext(handle, true);
        }

        public unsafe static InteropSassDataContext CreateFromData(string sassCode)
        {
            var maxBufferSize = Interop._utf8.GetMaxByteCount(sassCode.Length);
            using var memoryOwner = MemoryPool<byte>.Shared.Rent(maxBufferSize + 1);
            var buffer = memoryOwner.Memory.Span;
            var length = Interop._utf8.GetBytes(sassCode.AsSpan(), buffer);
            buffer[length] = 0;
            IntPtr sassString;
            fixed (byte* ptr = buffer)
            {
                sassString = Interop.sass_copy_c_string((IntPtr)ptr);
            }
            var handle = Interop.sass_make_data_context(sassString);
            if (IntPtr.Zero == handle)
            {
                throw new InvalidOperationException($"Failed to create file context from sass source.");
            }
            return new InteropSassDataContext(handle, true);
        }

        private PinnedUtf8String? _pinnedIndent;

        private PinnedUtf8String? _pinnedLineFeed;

        public override bool IsInvalid => IntPtr.Zero == handle;

        public int ErrorColumn
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Interop.sass_context_get_error_column(GetInnerContextInternal());
        }

        public string? ErrorFile
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Interop.ReadOptionalUtf8String(Interop.sass_context_get_error_file(GetInnerContextInternal()));
        }

        public string? ErrorJson
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Interop.ReadOptionalUtf8String(Interop.sass_context_get_error_json(GetInnerContextInternal()));
        }

        public int ErrorLine
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Interop.sass_context_get_error_line(GetInnerContextInternal());
        }

        public string? ErrorMessage
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Interop.ReadOptionalUtf8String(Interop.sass_context_get_error_message(GetInnerContextInternal()));
        }

        public string? ErrorSrc
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Interop.ReadOptionalUtf8String(Interop.sass_context_get_error_src(GetInnerContextInternal()));
        }

        public int ErrorStatus
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Interop.sass_context_get_error_status(GetInnerContextInternal());
        }

        public string? ErrorText
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Interop.ReadOptionalUtf8String(Interop.sass_context_get_error_text(GetInnerContextInternal()));
        }

        public string? OutputString
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Interop.ReadOptionalUtf8String(Interop.sass_context_get_output_string(GetInnerContextInternal()));
        }

        public InteropSassOptions Options
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new InteropSassOptions(GetOptionsInternal(), false, this);
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => SetOptionsInternal(value.DangerousGetHandle());
        }

        public string? SourceMapString
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Interop.ReadOptionalUtf8String(Interop.sass_context_get_source_map_string(GetInnerContextInternal()));
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

        protected abstract IntPtr GetInnerContextInternal();

        protected abstract IntPtr GetOptionsInternal();

        protected override bool ReleaseHandle()
        {
            _pinnedIndent?.Dispose();
            _pinnedLineFeed?.Dispose();
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

        public IReadOnlyList<FileInfoProvider> GetIncludedFiles()
            => Interop.ReadUtf8FileInfoProviderList(Interop.sass_context_get_included_files(handle));

        public abstract int Compile();

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