// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Fireasy.Common.Collections;

namespace Fireasy.Common.Extensions
{
    /// <summary>
    /// <see cref="IAsyncEnumerable{T}"/> 扩展方法。
    /// </summary>
    public static class AsyncEnumerableExtensions
    {
        /// <summary>
        /// 将 <see cref="IAsyncEnumerable{T}"/> 序列转换为 <see cref="IEnumerable{T}"/> 序列。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<T>> AsEnumerable<T>(this IAsyncEnumerable<T> source, CancellationToken cancellationToken = default)
        {
            await using var enumerator = source.GetAsyncEnumerator(cancellationToken);
            return new Async2SyncEnumerable<T>(enumerator);
        }

        /// <summary>
        /// 将 <see cref="IAsyncEnumerable{T}"/> 序列转换为 <see cref="List{T}"/> 对象。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<List<T>> ToListAsync<T>(this IAsyncEnumerable<T> source, CancellationToken cancellationToken = default)
        {
            var result = new List<T>();
            await foreach (var item in source.WithCancellation(cancellationToken).ConfigureAwait(false))
            {
                result.Add(item);
            }

            return result;
        }

        /// <summary>
        /// 返回 <see cref="IAsyncEnumerable{T}"/> 序列中的第一个对象。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<T?> FirstOrDefaultAsync<T>(this IAsyncEnumerable<T> source, CancellationToken cancellationToken = default)
        {
            await foreach (var item in source.WithCancellation(cancellationToken).ConfigureAwait(false))
            {
                return item;
            }

            return default(T);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static IAsyncEnumerable<T> AsAsyncEnumerable<T>(this IEnumerable<T> source)
        {
            using var enumerator = source.GetEnumerator();
            return new Sync2AsyncEnumerable<T>(enumerator);
        }
    }
}
