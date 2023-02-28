// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Microsoft.Extensions.DependencyInjection;

namespace Fireasy.Common.ObjectActivator
{
    /// <summary>
    /// 缺省的对象创建器。
    /// </summary>
    public class DefaultObjectActivator : IObjectActivator
    {
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// 初始化 <see cref="DefaultObjectActivator"/> 类的新实例。
        /// </summary>
        /// <param name="serviceProvider"></param>
        public DefaultObjectActivator(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// 创建类型 <typeparamref name="T"/> 的实例。
        /// </summary>
        /// <typeparam name="T">对象的类型。</typeparam>
        /// <param name="args">构造器参数，如果缺省则使用 <see cref="IServiceProvider.GetService(Type)"/> 来匹配构造函数的参数。</param>
        /// <returns></returns>
        public T? CreateInstance<T>(params object[] args)
        {
            if (CreateInstance(typeof(T), args) is T obj)
            {
                return obj;
            }

            return default;
        }

        /// <summary>
        /// 创建类型 <paramref name="type"/> 的实例。
        /// </summary>
        /// <param name="type">对象的类型。</param>
        /// <param name="args">构造器参数，如果缺省则使用 <see cref="IServiceProvider.GetService(Type)"/> 来匹配构造函数的参数。</param>
        /// <returns></returns>
        public object? CreateInstance(Type type, params object[] args)
        {
            Guard.ArgumentNull(type, nameof(type));

            return ActivatorUtilities.CreateInstance(_serviceProvider, type, args);
        }
    }
}
