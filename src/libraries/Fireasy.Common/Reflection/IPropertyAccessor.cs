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
    /// 属性的读写器。
    /// </summary>
    public interface IPropertyAccessor
    {
        /// <summary>
        /// 获取给定对象的属性的值。
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        object? GetValue(object? instance);

        /// <summary>
        /// 设置给定对象的属性的值。
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="value"></param>
        void SetValue(object? instance, object? value);
    }

    /// <summary>
    /// 属性的读写器。
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    public interface IPropertyAccessor<TValue>
    {
        /// <summary>
        /// 获取给定对象的属性的值。
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        TValue? GetValue(object? instance);

        /// <summary>
        /// 设置给定对象的属性的值。
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="value"></param>
        void SetValue(object? instance, TValue? value);
    }
}
