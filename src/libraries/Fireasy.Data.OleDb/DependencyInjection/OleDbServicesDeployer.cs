// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Fireasy.Common.DependencyInjection;
using Fireasy.Data.DependencyInjection;
using Fireasy.Data.Provider;
using Microsoft.Extensions.DependencyInjection;

[assembly: ServicesDeploy(typeof(OleDbServicesDeployer))]

namespace Fireasy.Data.DependencyInjection
{
    /// <summary>
    /// OleDb 服务部署。
    /// </summary>
    public class OleDbServicesDeployer : IServicesDeployer
    {
        void IServicesDeployer.Configure(IServiceCollection services)
        {
            var customizer = services.GetOrAddObjectAccessor<ProviderCustomizer>();

            customizer.AddProvider<OleDbProvider>("OleDb");
        }
    }
}
