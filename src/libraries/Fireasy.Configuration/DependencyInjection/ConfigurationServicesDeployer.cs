// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Fireasy.Common.DependencyInjection;
using Fireasy.Configuration.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

[assembly: ServicesDeploy(typeof(ConfigurationServicesDeployer))]

namespace Fireasy.Configuration.DependencyInjection
{
    /// <summary>
    /// 服务部署。
    /// </summary>
    public class ConfigurationServicesDeployer : IServicesDeployer
    {
        void IServicesDeployer.Configure(IServiceCollection services)
        {
            services.AddSingleton<ConfigurationUnity>();
            services.TryAddSingleton<IConfigurationAccessor>(new DefaultConfigurationAccessor());
        }
    }
}
