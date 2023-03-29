// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Fireasy.Common.Extensions;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;

namespace Fireasy.Common.DependencyInjection
{
    /// <summary>
    /// 默认的服务发现者实现。
    /// </summary>
    public class DefaultServiceDiscoverer : IServiceDiscoverer
    {
        private readonly DiscoverOptions _options;
        private readonly List<Assembly> _assemblies = new();
        private readonly List<ServiceDescriptor> _descriptors = new();

        /// <summary>
        /// 获取被发现的 <see cref="Assembly"/> 列表。
        /// </summary>
        ReadOnlyCollection<Assembly> IServiceDiscoverer.Assemblies
        {
            get => new ReadOnlyCollection<Assembly>(_assemblies);
        }

        /// <summary>
        /// 获取注册的 <see cref="ServiceDescriptor"/> 集合。
        /// </summary>
        ReadOnlyCollection<ServiceDescriptor> IServiceDiscoverer.Descriptors
        {
            get => new ReadOnlyCollection<ServiceDescriptor>(_descriptors);
        }

        /// <summary>
        /// 初始化 <see cref="DefaultServiceDiscoverer"/> 类的新实例。
        /// </summary>
        /// <param name="services"></param>
        /// <param name="options"></param>
        public DefaultServiceDiscoverer(IServiceCollection services, DiscoverOptions options)
        {
            _options = options;

            DiscoverServices(services);
        }

        /// <summary>
        /// 获取可发现的 <see cref="Assembly"/> 序列。
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerable<Assembly> GetAssemblies()
        {
            return Directory.GetFiles(Directory.GetCurrentDirectory(), "*.dll").Select(s => Assembly.LoadFrom(s));
        }

        /// <summary>
        /// 发现工作目录中所有程序集中的依赖类型。
        /// </summary>
        /// <param name="services"></param>
        private void DiscoverServices(IServiceCollection services)
        {
            foreach (var assembly in GetAssemblies())
            {
                if (_options?.AssemblyFilters?.Any(s => s.IsFilter(assembly)) == true)
                {
                    continue;
                }

                if (_options?.AssemblyFilterPredicates?.Any(s => s(assembly)) == true)
                {
                    continue;
                }

                _assemblies.Add(assembly);

                ConfigureServices(services, assembly);

                if (_options?.UseAnalyzers == false)
                {
                    DiscoverServices(services, assembly);
                }
            }
        }

        /// <summary>
        /// 发现程序集中的所有依赖类型。
        /// </summary>
        /// <param name="services"></param>
        /// <param name="assembly"></param>
        private void DiscoverServices(IServiceCollection services, Assembly assembly)
        {
            foreach (var type in assembly.GetExportedTypes())
            {
                if (_options?.TypeFilters?.Any(s => s.IsFilter(assembly, type)) == true)
                {
                    continue;
                }

                if (_options?.TypeFilterPredicates?.Any(s => s(assembly, type)) == true)
                {
                    continue;
                }

                ServiceLifetime? lifetime;
                var interfaceTypes = type.GetDirectImplementInterfaces().ToArray();

                //如果使用标注
                if (type.IsDefined(typeof(ServiceRegisterAttribute)))
                {
                    lifetime = type.GetCustomAttribute<ServiceRegisterAttribute>()!.Lifetime;
                }
                else
                {
                    lifetime = GetLifetimeFromType(type);
                }

                if (lifetime == null)
                {
                    continue;
                }

                if (interfaceTypes.Length > 0)
                {
                    interfaceTypes.ForEach(s => AddService(services, s, type, (ServiceLifetime)lifetime));
                }
                else
                {
                    AddService(services, type, type, (ServiceLifetime)lifetime);
                }
            }
        }

        private void ConfigureServices(IServiceCollection services, Assembly assembly)
        {
            var attrs = assembly.GetCustomAttributes<ServicesDeployAttribute>();
            if (attrs.Any())
            {
                foreach (var attr in attrs.OrderBy(s => s.Priority))
                {
                    if (Activator.CreateInstance(attr.Type) is IServicesDeployer deployer)
                    {
                        deployer.Configure(services);
                    }
                }
            }
            else
            {
                var types = assembly.GetTypes().Where(t => t.IsClass && !t.IsAbstract && typeof(IServicesDeployer).IsAssignableFrom(t)).ToList();

                var deployers = types
                    .Select(s => Activator.CreateInstance(s))
                    .Where(s => s is IServicesDeployer)
                    .Cast<IServicesDeployer>()
                    .ToList();

                deployers.ForEach(s => s!.Configure(services));
            }
        }

        private ServiceLifetime? GetLifetimeFromType(Type type)
        {
            if (typeof(ISingletonService).IsAssignableFrom(type))
            {
                return ServiceLifetime.Singleton;
            }
            else if (typeof(ITransientService).IsAssignableFrom(type))
            {
                return ServiceLifetime.Transient;
            }
            else if (typeof(IScopedService).IsAssignableFrom(type))
            {
                return ServiceLifetime.Scoped;
            }

            return null;
        }

        private ServiceDescriptor AddService(IServiceCollection services, Type serviceType, Type implType, ServiceLifetime lifetime)
        {
            var descriptor = ServiceDescriptor.Describe(serviceType, implType, lifetime);
            _descriptors.Add(descriptor);
            services.Add(descriptor);
            return descriptor;
        }
    }
}