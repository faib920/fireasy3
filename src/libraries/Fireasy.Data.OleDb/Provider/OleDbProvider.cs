// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Microsoft.Extensions.DependencyInjection;
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

        public override ConnectionParameter GetConnectionParameter(ConnectionString connectionString)
        {
            throw new NotImplementedException();
        }

        public override IServiceCollection RegisterServices(IServiceCollection services)
        {
            throw new NotImplementedException();
        }

        public override void UpdateConnectionString(ConnectionString connectionString, ConnectionParameter parameter)
        {
            throw new NotImplementedException();
        }
    }
}
