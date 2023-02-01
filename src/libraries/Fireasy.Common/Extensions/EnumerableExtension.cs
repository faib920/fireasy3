// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Fireasy.Common.Extensions
{
    /// <summary>
    /// 可枚举序列扩展方法。
    /// </summary>
    public static class EnumerableExtension
    {
        /// <summary>
        /// 枚举序列中的所有元素，并执行指定的方法。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="action"></param>
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            Guard.ArgumentNull(source, nameof(source));
            Guard.ArgumentNull(action, nameof(action));

            foreach (var item in source)
            {
                action(item);
            }
        }

        /// <summary>
        /// 枚举序列中的所有元素，并执行指定的方法（方法中带索引参数）。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="action"></param>
        public static void ForEach<T>(this IEnumerable<T> source, Action<T, int> action)
        {
            Guard.ArgumentNull(source, nameof(source));
            Guard.ArgumentNull(action, nameof(action));

            var index = 0;
            foreach (var item in source)
            {
                action(item, index++);
            }
        }

        /// <summary>
        /// 将一个序列拆成多个序列。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="splitSize">拆分大小。</param>
        /// <param name="splitMode">序列拆分方式。</param>
        /// <returns></returns>
        public static IEnumerable<IEnumerable<T>> Split<T>(this IEnumerable<T> source, int splitSize = 500, SequenceSplitMode splitMode = SequenceSplitMode.Normal)
        {
            var length = source.Count();

            var remanent = length;
            var done = 0;

            while (true)
            {
                if (remanent <= 0)
                {
                    break;
                }

                if (splitMode == SequenceSplitMode.Equationally && remanent > splitSize && remanent < splitSize * 2)
                {
                    splitSize = remanent / 2;
                    if (remanent % 2 != 0)
                    {
                        splitSize++;
                    }
                }

                var splitList = source.Skip(done).Take(splitSize).ToArray();
                if (splitList.Length == 0)
                {
                    break;
                }

                done += splitList.Length;
                remanent -= splitList.Length;
                yield return splitList;
            }
        }

        /// <summary>
        /// 将一个 <see cref="IEnumerable"/> 枚举成泛型 <typeparamref name="T"/> 的枚举。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable"></param>
        /// <returns></returns>
        public static IEnumerable<T> AsEnumerable<T>(this IEnumerable enumerable)
        {
            foreach (var item in enumerable)
            {
                yield return (T)item;
            }
        }
    }
}
