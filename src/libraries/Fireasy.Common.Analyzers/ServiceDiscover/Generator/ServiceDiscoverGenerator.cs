// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Fireasy.Common.Analyzers.ServiceDiscover.Metadata;
using System.Diagnostics;

namespace Fireasy.Common.Analyzers.ServiceDiscover.Generator
{
    [Generator]
    public class ServiceDiscoverGenerator : ISourceGenerator
    {
        void ISourceGenerator.Initialize(GeneratorInitializationContext context)
        {
            //Debugger.Launch();
            context.RegisterForSyntaxNotifications(() => new ServiceDiscoverSyntaxReceiver());
        }

        void ISourceGenerator.Execute(GeneratorExecutionContext context)
        {
            if (context.SyntaxContextReceiver is ServiceDiscoverSyntaxReceiver receiver)
            {
                var metadatas = receiver.GetMetadatas();

                if (metadatas.Count > 0)
                {
                    context.AddSource("ServiceDiscoverServicesDeployer.cs", BuildDiscoverSourceCode(metadatas));
                }
            }
        }

        private SourceText BuildDiscoverSourceCode(List<ClassMetadata> metadatas)
        {
            var sb = new StringBuilder();
            sb.AppendLine(@"
using Fireasy.Common.DependencyInjection;
using Fireasy.Common.DynamicProxy;
using Microsoft.Extensions.DependencyInjection;

[assembly: Fireasy.Common.DependencyInjection.ServicesDeployAttribute(typeof(__ServiceDiscoverNs.__ServiceDiscoverServicesDeployer), Priority = 1)]

namespace __ServiceDiscoverNs
{
    internal class __ServiceDiscoverServicesDeployer: IServicesDeployer
    {
        void IServicesDeployer.Configure(IServiceCollection services)
        {");

            foreach (var metadata in metadatas)
            {
                foreach (var svrType in metadata.ServiceTypes)
                {
                    sb.AppendLine($"            services.Add{metadata.Lifetime}(typeof({GetTypeName(svrType)}), TryGetProxyType(typeof({GetTypeName(metadata.ImplementationType)})));");
                }
            }
            sb.AppendLine(@"
        }

        private Type TryGetProxyType(Type implType)
        {
            if (Container.TryGet(implType, out var proxyType))
            {
                return proxyType;
            }
            return implType;
        }
    }
}");
            return SourceText.From(sb.ToString(), Encoding.UTF8);
        }

        private string GetTypeName(ITypeSymbol symbol)
        {
            if (symbol is INamedTypeSymbol namedTypeSymbol)
            {
                if (namedTypeSymbol.IsGenericType)
                {
                    var t = namedTypeSymbol.ToDisplayString();
                    return t.Substring(0, t.IndexOf("<") + 1) + new string(',', namedTypeSymbol.TypeArguments.Length - 1) + ">";
                }
            }

            return symbol.ToDisplayString();
        }
    }
}
