﻿// -----------------------------------------------------------------------
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
using System.ComponentModel;

namespace Fireasy.Data.Provider
{
    /// <summary>
    /// ShenTong(神通)数据库提供者。
    /// </summary>
    [Description("神通")]
    public class ShenTongProvider : ProviderBase
    {
        /// <summary>
        /// 初始化 <see cref="ShenTongProvider"/> 类的新实例。
        /// </summary>
        /// <param name="serviceProvider"></param>
        public ShenTongProvider(IServiceProvider serviceProvider)
            : base(serviceProvider, new AssemblyProviderFactoryResolver("System.Data.OscarClient.OscarFactory, Oscar.Data.SqlClient"))
        {
        }

        /// <summary>
        /// 获取提供者名称。
        /// </summary>
        public override string ProviderName => "ShenTong";

        /// <summary>
        /// 获取当前连接的参数。
        /// </summary>
        /// <returns></returns>
        public override ConnectionParameter GetConnectionParameter(ConnectionString connectionString)
        {
            return new ConnectionParameter
            {
                Server = connectionString.Properties.TryGetValue("data source", "server", "host"),
                Database = connectionString.Properties.TryGetValue("database", "initial catalog", "db"),
                UserId = connectionString.Properties.TryGetValue("user id", "userid", "uid", "user name", "username", "user"),
                Password = connectionString.Properties.TryGetValue("password", "pwd", "psw")
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
                .TrySetValue(parameter.Database, "database", "initial catalog", "db")
                .TrySetValue(parameter.UserId, "user id", "userid", "uid", "user name", "username", "user")
                .TrySetValue(parameter.Password, "password", "pwd", "psw")
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
            context.Services.TryAddSingleton<ISyntaxProvider, ShenTongSyntax>();
            context.Services.TryAddSingleton<ISchemaProvider, ShenTongSchema>();
            context.Services.TryAddSingleton<IRecordWrapper, GeneralRecordWrapper>();
        }
    }
}
