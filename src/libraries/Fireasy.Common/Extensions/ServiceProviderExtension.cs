﻿// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Fireasy.Common.DependencyInjection;

namespace Fireasy.Common.Extensions
{
    /// <summary>
    /// <see cref="IServiceProvider"/> 接口的扩展。
    /// </summary>
    public static class ServiceProviderExtension
    {
        /// <summary>
        /// 尝试从 <see cref="IServiceProvider"/> 实例中获取指定的服务，如果没有则使用 <paramref name="creator"/> 函数创建一个服务实例。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serviceProvier"></param>
        /// <param name="creator"></param>
        /// <returns></returns>
        public static T? TryGetService<T>(this IServiceProvider serviceProvier, Func<T>? creator = null)
        {
            return (T?)(serviceProvier?.GetService(typeof(T)) ?? (creator == null ? default : creator()));
        }

        /// <summary>
        /// 尝试从 <see cref="IServiceProvider"/> 实例中获取指定的服务，如果没有则使用 <paramref name="creator"/> 函数创建一个服务实例。
        /// </summary>
        /// <param name="serviceProvier"></param>
        /// <param name="serviceType"></param>
        /// <param name="creator"></param>
        /// <returns></returns>
        public static object? TryGetService(this IServiceProvider serviceProvier, Type serviceType, Func<object>? creator = null)
        {
            return serviceProvier?.GetService(serviceType) ?? (creator == null ? default : creator());
        }

        /// <summary>
        /// 如果对象实现了 <see cref="IServiceProviderAccessor"/>，则尝试获取 <see cref="IServiceProvider"/> 实例。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static IServiceProvider? TryGetServiceProvider<T>(this T obj)
        {
            if (obj is IServiceProviderAccessor accessor)
            {
                return accessor.ServiceProvider;
            }

            return null;
        }

        /// <summary>
        /// 如果对象实现了 <see cref="IServiceProviderAccessor"/>，则尝试对实例附加 <see cref="IServiceProvider"/> 实例。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="serviceProvider"></param>
        /// <returns></returns>
        public static T TrySetServiceProvider<T>(this T obj, IServiceProvider serviceProvider)
        {
            if (obj == null || serviceProvider == null)
            {
                return obj;
            }

            if (obj is IServiceProviderAccessor accessor && accessor.ServiceProvider != serviceProvider)
            {
                accessor.ServiceProvider = serviceProvider;
            }

            return obj;
        }
    }
}
