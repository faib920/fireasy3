// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Fireasy.Common.DependencyInjection;
using Fireasy.Data.Converter;
using Fireasy.Data.DependencyInjection;
using Fireasy.Data.Provider;
using Microsoft.Extensions.DependencyInjection;

[assembly: ServicesDeploy(typeof(DataServicesDeployer))]

namespace Fireasy.Data.DependencyInjection
{
    /// <summary>
    /// 服务部署。
    /// </summary>
    public class DataServicesDeployer : IServicesDeployer
    {
        void IServicesDeployer.Configure(IServiceCollection services)
        {
            services.AddSingleton<IProviderManager, DefaultProviderManager>();
            services.AddSingleton<IDatabaseFactory, DefaultDatabaseFactory>();
            services.AddSingleton<IRowMapperFactory, DefaultRowMapperFactory>();
            services.AddSingleton<IValueConvertManager, DefaultValueConvertManager>();
            services.AddScoped<IDatabase>(sp => sp.GetRequiredService<IDatabaseFactory>().CreateDatabase());
            services.AddSingleton<IConnectionStringEncryptor, DefaultConnectionStringEncryptor>();
            services.AddScoped<DistributedController>();
        }
    }
}
