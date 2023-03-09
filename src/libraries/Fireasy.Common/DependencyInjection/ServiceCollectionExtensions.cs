// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Fireasy.Common;
using Fireasy.Common.Compiler;
using Fireasy.Common.DependencyInjection;
using Fireasy.Common.Dynamic;
using Fireasy.Common.DynamicProxy;
using Fireasy.Common.ObjectActivator;
using Fireasy.Common.Reflection;
using Fireasy.Common.Serialization;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// <see cref="IServiceCollection"/> 为 fireasy 提供的扩展类。
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        private readonly static object _locker = new object();

        /// <summary>
        /// 添加框架的基本支持。
        /// </summary>
        /// <param name="services"><see cref="IServiceCollection"/> 实例。</param>
        /// <param name="configure">配置方法。</param>
        /// <returns></returns>
        public static SetupBuilder AddFireasy(this IServiceCollection services, Action<SetupOptions>? configure = null)
        {
            services.AddSingleton<IDynamicExpansibility, DefaultDynamicExpansibility>();
            services.AddSingleton<IObjectActivator, DefaultObjectActivator>();
            services.AddSingleton<IDynamicProxyFactory, DefaultDynamicProxyFactory>();
            services.AddSingleton<IReflectionFactory, DefaultReflectionFactory>();
            services.AddSingleton<IJsonSerializer, DefaultJsonSerializer>();
#if !NET5_0_OR_GREATER
            services.AddSingleton<IBinarySerializer, BinaryCompressSerializer>();
#endif
            services.AddSingleton<DynamicDescriptionSupporter>(sp => new DynamicDescriptionSupporter(sp));
            services.AddSingleton<ICodeCompilerManager>(new DefaultCodeCompilerManager());

            var options = new SetupOptions();
            configure?.Invoke(options);

            var builder = new SetupBuilder(services, options);

            var discoverer = options.DiscoverOptions.DiscovererFactory == null ? new DefaultServiceDiscoverer(services, options.DiscoverOptions)
                : options.DiscoverOptions.DiscovererFactory(services, options.DiscoverOptions);

            if (discoverer != null)
            {
                services.AddSingleton<IServiceDiscoverer>(discoverer);
            }

            return builder;
        }

        /// <summary>
        /// 添加一个类型为 <typeparamref name="T"/> 的对象访问者。如果已经注册过访问者，则返回之前注册的实例。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="services"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static IObjectAccessor<T> AddObjectAccessor<T>(this IServiceCollection services, T? obj = default)
        {
            lock (_locker)
            {
                ServiceDescriptor? descriptor;
                if ((descriptor = services.FirstOrDefault(s => s.ServiceType == typeof(IObjectAccessor<T>))) != null && descriptor.ImplementationInstance != null)
                {
                    return (IObjectAccessor<T>)descriptor.ImplementationInstance;
                }

                var accessor = new DefaultObjectAccessor<T>(obj);
                services.Insert(0, ServiceDescriptor.Singleton(typeof(IObjectAccessor<T>), accessor));

                return accessor;
            }
        }

        /// <summary>
        /// 从 <paramref name="services"/> 中获取 <see cref="IObjectAccessor{T}"/> 的值，如果不存在则实例化一个对象，并添加到 <paramref name="services"/> 中。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static T GetOrAddObjectAccessor<T>(this IServiceCollection services) where T : class, new()
        {
            lock (_locker)
            {
                ServiceDescriptor? descriptor;
                if ((descriptor = services.FirstOrDefault(s => s.ServiceType == typeof(IObjectAccessor<T>))) != null && descriptor.ImplementationInstance != null)
                {
                    return ((IObjectAccessor<T>)descriptor.ImplementationInstance).Value!;
                }

                var obj = new T();
                var accessor = new DefaultObjectAccessor<T>(obj);
                services.Insert(0, ServiceDescriptor.Singleton(typeof(IObjectAccessor<T>), accessor));

                return obj;
            }
        }

        /// <summary>
        /// 从 <see cref="IServiceCollection"/> 里查找 <typeparamref name="TService"/> 单例实例。
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static TService? GetSingletonInstance<TService>(this IServiceCollection services) where TService : class
        {
            var descriptor = services.FirstOrDefault(s => s.ServiceType == typeof(TService));
            if (descriptor != null && descriptor.Lifetime == ServiceLifetime.Singleton)
            {
                if (descriptor.ImplementationInstance is TService instance)
                {
                    return instance;
                }
                if (descriptor.ImplementationFactory != null)
                {
                    return descriptor.ImplementationFactory(null) as TService;
                }
            }

            return default;
        }

    }
}
