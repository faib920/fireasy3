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
using System.Data.OleDb;

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
        /// 注册服务。
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public override IServiceCollection RegisterServices(IServiceCollection services)
        {
            services = base.RegisterServices(services);
            services.TryAddSingleton<ISyntaxProvider, OleDbSyntax>();
            services.TryAddSingleton<ISchemaProvider, OleDbSchema>();
            services.TryAddSingleton<IRecordWrapper, GeneralRecordWrapper>();
            return services;
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
    }
}
