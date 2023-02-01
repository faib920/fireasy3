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
using System;

namespace Fireasy.Data.Provider
{
    /// <summary>
    /// Firebird数据库提供者。使用 FirebirdSql.Data.FirebirdClient 提供。
    /// </summary>
    public class FirebirdProvider : ProviderBase
    {
        /// <summary>
        /// 初始化 <see cref="PostgreSqlProvider"/> 类的新实例。
        /// </summary>
        /// <param name="serviceProvider"></param>
        public FirebirdProvider(IServiceProvider serviceProvider)
            : base(serviceProvider,
                  new InjectedProviderFactoryResolver<FirebirdProvider>(serviceProvider),
                  new AssemblyProviderFactoryResolver("FirebirdSql.Data.FirebirdClient.FirebirdClientFactory, FirebirdSql.Data.FirebirdClient"))
        {
        }

        /// <summary>
        /// 获取提供者名称。
        /// </summary>
        public override string ProviderName => "Firebird";

        /// <summary>
        /// 获取当前连接的参数。
        /// </summary>
        /// <returns></returns>
        public override ConnectionParameter GetConnectionParameter(ConnectionString connectionString)
        {
            return new ConnectionParameter
            {
                Server = connectionString.Properties.TryGetValue("server", "datasource"),
                Database = connectionString.Properties.TryGetValue("database", "initial catalog"),
                UserId = connectionString.Properties.TryGetValue("userid", "user id", "user"),
                Password = connectionString.Properties.TryGetValue("password", "pwd"),
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
                .TrySetValue(parameter.Database, "database", "initial catalog")
                .TrySetValue(parameter.UserId, "userid", "user id")
                .TrySetValue(parameter.Password, "password", "pwd")
                .Update();
        }

        /// <summary>
        /// 注册服务。
        /// </summary>
        /// <param name="services"></param>
        public override IServiceCollection RegisterServices(IServiceCollection services)
        {
            return services.AddSingleton<IGeneratorProvider, BaseSequenceGenerator>()
                .AddSingleton<ISyntaxProvider, FirebirdSyntax>()
                .AddSingleton<ISchemaProvider, FirebirdSchema>()
                .AddSingleton<IBatcherProvider, MySqlBatcher>()
                .AddSingleton<IRecordWrapper, GeneralRecordWrapper>();
        }
    }
}
