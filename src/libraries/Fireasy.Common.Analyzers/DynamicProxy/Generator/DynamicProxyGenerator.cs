// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using System.Diagnostics;

namespace Fireasy.Common.Analyzers.DynamicProxy.Generator
{
    /// <summary>
    /// 动态代理生成器。
    /// </summary>
    [Generator]
    public class DynamicProxyGenerator : ISourceGenerator
    {
        void ISourceGenerator.Initialize(GeneratorInitializationContext context)
        {
            Debugger.Launch();
            context.RegisterForSyntaxNotifications(() => new DynamicProxySyntaxReceiver());
        }

        void ISourceGenerator.Execute(GeneratorExecutionContext context)
        {
            var mappers = new Dictionary<string, string>();

            if (context.SyntaxContextReceiver is DynamicProxySyntaxReceiver receiver)
            {
                var metadatas = receiver.GetMetadatas();
                metadatas.ForEach(s =>
                {
                    context.AddSource(s.SourceCodeName, new DynamicProxyClassBuilder(s).BuildSource());
                    mappers.Add(s.TypeFullName, s.ProxyTypeFullName);
                });

                if (mappers.Count > 0)
                {
                    context.AddSource("DynamicProxyServicesDeployer.cs", BuildDeploySourceCode(mappers));
                }
            }
        }

        private SourceText BuildDeploySourceCode(Dictionary<string, string> mappers)
        {
            var sb = new StringBuilder();
            sb.AppendLine(@"
using Fireasy.Common.DependencyInjection;
using Fireasy.Common.DynamicProxy;
using Microsoft.Extensions.DependencyInjection;

[assembly: Fireasy.Common.DependencyInjection.ServicesDeployAttribute(typeof(__DynamicProxyNs.__DynamicProxyServicesDeployer))]

namespace __DynamicProxyNs
{
    internal class __DynamicProxyServicesDeployer: IServicesDeployer
    {
        void IServicesDeployer.Configure(IServiceCollection services)
        {");

            foreach (var kvp in mappers)
            {
                sb.AppendLine($"            Container.TryAdd(typeof({kvp.Key}), typeof({kvp.Value}));");
            }
            sb.AppendLine(@"
        }
    }
}");
            return SourceText.From(sb.ToString(), Encoding.UTF8);
        }
    }
}
