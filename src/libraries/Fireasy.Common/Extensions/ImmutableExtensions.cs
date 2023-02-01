// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using System.Collections.Immutable;

namespace Fireasy.Common.Extensions
{
    /// <summary>
    /// 不可变序列扩展。
    /// </summary>
    public static class ImmutableExtensions
    {
        /// <summary>
        /// 构造一个不可变的序列。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static ImmutableList<T> ToImmutableList<T>(this IEnumerable<T> source)
        {
            return ImmutableList.CreateRange<T>(source);
        }

        /// <summary>
        /// 构造一个不可变的数组。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static ImmutableArray<T> ToImmutableArray<T>(this IEnumerable<T> source)
        {
            return ImmutableArray.CreateRange<T>(source);
        }

        /// <summary>
        /// 构造一个不可变的字典。
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static ImmutableDictionary<TKey, TValue> ToImmutableDictionary<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> source) where TKey : notnull
        {
            return ImmutableDictionary.CreateRange<TKey, TValue>(source);
        }

        /// <summary>
        /// 构造一个不可变的哈希散列。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static ImmutableHashSet<T> ToImmutableHashSet<T>(this IEnumerable<T> source)
        {
            return ImmutableHashSet.CreateRange<T>(source);
        }
    }
}
