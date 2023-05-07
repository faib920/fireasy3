// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Fireasy.Data.RecordWrapper;
using Fireasy.Data.Schema;
using Fireasy.Data.Syntax;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Data.Common;
using System.Data.Odbc;
using System.Linq;
using System.Text.RegularExpressions;

namespace Fireasy.Data.Provider
{
    /// <summary>
    /// OleDb 驱动适配。
    /// </summary>
    public class OdbcProvider : ProviderBase
    {
        /// <summary>
        /// 初始化 <see cref="OdbcProvider"/> 类的新实例。
        /// </summary>
        /// <param name="serviceProvider"></param>
        public OdbcProvider(IServiceProvider serviceProvider)
            : base(serviceProvider, new InstantiatedProviderFactoryResolver(OdbcFactory.Instance))
        {
        }

        /// <summary>
        /// 获取当前连接的参数。
        /// </summary>
        /// <param name="connectionString">连接字符串对象。</param>
        /// <returns>连接字符串参数对象。</returns>
        public override ConnectionParameter GetConnectionParameter(ConnectionString connectionString)
        {
            return new ConnectionParameter
            {
                Server = connectionString.Properties.TryGetValue("data source", "server"),
                Database = connectionString.Properties.TryGetValue("database"),
                UserId = connectionString.Properties.TryGetValue("uid", "userid"),
                Password = connectionString.Properties.TryGetValue("pwd")
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
                .TrySetValue(parameter.Database, "database")
                .TrySetValue(parameter.UserId, "uid", "user id")
                .TrySetValue(parameter.Password, "pwd")
                .Update();
        }

        /// <summary>
        /// 初始化。
        /// </summary>
        /// <param name="context">初始化上下文。</param>
        public override void Initialize(ProviderInitializeContext context)
        {
            base.Initialize(context);

            var driver = context.ConnectionString.Properties.TryGetValue("driver");
            if (driver?.StartsWith("MySQL ODBC") == true)
            {
                context.Services.TryAddSingleton<ISyntaxProvider, MySqlSyntax>();
                context.Services.TryAddSingleton<ISchemaProvider, MySqlSchema>();
            }
            else if (Regex.IsMatch(driver, @"DM(\d+) ODBC DRIVER"))
            {
                context.Services.TryAddSingleton<ISyntaxProvider, DamengSyntax>();
                context.Services.TryAddSingleton<ISchemaProvider, DamengSchema>();
            }
            else if (driver == "SQL Server")
            {
                context.Services.TryAddSingleton<ISyntaxProvider, SqlServerSyntax>();
                context.Services.TryAddSingleton<ISchemaProvider, SqlServerSchema>();
            }
            else if (driver?.StartsWith("Oracle") == true)
            {
                context.Services.TryAddSingleton<ISyntaxProvider, OracleSyntax>();
                context.Services.TryAddSingleton<ISchemaProvider, OracleSchema>();
            }
            else if (driver?.StartsWith("Kingbase") == true)
            {
                context.Services.TryAddSingleton<ISyntaxProvider, KingbaseSyntax>();
                context.Services.TryAddSingleton<ISchemaProvider, KingbaseSchema>();
            }
            else if (driver?.StartsWith("OSCAR ODBC") == true)
            {
                context.Services.TryAddSingleton<ISyntaxProvider, ShenTongSyntax>();
                context.Services.TryAddSingleton<ISchemaProvider, ShenTongSchema>();
            }

            context.Services.TryAddSingleton<ISchemaProvider, OdbcSchema>();
            context.Services.TryAddSingleton<IRecordWrapper, GeneralRecordWrapper>();
        }

        /// <summary>
        /// 处理 <see cref="DbCommand"/> 对象。
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public override DbCommand PrepareCommand(DbCommand command)
        {
            //OleDb需要调整参数的顺序
            var matches = Regex.Matches(command.CommandText, @"(@[\w]+)");
            if (matches.Count > 0)
            {
                var parameters = new DbParameter[command.Parameters.Count];
                command.Parameters.CopyTo(parameters, 0);
                command.Parameters.Clear();

                var dict = parameters.ToDictionary(s => s.ParameterName);

                foreach (Match match in matches)
                {
                    if (dict.TryGetValue(match.Value.Substring(1), out var par))
                    {
                        command.Parameters.Add(par);
                    }
                }
            }

            return base.PrepareCommand(command);
        }
    }
}
