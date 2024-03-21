using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace NCoreUtils.Sass.Internal;

public readonly struct InteropSassCompilerImportEntries : IReadOnlyList<SassImportEntry>
{
    [method: MethodImpl(MethodImplOptions.AggressiveInlining)]
    public struct InteropSassCompilerImportEntriesEnumerator(InteropSassCompiler compiler) : IEnumerator<SassImportEntry>
    {
        private readonly InteropSassCompiler _compiler = compiler;

        private int _index = -1;

        private SassImportEntry _current = default;

        readonly object IEnumerator.Current => Current;

        public readonly SassImportEntry Current
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _current;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly void Dispose() { }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext()
        {
            var len = Interop.sass_compiler_get_import_stack_size(_compiler.DangerousGetHandle());
            var i = _index + 1;
            if (i < len)
            {
                _index = i;
                _current = SassImportEntry.Create(Interop.sass_compiler_get_import_entry(_compiler.DangerousGetHandle(), i));
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

    private readonly InteropSassCompiler _compiler;

    public int Count
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Interop.sass_compiler_get_import_stack_size(_compiler.DangerousGetHandle());
    }

    public SassImportEntry this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            if (index < 0 || index >= Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }
            return UncheckedGetItem(index);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public InteropSassCompilerImportEntries(InteropSassCompiler options)
        => _compiler = options ?? throw new ArgumentNullException(nameof(options));

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();

    IEnumerator<SassImportEntry> IEnumerable<SassImportEntry>.GetEnumerator()
        => GetEnumerator();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private SassImportEntry UncheckedGetItem(int index)
        => SassImportEntry.Create(Interop.sass_compiler_get_import_entry(_compiler.DangerousGetHandle(), index));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public InteropSassCompilerImportEntriesEnumerator GetEnumerator()
        => new(_compiler);

    public SassImportEntry[] ToArray()
    {
        var count = Count;
        var result = new SassImportEntry[count];
        for (var i = 0; i < count; ++i)
        {
            result[i] = UncheckedGetItem(i);
        }
        return result;
    }
}