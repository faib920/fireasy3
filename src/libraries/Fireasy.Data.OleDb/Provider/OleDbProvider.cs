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
using System.Data.OleDb;
using System.Linq;
using System.Text.RegularExpressions;

namespace Fireasy.Data.Provider
{
    /// <summary>
    /// OleDb 驱动适配。
    /// </summary>
    public class OleDbProvider : ProviderBase
    {
        /// <summary>
        /// 初始化 <see cref="OleDbProvider"/> 类的新实例。
        /// </summary>
        /// <param name="serviceProvider"></param>
        public OleDbProvider(IServiceProvider serviceProvider)
            : base(serviceProvider, new InstantiatedProviderFactoryResolver(OleDbFactory.Instance))
        {
        }

        /// <summary>
        /// 获取当前连接的参数。
        /// </summary>
        /// <param name="connectionString">连接字符串对象。</param>
        /// <returns>连接字符串参数对象。</returns>
        public override ConnectionParameter GetConnectionParameter(ConnectionString connectionString)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 使用参数更新指定的连接。
        /// </summary>
        /// <param name="connectionString">连接字符串对象。</param>
        /// <param name="parameter"></param>
        public override void UpdateConnectionString(ConnectionString connectionString, ConnectionParameter parameter)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 初始化。
        /// </summary>
        /// <param name="context">初始化上下文。</param>
        public override void Initialize(ProviderInitializeContext context)
        {
            base.Initialize(context);
            context.Services.TryAddSingleton<ISyntaxProvider, OleDbSyntax>();
            context.Services.TryAddSingleton<ISchemaProvider, OleDbSchema>();
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
