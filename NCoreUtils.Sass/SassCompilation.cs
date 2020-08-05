using System;
using System.Threading;
using NCoreUtils.Sass.Internal;

namespace NCoreUtils.Sass
{
    public sealed class SassCompilation : IDisposable
    {
        private int _isDisposed;

        private InteropSassCompiler _compiler;

        public InteropSassContext Context { get; }

        internal SassCompilation(InteropSassContext context, InteropSassCompiler compiler)
        {
            Context = context;
            _compiler = compiler;
        }

        public SassResults Execute()
            => Context.GetResults(_compiler.Execute());

        public SassImportEntry[] GetImportEntries()
            => _compiler.Imports.ToArray();

        public void Dispose()
        {
            if (0 == Interlocked.CompareExchange(ref _isDisposed, 1, 0))
            {
                _compiler.Dispose();
                Context.Dispose();
            }
        }
    }
}