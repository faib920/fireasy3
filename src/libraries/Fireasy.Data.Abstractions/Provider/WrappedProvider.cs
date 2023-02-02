// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Fireasy.Common.Collections;
using Fireasy.Common.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System.Data.Common;

namespace Fireasy.Data.Provider
{
    /// <summary>
    /// 包装一个现有的提供者，用于重新定义 <see cref="IProviderService"/> 服务集。
    /// </summary>
    public sealed class WrappedProvider : IProvider, IFeaturedProvider, IServiceProviderAccessor
    {
        private readonly IProvider _provider;
        private readonly ExtraConcurrentDictionary<Type, Lazy<IProviderService>> _services = new ExtraConcurrentDictionary<Type, Lazy<IProviderService>>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="provider"></param>
        public WrappedProvider(IProvider provider)
        {
            _provider = provider;
        }

        /// <summary>
        /// 获取内部的 <see cref="IProvider"/> 实例。
        /// </summary>
        public IProvider Provider => _provider;

        IServiceProvider IServiceProviderAccessor.ServiceProvider { get => _provider.ServiceProvider; set => _provider.ServiceProvider = value; }

        string IProvider.ProviderName { get => _provider.ProviderName; set => _provider.ProviderName = value; }

        DbProviderFactory IProvider.DbProviderFactory => _provider.DbProviderFactory;

        IsolationLevel IProvider.AmendIsolationLevel(IsolationLevel level) => _provider.AmendIsolationLevel(level);

        IProvider? IFeaturedProvider.Clone(string feature) => _provider is IFeaturedProvider featureProvider ? featureProvider.Clone(feature) : null;

        string? IFeaturedProvider.GetFeature(ConnectionString connectionString) => _provider is IFeaturedProvider featureProvider ? featureProvider.GetFeature(connectionString) : null;

        IServiceCollection IProvider.RegisterServices(IServiceCollection services) => _provider.RegisterServices(services);

        TService IProvider.GetService<TService>() => _provider.GetService<TService>();

        ConnectionParameter IProvider.GetConnectionParameter(ConnectionString connectionString) => _provider.GetConnectionParameter(connectionString);

        void IProvider.UpdateConnectionString(ConnectionString connectionString, ConnectionParameter parameter)
        {
            _provider.UpdateConnectionString(connectionString, parameter);
        }

        DbCommand IProvider.PrepareCommand(DbCommand command) => _provider.PrepareCommand(command);

        DbConnection IProvider.PrepareConnection(DbConnection connection) => _provider.PrepareConnection(connection);

        DbParameter IProvider.PrepareParameter(DbParameter parameter) => _provider.PrepareParameter(parameter);
    }
}
