// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Fireasy.Data.Identity;
using Fireasy.Data.RecordWrapper;
using Fireasy.Data.Schema;
using Fireasy.Data.Syntax;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Fireasy.Data.Provider
{
    /// <summary>
    /// Dameng(达梦)数据库提供者。
    /// </summary>
    public class DamengProvider : ProviderBase
    {
        /// <summary>
        /// 初始化 <see cref="DamengProvider"/> 类的新实例。
        /// </summary>
        /// <param name="serviceProvider"></param>
        public DamengProvider(IServiceProvider serviceProvider)
            : base(serviceProvider,
                  new InjectedProviderFactoryResolver<DamengProvider>(serviceProvider),
                  new AssemblyProviderFactoryResolver("Dm.DmClientFactory, DmProvider"))
        {
        }

        /// <summary>
        /// 获取提供者名称。
        /// </summary>
        public override string ProviderName => "Dameng";

        /// <summary>
        /// 获取当前连接的参数。
        /// </summary>
        /// <returns></returns>
        public override ConnectionParameter GetConnectionParameter(ConnectionString connectionString)
        {
            return new ConnectionParameter
            {
                Server = connectionString.Properties.TryGetValue("data source", "server", "host"),
                Database = connectionString.Properties.TryGetValue("database", "initial catalog"),
                UserId = connectionString.Properties.TryGetValue("user id", "userid"),
                Password = connectionString.Properties.TryGetValue("password", "pwd")
            };
        }

        /// <summary>
        /// 使用参数更新指定的连接。
        /// </summary>
        /// <param name="connectionString">连接字符串对象。</param>
        /// <param name="parameter"></param>
        public override void UpdateConnectionString(ConnectionString connectionString, ConnectionParameter parameter)
        {
            connectionString.Properties.TrySetValue(parameter.Server, "data source", "server", "host")
                .TrySetValue(parameter.Database, "database", "initial catalog")
                .TrySetValue(parameter.UserId, "user id", "userid")
                .TrySetValue(parameter.Password, "password", "pwd")
                .Update();
        }

        /// <summary>
        /// 注册服务。
        /// </summary>
        /// <param name="services"></param>
        public override IServiceCollection RegisterServices(IServiceCollection services)
        {
            services = base.RegisterServices(services);
            services.TryAddSingleton<IGeneratorProvider, BaseSequenceGenerator>();
            services.TryAddSingleton<ISyntaxProvider, DamengSyntax>();
            services.TryAddSingleton<ISchemaProvider, DamengSchema>();
            services.TryAddSingleton<IRecordWrapper, GeneralRecordWrapper>();

            return services;
        }
    }
}
