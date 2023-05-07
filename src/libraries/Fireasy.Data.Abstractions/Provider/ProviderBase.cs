// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Fireasy.Common.Collections;
using Fireasy.Common.DependencyInjection;
using Fireasy.Common.Extensions;
using Microsoft.Extensions.DependencyInjection;
using System.Data.Common;

namespace Fireasy.Data.Provider
{
    /// <summary>
    /// 基本的数据库提供者。
    /// </summary>
    public abstract class ProviderBase : IProvider
    {
        private DbProviderFactory? _factory;
        private readonly IProviderFactoryResolver[] _resolvers;
        private static ExtraConcurrentDictionary<Type, DbProviderFactory?> _cache = new ExtraConcurrentDictionary<Type, DbProviderFactory?>();

        /// <summary>
        /// 使用提供者名称初始化 <see cref="ProviderBase"/> 类的新实例。
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <param name="resolvers"></param>
        protected ProviderBase(IServiceProvider serviceProvider, params IProviderFactoryResolver[] resolvers)
        {
            ServiceProvider = serviceProvider;
            _resolvers = resolvers;
        }

        /// <summary>
        /// 获取描述数据库的名称。
        /// </summary>
        public virtual string ProviderName { get; set; }

        /// <summary>
        /// 获取数据库提供者工厂。
        /// </summary>
        public virtual DbProviderFactory DbProviderFactory
        {
            get
            {
                var factory = _cache.GetOrAdd(this.GetType(), InitDbProviderFactory);
                return factory ?? throw new NotSupportedException("未提供 DbProviderFactory。");
            }
        }

        /// <summary>
        /// 获取或设置应用服务提供者。
        /// </summary>
        public IServiceProvider ServiceProvider { get; set; }

        /// <summary>
        /// 获取当前连接的参数。
        /// </summary>
        /// <returns></returns>
        public abstract ConnectionParameter GetConnectionParameter(ConnectionString connectionString);

        /// <summary>
        /// 使用参数更新指定的连接。
        /// </summary>
        /// <param name="connectionString">连接字符串对象。</param>
        /// <param name="parameter"></param>
        public abstract void UpdateConnectionString(ConnectionString connectionString, ConnectionParameter parameter);

        /// <summary>
        /// 克隆一个副本。
        /// </summary>
        /// <returns></returns>
        public virtual IProvider Clone()
        {
            return (IProvider)MemberwiseClone();
        }

        /// <summary>
        /// 初始化。
        /// </summary>
        /// <param name="context">初始化上下文。</param>
        public virtual void Initialize(ProviderInitializeContext context)
        {
            AddFromCustomizer(context.Services);
        }

        /// <summary>
        /// 获取服务。
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <returns></returns>
        public TService? GetService<TService>() where TService : IProviderService
        {
            return ServiceProvider.TryGetService<TService>() ?? default;
        }

        /// <summary>
        /// 处理 <see cref="DbConnection"/> 对象。
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        public virtual DbConnection PrepareConnection(DbConnection connection)
        {
            return connection;
        }

        /// <summary>
        /// 处理 <see cref="DbCommand"/> 对象。
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public virtual DbCommand PrepareCommand(DbCommand command)
        {
            return command;
        }

        /// <summary>
        /// 处理 <see cref="DbParameter"/> 对象。
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public virtual DbParameter PrepareParameter(DbParameter parameter)
        {
            return parameter;
        }

        /// <summary>
        /// 处理事务级别。
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public virtual IsolationLevel AmendIsolationLevel(IsolationLevel level)
        {
            return level;
        }

        /// <summary>
        /// 初始化 <see cref="DbProviderFactory"/> 对象。
        /// </summary>
        /// <returns></returns>
        protected virtual DbProviderFactory? InitDbProviderFactory()
        {
            DbProviderFactory? factory = null;
            foreach (var resolver in _resolvers)
            {
                if (resolver.TryResolve(out factory))
                {
                    return factory;
                }
            }

            return null;
        }

        /// <summary>
        /// 从 <see cref="ProviderCustomizer"/> 中注册 <see cref="IProviderService"/> 插件服务。
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        protected IServiceCollection AddFromCustomizer(IServiceCollection services)
        {
            var accessor = ServiceProvider.GetService<IObjectAccessor<ProviderCustomizer>>();
            if (accessor != null && accessor.Value is ProviderCustomizer customizer)
            {
                foreach (var map in customizer.GetProviderServiceMappers(this.GetType()))
                {
                    services.AddSingleton(map.DefinedType, map.ServiceType);
                }
            }

            return services;
        }
    }
}
