using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;

namespace NCoreUtils.Sass.Internal;

internal static partial class Interop
{
    private readonly struct Pattern
    {
        private static bool Eq(ReadOnlySpan<char> a, ReadOnlySpan<char> b)
            => MemoryExtensions.Equals(a, b, StringComparison.InvariantCulture);

        private int MinLength { get; }

        public string Prefix { get; }

        public string Suffix { get; }

        public Pattern(string prefix, string suffix)
        {
            Prefix = prefix;
            Suffix = suffix;
            MinLength = prefix.Length + 1 + Suffix.Length;
        }

        public bool Match(ReadOnlySpan<char> input, out decimal version)
        {
            if (input.Length == Prefix.Length + Suffix.Length && Eq(input[..Prefix.Length], Prefix) && Eq(input[Prefix.Length..], Suffix))
            {
                version = 0;
                return true;
            }
            var prefixLength = Prefix.Length;
            if (Suffix.Length == 0)
            {
                if (input.Length < MinLength || !Eq(Prefix, input[..prefixLength]))
                {
                    version = default;
                    return false;
                }
                return decimal.TryParse(input[prefixLength..], NumberStyles.Float, CultureInfo.InvariantCulture, out version);
            }
            var suffixLength = Suffix.Length;
            if (input.Length < MinLength || !Eq(Prefix, input[..Prefix.Length]) || !Eq(Suffix, input[^suffixLength..]))
            {
                version = default;
                return false;
            }
            return decimal.TryParse(input[prefixLength..^suffixLength], NumberStyles.Float, CultureInfo.InvariantCulture, out version);
        }
    }

    static Interop()
    {
        NativeLibrary.SetDllImportResolver(typeof(Interop).Assembly, (libraryName, assembly, searchPath) =>
        {
            if (libraryName == Libname)
            {
                Pattern sassPattern;
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD))
                {
                    sassPattern = new("libsass.so.", string.Empty);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    sassPattern = new("sass.", ".dll");
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    sassPattern = new("libicuuc.", ".dylib");
                }
                else
                {
                    throw new InvalidOperationException($"Unsupported platform: {RuntimeInformation.OSDescription}");
                }
                foreach (var basePath in GetSearchPaths())
                {
                    if (!Directory.Exists(basePath))
                    {
                        continue;
                    }
                    foreach (var fullPath in Directory.EnumerateFiles(basePath))
                    {
                        if (sassPattern.Match(Path.GetFileName(fullPath), out var version))
                        {
                            try
                            {
                                if (NativeLibrary.TryLoad(fullPath, out var handle))
                                {
                                    return handle;
                                }
                            }
                            catch (Exception exn)
                            {
                                Console.Error.WriteLine(exn);
                            }
                        }
                    }
                }
            }
            return IntPtr.Zero;
        });
    }

    private static IEnumerable<string> GetSearchPaths()
    {
        yield return Environment.CurrentDirectory;
        var env = Environment.GetEnvironmentVariable("LIBSASS_PATH");
        if (!string.IsNullOrEmpty(env))
        {
            yield return env;
        }
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            yield return "/usr/lib/x86_64-linux-gnu";
            yield return "/lib/x86_64-linux-gnu";
            yield return "/usr/local/lib";
            yield return "/usr/lib";
            yield return "/lib";
        }
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            yield return "C:\\Windows\\System32";
        }
    }
}