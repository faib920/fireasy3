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
    /// SQLite数据库提供者。使用 System.Data.SQLite 提供。
    /// </summary>
    public class SQLiteProvider : ProviderBase
    {
        /// <summary>
        /// 初始化 <see cref="SQLiteProvider"/> 类的新实例。
        /// </summary>
        /// <param name="serviceProvider"></param>
        public SQLiteProvider(IServiceProvider serviceProvider)
            : base(serviceProvider,
                  new InjectedProviderFactoryResolver<SQLiteProvider>(serviceProvider),
                  new AssemblyProviderFactoryResolver(
                "System.Data.SQLite.SQLiteFactory, System.Data.SQLite",
                "Microsoft.Data.Sqlite.SqliteFactory, Microsoft.Data.Sqlite"))
        {
        }

        /// <summary>
        /// 获取提供者名称。
        /// </summary>
        public override string ProviderName => "SQLite";

        /// <summary>
        /// 获取当前连接的参数。
        /// </summary>
        /// <returns></returns>
        public override ConnectionParameter GetConnectionParameter(ConnectionString connectionString)
        {
            return new ConnectionParameter
            {
                Database = connectionString.Properties["data source"],
            };
        }

        /// <summary>
        /// 使用参数更新指定的连接。
        /// </summary>
        /// <param name="connectionString">连接字符串对象。</param>
        /// <param name="parameter"></param>
        public override void UpdateConnectionString(ConnectionString connectionString, ConnectionParameter parameter)
        {
            connectionString.Properties
                .TrySetValue(parameter.Database, "data source")
                .Update();
        }

        /// <summary>
        /// 调整事务级别。
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public override IsolationLevel AmendIsolationLevel(IsolationLevel level)
        {
            return level == IsolationLevel.ReadUncommitted ?
                IsolationLevel.ReadCommitted : base.AmendIsolationLevel(level);
        }

        /// <summary>
        /// 注册服务。
        /// </summary>
        /// <param name="services"></param>
        public override IServiceCollection RegisterServices(IServiceCollection services)
        {
            services = base.RegisterServices(services);
            services.TryAddSingleton<IGeneratorProvider, BaseSequenceGenerator>();
            services.TryAddSingleton<ISyntaxProvider, SQLiteSyntax>();
            services.TryAddSingleton<ISchemaProvider, SQLiteSchema>();
            services.TryAddSingleton<IBatcherProvider, SQLiteBatcher>();
            services.TryAddSingleton<IRecordWrapper, GeneralRecordWrapper>();

            return services;
        }
    }
}
