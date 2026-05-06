using System;
using System.Buffers;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace NCoreUtils.Sass.Internal;

public sealed class PinnableUtf8String : IDisposable
{
    private byte[]? _data;

    private readonly int _size;

    public GCHandle _handle;

    public PinnableUtf8String(string source)
    {
        _data = ArrayPool<byte>.Shared.Rent(Interop._utf8.GetByteCount(source) + 1);
        var size = Interop._utf8.GetBytes(source, _data);
        _size = size + 1;
        _data[size] = 0;
    }

    ~PinnableUtf8String()
        => Dispose(false);

    public nint Pin()
    {
        Debug.Assert(!_handle.IsAllocated);
        return (_handle = GCHandle.Alloc(_data, GCHandleType.Pinned)).AddrOfPinnedObject();
    }

    public void Unpin()
    {
        Debug.Assert(_handle.IsAllocated);
        _handle.Free();
    }

    private void Dispose(bool disposing)
    {
        if (_handle.IsAllocated)
        {
            _handle.Free();
        }
        if (disposing)
        {
            if (Interlocked.CompareExchange(ref _data, null, _data) is byte[] data)
            {
                ArrayPool<byte>.Shared.Return(data);
            }
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    public override string ToString()
        => _data is byte[] data ? Interop._utf8.GetString(data, 0, _size) : "<disposed>";
}

[Obsolete]
public sealed class PinnedUtf8String : IDisposable
{
    private int _isDisposed;

    private readonly IMemoryOwner<byte> _owner;

    private readonly MemoryHandle _handle;

    public unsafe IntPtr Pointer
        => 0 != Interlocked.CompareExchange(ref _isDisposed, 0, 0)
            ? throw new ObjectDisposedException(nameof(PinnedUtf8String))
            : (IntPtr)_handle.Pointer;

    public PinnedUtf8String(string source)
    {
        _owner = MemoryPool<byte>.Shared.Rent(Interop._utf8.GetByteCount(source) + 1);
        var memory = _owner.Memory;
        _handle = memory.Pin();
        var size = Interop._utf8.GetBytes(source, memory.Span);
        memory.Span[size] = 0;
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