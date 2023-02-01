// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Fireasy.Common.Reflection
{
    /// <summary>
    /// 字段的读写器。
    /// </summary>
    public interface IFieldAccessor
    {
        /// <summary>
        /// 获取给定对象的字段的值。
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        object? GetValue(object? instance);

        /// <summary>
        /// 设置给定对象的字段的值。
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="value"></param>
        void SetValue(object? instance, object? value);
    }

    /// <summary>
    /// 字段的读写器。
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    public interface IFieldAccessor<TValue>
    {
        /// <summary>
        /// 获取给定对象的字段的值。
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        TValue? GetValue(object? instance);

        /// <summary>
        /// 设置给定对象的字段的值。
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="value"></param>
        void SetValue(object? instance, TValue? value);
    }
}
