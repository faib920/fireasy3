// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Fireasy.Common.Collections
{
    public class Sync2AsyncEnumerable<T> : IAsyncEnumerable<T>
    {
        private readonly IEnumerator<T> _enumerator;

        public Sync2AsyncEnumerable(IEnumerator<T> enumerator)
        {
            _enumerator = enumerator;
        }

        public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return new Sync2AsyncEnumerator(_enumerator);
        }

        private class Sync2AsyncEnumerator : IAsyncEnumerator<T>
        {
            private readonly IEnumerator<T> _enumerator;

            public Sync2AsyncEnumerator(IEnumerator<T> enumerator)
            {
                _enumerator = enumerator;
            }

            public T Current => _enumerator.Current;

            ValueTask IAsyncDisposable.DisposeAsync()
            {
                GC.SuppressFinalize(this);

                _enumerator.Dispose();

                return new ValueTask();
            }

            public ValueTask<bool> MoveNextAsync()
            {
                return new ValueTask<bool>(_enumerator.MoveNext());
            }
        }
    }
}
