using System;

namespace NCoreUtils.Sass.Internal
{
    public interface IInteropSassOptionsMemory : IDisposable
    {
        IntPtr PinIndentString(string input);

        IntPtr PinLineFeedString(string input);
    }
}