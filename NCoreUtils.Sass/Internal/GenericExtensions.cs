using System;
using System.Runtime.CompilerServices;

namespace NCoreUtils.Sass.Internal;

internal static class GenericExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T? Exchange<T>(ref T? target, T? value)
        where T : class
    {
        var old = target;
        target = value;
        return old;
    }

    public static void UpdatePinnable(ref PinnableUtf8String? target, string value, Action<PinnableUtf8String> action)
    {
        var pinnable = new PinnableUtf8String(value);
        try
        {
            action(pinnable);
            Exchange(ref target, pinnable)?.Dispose();
        }
        catch
        {
            pinnable.Dispose();
            throw;
        }
    }
}