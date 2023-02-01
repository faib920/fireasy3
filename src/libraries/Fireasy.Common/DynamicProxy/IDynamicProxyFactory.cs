// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Fireasy.Common.DynamicProxy
{
    /// <summary>
    /// 动态代理工厂。
    /// </summary>
    public interface IDynamicProxyFactory
    {
        /// <summary>
        /// 创建一个代理，将定义的拦截器注入到属性或方法内。
        /// </summary>
        /// <param name="objectType">用于创建代理类型的基类型。</param>
        /// <param name="args">创建对象的一组参数。</param>
        /// <returns></returns>
        object? BuildProxy(Type objectType, params object[] args);

        /// <summary>
        /// 获取新的代理类。
        /// </summary>
        /// <param name="objectType"></param>
        /// <returns></returns>
        Type GetProxyType(Type objectType);
    }
}
