// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Fireasy.Data.Batcher;
using Fireasy.Data.Identity;
using Fireasy.Data.RecordWrapper;
using Fireasy.Data.Schema;
using Fireasy.Data.Syntax;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Fireasy.Data.Provider
{
    /// <summary>
    /// PostgreSql数据库提供者。使用 Npgsql 提供。
    /// </summary>
    public class PostgreSqlProvider : ProviderBase
    {
        /// <summary>
        /// 初始化 <see cref="PostgreSqlProvider"/> 类的新实例。
        /// </summary>
        /// <param name="serviceProvider"></param>
        public PostgreSqlProvider(IServiceProvider serviceProvider)
            : base(serviceProvider,
                  new InjectedProviderFactoryResolver<PostgreSqlProvider>(serviceProvider),
                  new AssemblyProviderFactoryResolver("Npgsql.NpgsqlFactory, Npgsql"))
        {
        }

        /// <summary>
        /// 获取提供者名称。
        /// </summary>
        public override string ProviderName => "PostgreSql";

        /// <summary>
        /// 获取当前连接的参数。
        /// </summary>
        /// <returns></returns>
        public override ConnectionParameter GetConnectionParameter(ConnectionString connectionString)
        {
            return new ConnectionParameter
            {
                Server = connectionString.Properties["server"],
                Database = connectionString.Properties["database"],
                UserId = connectionString.Properties.TryGetValue("userid", "user id"),
                Password = connectionString.Properties["password"]
            };
        }

        /// <summary>
        /// 使用参数更新指定的连接。
        /// </summary>
        /// <param name="connectionString">连接字符串对象。</param>
        /// <param name="parameter"></param>
        public override void UpdateConnectionString(ConnectionString connectionString, ConnectionParameter parameter)
        {
            connectionString.Properties.TrySetValue(parameter.Server, "server")
                .TrySetValue(parameter.Database, "database")
                .TrySetValue(parameter.UserId, "userid")
                .TrySetValue(parameter.Password, "password")
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
            services.TryAddSingleton<ISyntaxProvider, PostgreSqlSyntax>();
            services.TryAddSingleton<ISchemaProvider, PostgreSqlSchema>();
            services.TryAddSingleton<IBatcherProvider, MySqlBatcher>();
            services.TryAddSingleton<IRecordWrapper, GeneralRecordWrapper>();

            return services;
        }
    }
}
