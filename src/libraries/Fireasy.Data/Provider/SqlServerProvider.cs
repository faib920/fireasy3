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
    /// MsSql数据库提供者。
    /// </summary>
    public class SqlServerProvider : ProviderBase
    {
        /// <summary>
        /// 初始化 <see cref="SqlServerProvider"/> 类的新实例。
        /// </summary>
        /// <param name="serviceProvider"></param>
        public SqlServerProvider(IServiceProvider serviceProvider)
            : base(serviceProvider,
                  new InjectedProviderFactoryResolver<SqlServerProvider>(serviceProvider),
                  new AssemblyProviderFactoryResolver(
                "Microsoft.Data.SqlClient.SqlClientFactory, Microsoft.Data.SqlClient",
                "System.Data.SqlClient.SqlClientFactory, System.Data.SqlClient"))
        {
        }

        /// <summary>
        /// 获取提供者名称。
        /// </summary>
        public override string ProviderName => "SqlServer";

        /// <summary>
        /// 获取当前连接的参数。
        /// </summary>
        /// <returns></returns>
        public override ConnectionParameter GetConnectionParameter(ConnectionString connectionString)
        {
            return new ConnectionParameter
            {
                Server = connectionString.Properties.TryGetValue("data source", "server"),
                Database = connectionString.Properties.TryGetValue("initial catalog", "database", "attachdbfilename"),
                UserId = connectionString.Properties.TryGetValue("user id", "uid"),
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
            connectionString.Properties.TrySetValue(parameter.Server, "data source", "server")
                .TrySetValue(parameter.Database, "initial catalog", "database", "attachdbfilename")
                .TrySetValue(parameter.UserId, "user id", "uid")
                .TrySetValue(parameter.Password, "password", "pwd")
                .Update();
        }

        /// <summary>
        /// 初始化。
        /// </summary>
        /// <param name="context">初始化上下文。</param>
        public override void Initialize(ProviderInitializeContext context)
        {
            base.Initialize(context);
            context.Services.TryAddSingleton<IGeneratorProvider, BaseSequenceGenerator>();
            context.Services.TryAddSingleton<ISyntaxProvider, SqlServerSyntax>();
            context.Services.TryAddSingleton<ISchemaProvider, SqlServerSchema>();
            context.Services.TryAddSingleton<IBatcherProvider, SqlServerBatcher>();
            context.Services.TryAddSingleton<IRecordWrapper, GeneralRecordWrapper>();
        }
    }
}
