// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Fireasy.Common.DependencyInjection
{
    /// <summary>
    /// 对象访问器接口，可以以注入的方式来访问对象。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IObjectAccessor<out T>
    {
        /// <summary>
        /// 获取实例。
        /// </summary>
        T? Value { get; }
    }
}
