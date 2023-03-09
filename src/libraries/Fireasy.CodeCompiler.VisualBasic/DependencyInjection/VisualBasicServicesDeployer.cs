// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Fireasy.CodeCompiler.VBasic;
using Fireasy.Common.Compiler;
using Fireasy.Common.DependencyInjection;
using Fireasy.Data.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

[assembly: ServicesDeploy(typeof(VisualBasicServicesDeployer))]

namespace Fireasy.Data.DependencyInjection
{
    /// <summary>
    /// 服务部署。
    /// </summary>
    public class VisualBasicServicesDeployer : IServicesDeployer
    {
        void IServicesDeployer.Configure(IServiceCollection services)
        {
            var manager = services.GetSingletonInstance<ICodeCompilerManager>();
            manager!.Register<VisualBasicCodeCompiler>("vb");
        }
    }
}
