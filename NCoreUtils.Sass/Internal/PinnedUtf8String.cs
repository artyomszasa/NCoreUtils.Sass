using System;
using System.Buffers;
using System.Threading;

namespace NCoreUtils.Sass.Internal
{
    public sealed class PinnedUtf8String : IDisposable
    {
        private int _isDisposed;

        private readonly IMemoryOwner<byte> _owner;

        private readonly MemoryHandle _handle;

        public unsafe IntPtr Pointer
        {
            get
            {
                if (0 != Interlocked.CompareExchange(ref _isDisposed, 0, 0))
                {
                    throw new ObjectDisposedException(nameof(PinnedUtf8String));
                }
                return (IntPtr)_handle.Pointer;
            }
        }

        public unsafe PinnedUtf8String(string source)
        {
            var maxLength = Interop._utf8.GetMaxByteCount(source.Length) + 1;
            _owner = MemoryPool<byte>.Shared.Rent(maxLength);
            var memory = _owner.Memory;
            _handle = _owner.Memory.Pin();
            int size;
            fixed (char* psource = source)
            {
                size = Interop._utf8.GetBytes(psource, source.Length, (byte*)_handle.Pointer, maxLength);
            }
            ((byte*)_handle.Pointer)[size] = 0;
        }

        ~PinnedUtf8String()
            => Dispose(false);

        private void Dispose(bool disposing)
        {
            if (0 == Interlocked.CompareExchange(ref _isDisposed, 1, 0))
            {
                _handle.Dispose();
                if (disposing)
                {
                    _owner.Dispose();
                }
            }
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            Dispose(true);
        }
    }
}