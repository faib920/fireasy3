// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Fireasy.Common.Collections
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AsyncEmptyEnumerable<T> : IAsyncEnumerable<T>
    {
        public static AsyncEmptyEnumerable<T> Empty = new AsyncEmptyEnumerable<T>();

        IAsyncEnumerator<T> IAsyncEnumerable<T>.GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return new AsyncEmptyEnumerator();
        }

        private class AsyncEmptyEnumerator : IAsyncEnumerator<T>
        {
            public T Current => throw new NotImplementedException();

            ValueTask IAsyncDisposable.DisposeAsync()
            {
                return new ValueTask();
            }

            ValueTask<bool> IAsyncEnumerator<T>.MoveNextAsync()
            {
                return new ValueTask<bool>(false);
            }
        }
    }
}
