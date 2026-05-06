using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using System.Text;

namespace NCoreUtils.Sass.Internal;

internal static partial class Interop
{
    private const string Libname = "sass";

    internal static readonly UTF8Encoding _utf8 = new(false);

    #region string handling

    public unsafe static string? ReadOptionalUtf8String(IntPtr ptr)
    {
        if (IntPtr.Zero == ptr)
        {
            return default;
        }
        var p = (byte*)ptr;
        var i = 0;
        while (*(p + i) != 0)
        {
            ++i;
        }
        if (0 == i)
        {
            return string.Empty;
        }
        return _utf8.GetString(p, i);
    }

    public unsafe static string ReadUtf8String(IntPtr ptr)
    {
        var p = (byte*)ptr;
        if (0 == *p)
        {
            return string.Empty;
        }
        ++p;
        while (*p != 0) { ++p; }
        return _utf8.GetString(p, unchecked((int)(p - (byte*)ptr)));
    }

    public unsafe static List<string> ReadUtf8StringList(IntPtr ptr)
    {
        var list = new List<string>();
        if (IntPtr.Zero == ptr)
        {
            return list;
        }
        for (var p = (byte**)ptr; *p != (byte*)0; ++p)
        {
            var line = ReadOptionalUtf8String((IntPtr)(*p));
            if (line is not null)
            {
                list.Add(line);
            }
        }
        return list;
    }

    public unsafe static GenericPath[] ReadUtf8FileInfoProviderList(IntPtr ptr)
    {
        var builder = new ArrayBuilder<GenericPath>();
        for (var p = (byte**)ptr; *p != (byte*)0; ++p)
        {
            var line = ReadOptionalUtf8String((IntPtr)(*p));
            if (line is not null)
            {
                builder.Add(new GenericPath(line));
            }
        }
        return builder.ToArray();
    }

    public unsafe static void PassUtf8String(string input, Action<IntPtr> action)
    {
        if (input.Length == 0)
        {
            Span<byte> emptyBuffer = stackalloc byte[1];
            emptyBuffer[0] = 0;
            fixed (byte* ptr = emptyBuffer)
            {
                action((IntPtr)ptr);
            }
        }
        else
        {
            var bufferSize = _utf8.GetMaxByteCount(input.Length);
            Span<byte> buffer = stackalloc byte[bufferSize + 1];
            var dataSize = _utf8.GetBytes(input.AsSpan(), buffer);
            buffer[dataSize] = 0;
            fixed (byte* ptr = buffer)
            {
                action((IntPtr)ptr);
            }
        }
    }

    public unsafe static T PassUtf8String<T>(string input, Func<IntPtr, T> action)
    {
        if (input.Length == 0)
        {
            Span<byte> emptyBuffer = stackalloc byte[1];
            emptyBuffer[0] = 0;
            fixed (byte* ptr = emptyBuffer)
            {
                return action((IntPtr)ptr);
            }
        }
        var bufferSize = _utf8.GetMaxByteCount(input.Length);
        Span<byte> buffer = stackalloc byte[bufferSize + 1];
        var dataSize = _utf8.GetBytes(input.AsSpan(), buffer);
        buffer[dataSize] = 0;
        fixed (byte* ptr = buffer)
        {
            return action((IntPtr)ptr);
        }
    }

    #endregion
}