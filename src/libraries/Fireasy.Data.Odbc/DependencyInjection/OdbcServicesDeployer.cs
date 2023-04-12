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

[assembly: ServicesDeploy(typeof(OdbcServicesDeployer))]

namespace Fireasy.Data.DependencyInjection
{
    /// <summary>
    /// OleDb 服务部署。
    /// </summary>
    public class OdbcServicesDeployer : IServicesDeployer
    {
        void IServicesDeployer.Configure(IServiceCollection services)
        {
            var customizer = services.GetOrAddObjectAccessor<ProviderCustomizer>();

            customizer.AddProvider<OdbcProvider>("Odbc");
        }
    }
}
