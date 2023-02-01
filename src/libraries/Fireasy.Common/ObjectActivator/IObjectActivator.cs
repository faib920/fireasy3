// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Fireasy.Common.ObjectActivator
{
    /// <summary>
    /// 提供对象创建接口。
    /// </summary>
    public interface IObjectActivator
    {
        /// <summary>
        /// 创建类型 <typeparamref name="T"/> 的实例。
        /// </summary>
        /// <typeparam name="T">对象的类型。</typeparam>
        /// <param name="args">构造器参数，如果缺省则使用 <see cref="IServiceProvider.GetService(Type)"/> 来匹配构造函数的参数。</param>
        /// <returns></returns>
        T? CreateInstance<T>(params object[] args);

        /// <summary>
        /// 创建类型 <paramref name="type"/> 的实例。
        /// </summary>
        /// <param name="type">对象的类型。</param>
        /// <param name="args">构造器参数，如果缺省则使用 <see cref="IServiceProvider.GetService(Type)"/> 来匹配构造函数的参数。</param>
        /// <returns></returns>
        object? CreateInstance(Type type, params object[] args);
    }
}
