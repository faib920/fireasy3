// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Fireasy.Common.Collections
{
    public class Async2SyncEnumerable<T> : IEnumerable<T>
    {
        private readonly IAsyncEnumerator<T> _enumerator;

        public Async2SyncEnumerable(IAsyncEnumerator<T> enumerator)
        {
            _enumerator = enumerator;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new Async2SyncEnumerator(_enumerator);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private class Async2SyncEnumerator : IEnumerator<T>
        {
            private readonly IAsyncEnumerator<T> _enumerator;

            public Async2SyncEnumerator(IAsyncEnumerator<T> enumerator)
            {
                _enumerator = enumerator;
            }

            public T Current => _enumerator.Current;

            object? System.Collections.IEnumerator.Current => Current;

            void IDisposable.Dispose()
            {
                GC.SuppressFinalize(this);

                _enumerator.DisposeAsync().GetAwaiter();
            }

            public bool MoveNext()
            {
                var value = _enumerator.MoveNextAsync().ConfigureAwait(false).GetAwaiter().GetResult();
                return value;
            }

            void System.Collections.IEnumerator.Reset()
            {
            }
        }
    }
}
