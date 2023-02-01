﻿// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using System.Dynamic;

namespace Fireasy.Common.Dynamic
{
    /// <summary>
    /// 提供 <see cref="IDynamicMetaObjectProvider"/> 的动态扩展性。
    /// </summary>
    public interface IDynamicExpansibility
    {
        /// <summary>
        /// 获取动态对象中指定名称的属性值。
        /// </summary>
        /// <param name="dynamicProvider">一个动态对象。</param>
        /// <param name="name">属性的名称。</param>
        /// <returns></returns>
        object? GetMember(IDynamicMetaObjectProvider dynamicProvider, string name);

        /// <summary>
        /// 设置动态对象中指定名称的属性值。
        /// </summary>
        /// <param name="dynamicProvider">一个动态对象。</param>
        /// <param name="name">属性的名称。</param>
        /// <param name="value">设置值。</param>
        /// <returns></returns>
        void SetMember(IDynamicMetaObjectProvider dynamicProvider, string name, object value);

        /// <summary>
        /// 尝试获取动态对象中指定名称的属性值。
        /// </summary>
        /// <param name="dynamicProvider">一个动态对象。</param>
        /// <param name="name">属性的名称。</param>
        /// <param name="value">返回值。</param>
        /// <returns></returns>
        bool TryGetMember(IDynamicMetaObjectProvider dynamicProvider, string name, out object? value);

        /// <summary>
        /// 尝试设置动态对象中指定名称的属性值。
        /// </summary>
        /// <param name="dynamicProvider">一个动态对象。</param>
        /// <param name="name">属性的名称。</param>
        /// <param name="value">设置值。</param>
        /// <returns></returns>
        bool TrySetMember(IDynamicMetaObjectProvider dynamicProvider, string name, object value);
    }
}
