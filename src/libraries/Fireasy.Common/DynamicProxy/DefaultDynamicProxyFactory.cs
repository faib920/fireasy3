// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Fireasy.Common.ObjectActivator;

namespace Fireasy.Common.DynamicProxy
{
    /// <summary>
    /// 缺省的动态代理工厂。
    /// </summary>
    public class DefaultDynamicProxyFactory : IDynamicProxyFactory
    {
        private readonly IObjectActivator _objectActivator;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objectActivator"></param>
        public DefaultDynamicProxyFactory(IObjectActivator objectActivator)
        {
            _objectActivator = objectActivator;
        }

        /// <summary>
        /// 创建一个代理，将定义的拦截器注入到属性或方法内。
        /// </summary>
        /// <typeparam name="T">用于创建代理类型的基类型。</typeparam>
        /// <param name="args">创建对象的一组参数。</param>
        /// <returns>类型 <typeparamref name="T"/> 的代理。</returns>
        public T? BuildProxy<T>(params object[] args) where T : class
        {
            return (T?)BuildProxy(typeof(T), args);
        }

        /// <summary>
        /// 创建一个代理，将定义的拦截器注入到属性或方法内。
        /// </summary>
        /// <param name="objectType">用于创建代理类型的基类型。</param>
        /// <param name="args">创建对象的一组参数。</param>
        /// <returns></returns>
        public object? BuildProxy(Type objectType, params object[] args)
        {
            var proxyType = GetProxyType(objectType);
            if (proxyType == null)
            {
                return null;
            }

            return _objectActivator.CreateInstance(proxyType, args);
        }

        /// <summary>
        /// 获取新的代理类。
        /// </summary>
        /// <param name="objectType"></param>
        /// <returns></returns>
        public Type? GetProxyType(Type objectType)
        {
            Guard.ArgumentNull(objectType, nameof(objectType));

            if (Container.TryGet(objectType, out var proxyType))
            {
                return proxyType;
            }

            return null;
        }
    }
}