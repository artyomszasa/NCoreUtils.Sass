using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.Extensions.Primitives;

namespace NCoreUtils.Sass.Internal;

internal readonly struct SegmentRange(int position, int length)
{
    public int Position { get; } = position;

    public int Length { get; } = length;

    public void Deconstruct(out int position, out int length)
    {
        position = Position;
        length = Length;
    }
}

public readonly struct GenericPath : IEquatable<GenericPath>, IReadOnlyList<StringSegment>
{
    public struct Enumerator : IEnumerator<StringSegment>
    {
        private readonly string _source;

        private readonly SegmentRange[] _segmentData;

        private int _index;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal Enumerator(string source, SegmentRange[] segmentData)
        {
            _source = source;
            _segmentData = segmentData;
            _index = -1;
        }

        readonly object IEnumerator.Current => Current;

        public StringSegment Current { get; private set; } = default;

        public readonly void Dispose() { }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext()
        {
            var index = _index + 1;
            if (index < _segmentData.Length)
            {
                _index = index;
                Current = new StringSegment(_source, _segmentData[_index].Position, _segmentData[_index].Length);
                return true;
            }
            Current = default;
            return false;
        }

        public void Reset()
        {
            _index = -1;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsDelimiter(char ch)
        => ch == '/' || ch == '\\';

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static (SegmentRange[] segments, bool isAbsolute) GetSegments(string source)
    {
        var isAbsolute = false;
        var input = source.AsSpan();
        var counter = 0;
        foreach (var ch in input)
        {
            if (IsDelimiter(ch))
            {
                ++counter;
            }
        }
        var segments = new ArrayBuilder<SegmentRange>(counter + 1);
        var s = 0;
        bool first = true;
        for (var i = 0; i < input.Length; ++i)
        {
            var ch = input[i];
            if (first)
            {
                first = false;
                if (IsDelimiter(ch))
                {
                    isAbsolute = true;
                    s = i + 1;
                }
            }
            else if (IsDelimiter(ch))
            {
                // skip empty segments
                if (s != i)
                {
                    segments.Add(new(s, i - s));
                    s = i + 1;
                }
            }
        }
        // handle last segment
        if (s < input.Length)
        {
            segments.Add(new (s, input.Length - s));
        }
        return (segments.ToArray(), isAbsolute);
    }

    private readonly string _source;

    private readonly SegmentRange[] _segmentData;

    private readonly bool _isAbsolutePath;

    public bool IsEmpty
    {
        // NOTE: if source is not null then segmentData is neither null.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => string.IsNullOrEmpty(_source) || _segmentData.Length == 0;
    }

    public string Raw
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _source ?? string.Empty;
    }

    public int Count
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _segmentData?.Length ?? 0;
    }

    public StringSegment this[int index]
    {
        get
        {
            if (IsEmpty)
            {
                throw new IndexOutOfRangeException();
            }
            var (position, length) = _segmentData[index];
            return new StringSegment(_source, position, length);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public GenericPath(string source)
    {
        _source = source ?? throw new ArgumentNullException(nameof(source));
        (_segmentData, _isAbsolutePath) = GetSegments(source);
    }

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();

    IEnumerator<StringSegment> IEnumerable<StringSegment>.GetEnumerator()
        => GetEnumerator();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(GenericPath other)
    {
        if (IsEmpty)
        {
            return other.IsEmpty;
        }
        if (other.IsEmpty)
        {
            return false;
        }
        var ae = GetEnumerator();
        var be = other.GetEnumerator();
        while (ae.MoveNext())
        {
            if (!be.MoveNext())
            {
                return false;
            }
            if (ae.Current != be.Current)
            {
                return false;
            }
        }
        return !be.MoveNext();
    }

    public override bool Equals(object? obj)
        => obj is GenericPath other && Equals(other);

    public override int GetHashCode()
    {
        if (IsEmpty)
        {
            return 0;
        }
        var hashCode = new HashCode();
        foreach (var segment in this)
        {
            hashCode.Add(segment);
        }
        return hashCode.ToHashCode();
    }

    public Enumerator GetEnumerator()
        => new(_source, _segmentData ?? []);

    public string ToString(char separator)
    {
        if (IsEmpty)
        {
            return string.Empty;
        }
        var dataSize = _isAbsolutePath ? 1 : 0;
        var counter = 0;
        foreach (var (_, len) in _segmentData)
        {
            dataSize += len;
            ++counter;
        }
        var bufferSize = dataSize + counter - 1;
        if (bufferSize < 4096)
        {
            // build on stack
            Span<char> buffer = stackalloc char[bufferSize];
            var builder = new SpanBuilder(buffer);
            var data = _source.AsSpan();
            var first = true;
            foreach (var (pos, len) in _segmentData)
            {
                if (first)
                {
                    first = false;
                    if (_isAbsolutePath)
                    {
                        builder.Append(separator);
                    }
                }
                else
                {
                    builder.Append(separator);
                }
                builder.Append(data.Slice(pos, len));
            }
            return builder.ToString();
        }
        {
            // build using StringBuilder
            var builder = new StringBuilder(bufferSize);
            var data = _source.AsSpan();
            var first = true;
            foreach (var (pos, len) in _segmentData)
            {
                if (first)
                {
                    first = false;
                    if (_isAbsolutePath)
                    {
                        builder.Append(separator);
                    }
                }
                else
                {
                    builder.Append(separator);
                }
#if NETSTANDARD2_1 || NET8_0_OR_GREATER
                builder.Append(data.Slice(pos, len));
#else
                builder.Append(data.Slice(pos, len).ToString());
#endif
            }
            return builder.ToString();
        }
    }

    public override string ToString()
        => ToString(System.IO.Path.DirectorySeparatorChar);
}