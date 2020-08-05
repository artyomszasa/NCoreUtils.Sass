using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace NCoreUtils.Sass.Internal
{
    public readonly struct InteropSassOptionIncludePaths : IReadOnlyList<string>
    {
        public struct InteropSassOptionIncludePathsEnumerator : IEnumerator<string>
        {
            private readonly InteropSassOptions _options;

            private int _index;

            private string? _current;

            object IEnumerator.Current => Current;

            public string Current
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => _current!;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public InteropSassOptionIncludePathsEnumerator(InteropSassOptions options)
            {
                _options = options;
                _index = -1;
                _current = default;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Dispose() { }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool MoveNext()
            {
                var len = Interop.sass_option_get_include_path_size(_options.DangerousGetHandle());
                var i = _index + 1;
                if (i < len)
                {
                    _index = i;
                    _current = Interop.ReadUtf8String(Interop.sass_option_get_include_path(_options.DangerousGetHandle(), i));
                    return true;
                }
                _index = len;
                _current = default;
                return false;
            }

            public void Reset()
            {
                _index = -1;
                _current = default;
            }
        }

        private readonly InteropSassOptions _options;

        public int Count
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Interop.sass_option_get_include_path_size(_options.DangerousGetHandle());
        }

        public string this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if (index < 0 || index >= Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }
                return Interop.ReadUtf8String(Interop.sass_option_get_include_path(_options.DangerousGetHandle(), index));
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public InteropSassOptionIncludePaths(InteropSassOptions options)
            => _options = options ?? throw new ArgumentNullException(nameof(options));

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();

        IEnumerator<string> IEnumerable<string>.GetEnumerator()
            => GetEnumerator();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(string includePath)
        {
            var options = _options;
            Interop.PassUtf8String(includePath, ptr => Interop.sass_option_push_include_path(options.DangerousGetHandle(), ptr));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public InteropSassOptionIncludePathsEnumerator GetEnumerator()
            => new InteropSassOptionIncludePathsEnumerator(_options);
    }
}