// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Fireasy.Common.Extensions;
using Fireasy.Common.ObjectActivator;
using Fireasy.Configuration;
using Fireasy.Data.Configuration;
using Fireasy.Data.Provider;
using Microsoft.Extensions.DependencyInjection;

namespace Fireasy.Data
{
    /// <summary>
    /// <see cref="IDatabase"/> 的缺省工厂。
    /// </summary>
    public class DefaultDatabaseFactory : IDatabaseFactory
    {
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// 初始化 <see cref="DefaultDatabaseFactory"/> 类的新实例。
        /// </summary>
        /// <param name="serviceProvider"></param>
        public DefaultDatabaseFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// 使用配置实例创建 <see cref="IDatabase"/> 实例。
        /// </summary>
        /// <param name="instanceName">配置实例名称。</param>
        /// <returns></returns>
        public IDatabase? CreateDatabase(string? instanceName = null)
        {
            var unity = _serviceProvider.GetRequiredService<ConfigurationUnity>();
            var providerManager = _serviceProvider.GetRequiredService<IProviderManager>();
            var section = unity.GetSection<InstanceConfigurationSection>();
            if (section == null || (string.IsNullOrEmpty(instanceName) && section.Settings.Count == 0))
            {
                throw new InvalidOperationException("未发现 fireasy:dataInstances 的配置。");
            }

            var setting = string.IsNullOrEmpty(instanceName) ? section.GetDefault() : section.Settings[instanceName!];
            if (setting is not IInstanceConfigurationSetting instanceSetting)
            {
                throw new InvalidOperationException($"在配置 fireasy:dataInstances 中未找到 {instanceName} 的配置项。");
            }

            var provider = providerManager.GetDefinedProvider(instanceSetting);
            if (provider == null)
            {
                throw new InvalidOperationException($"未查找到提供者 {instanceSetting.ProviderType}，如果是自定义提供者，请在 fireasy:dataProviders 里进行配置。");
            }

            if (instanceSetting.Clusters.Count > 0)
            {
                var distConns = GetDistributedConnections(instanceSetting);
                if (distConns?.Count > 0)
                {
                    return CreateDatabase(distConns, provider);
                }
            }

            return CreateDatabase(instanceSetting.ConnectionString, provider);
        }

        /// <summary>
        /// 使用提供者类型 <typeparamref name="TProvider"/> 及连接串创建 <see cref="IDatabase"/> 实例。
        /// </summary>
        /// <typeparam name="TProvider"></typeparam>
        /// <param name="connectionString">数据库连接字符串。</param>
        /// <returns></returns>
        public IDatabase CreateDatabase<TProvider>(string connectionString) where TProvider : IProvider
        {
            Guard.ArgumentNull(connectionString, nameof(connectionString));

            var objActivator = _serviceProvider.GetRequiredService<IObjectActivator>();
            var provider = objActivator.CreateInstance(typeof(TProvider), _serviceProvider);
            if (provider is TProvider _provider)
            {
                return CreateDatabase(connectionString, _provider);
            }

            throw new NotSupportedException("未指定数据库提供者。");
        }

        /// <summary>
        /// 使用提供者名称及连接串创建 <see cref="IDatabase"/> 实例。
        /// </summary>
        /// <param name="providerName">提供者名称，可通过 <see cref="IProviderManager.GetSupportedProviderNames"/> 获取。</param>
        /// <param name="connectionString">数据库连接字符串。</param>
        /// <returns></returns>
        public IDatabase CreateDatabase(string providerName, string connectionString)
        {
            Guard.ArgumentNull(providerName, nameof(providerName));
            Guard.ArgumentNull(connectionString, nameof(connectionString));

            var providerManager = _serviceProvider.GetRequiredService<IProviderManager>();
            var provider = providerManager.GetDefinedProvider(providerName);
            if (provider != null)
            {
                return CreateDatabase(connectionString, provider);
            }

            throw new NotSupportedException($"未找到数据库提供者 {providerName}。");
        }

        /// <summary>
        /// 注册内部服务。
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        protected virtual IServiceCollection RegisterInternalServices(IServiceCollection services)
        {
            return services;
        }

        /// <summary>
        /// 通过一组分布式连接串创建一个 <see cref="IDatabase"/> 实例。
        /// </summary>
        /// <param name="distConns">分布式连接字符串集合。</param>
        /// <param name="provider">提供者实例。</param>
        /// <returns></returns>
        protected virtual IDatabase CreateDatabase(List<DistributedConnectionString> distConns, IProvider provider)
        {
            var master = distConns.First(s => s.Mode == DistributedMode.Master);
            var _ = CreateServicePrivoder(master, provider);

            return new InterceptedDatabase(distConns, provider);
        }

        /// <summary>
        /// 通过一个 <paramref name="connectionString"/> 创建一个 <see cref="IDatabase"/> 实例。
        /// </summary>
        /// <param name="connectionString">连接字符串。</param>
        /// <param name="provider">提供者实例。</param>
        /// <returns></returns>
        protected virtual IDatabase CreateDatabase(ConnectionString connectionString, IProvider provider)
        {
            var _ = CreateServicePrivoder(connectionString, provider);

            return new InterceptedDatabase(connectionString, provider);
        }

        /// <summary>
        /// 创建一个 <see cref="IServiceProvider"/> 实例。
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="provider"></param>
        /// <returns></returns>
        protected virtual IServiceProvider CreateServicePrivoder(ConnectionString connectionString, IProvider provider)
        {
            IServiceCollection services = new ServiceCollection();
            services = RegisterInternalServices(services).AddSingleton<IProvider>(provider);
            provider.Initialize(new ProviderInitializeContext(services, connectionString));
            var serviceProvider = services.BuildServiceProvider();
            var scope = _serviceProvider.CreateScope();
            var internalSp = new InternalServiceProvider(scope, serviceProvider);

            provider.TrySetServiceProvider(internalSp);
            return internalSp;
        }

        /// <summary>
        /// 获取分布式的数据库连接串。
        /// </summary>
        /// <param name="setting">数据库实例配置。</param>
        /// <returns></returns>
        private static List<DistributedConnectionString>? GetDistributedConnections(IInstanceConfigurationSetting setting)
        {
            if (setting.Clusters?.Count > 0)
            {
                var connections = new List<DistributedConnectionString>();
                foreach (var cluster in setting.Clusters)
                {
                    connections.Add(new DistributedConnectionString(cluster.ConnectionString)
                    {
                        Mode = cluster.Mode,
                        Weight = cluster.Weight
                    });
                }

                return connections;
            }

            return null;
        }
    }
}
