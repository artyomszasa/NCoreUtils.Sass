using System;

namespace NCoreUtils.Sass.Internal;

public interface IInteropSassOptionsMemory : IDisposable
{
    void UpdateIndent(string indent, Action<PinnableUtf8String> action);

    void UpdateLineFeed(string lineFeed, Action<PinnableUtf8String> action);
}