using System;

namespace NCoreUtils.Sass.Internal;

public sealed class InteropSassManagedString(nint ptr, int length, bool owned)
    : IDisposable
    , IInteropSassManagedMemory
{
    private nint _ptr = ptr;

    private bool _owned = owned;

    public int Length { get; } = length;

    public unsafe ReadOnlySpan<byte> Span
        => _ptr == nint.Zero
            ? default
            : new ReadOnlySpan<byte>((void*)_ptr, Length);

    nint IInteropSassManagedMemory.Ptr
    {
        get => _ptr;
        set => _ptr = value;
    }

    ~InteropSassManagedString()
        => DoDispose();

    private void DoDispose()
    {
        if (_ptr != nint.Zero)
        {
            if (_owned)
            {
                Interop.sass_free_memory(this);
                _owned = false;
            }
            else
            {
                _ptr = nint.Zero;
            }
        }
    }

    public void Dispose()
    {
        DoDispose();
        GC.SuppressFinalize(this);
    }

    internal void MarkAsNotOwned()
        => _owned = false;
}