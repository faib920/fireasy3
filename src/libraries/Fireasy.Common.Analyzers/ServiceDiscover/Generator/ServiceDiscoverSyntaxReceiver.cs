// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Fireasy.Common.Analyzers.ServiceDiscover.Metadata;

namespace Fireasy.Common.Analyzers.ServiceDiscover.Generator
{
    internal class ServiceDiscoverSyntaxReceiver : ISyntaxContextReceiver
    {
        private const string SingletonServiceName = "Fireasy.Common.DependencyInjection.ISingletonService";
        private const string TransientServiceName = "Fireasy.Common.DependencyInjection.ITransientService";
        private const string ScopedServiceName = "Fireasy.Common.DependencyInjection.IScopedService";
        private const string RegisterAttributeName = "Fireasy.Common.DependencyInjection.ServiceRegisterAttribute";

        private List<ClassMetadata> _metadatas = new();

        void ISyntaxContextReceiver.OnVisitSyntaxNode(GeneratorSyntaxContext context)
        {
            if (context.Node is ClassDeclarationSyntax classSyntax)
            {
                AnalyseClassSyntax(context.SemanticModel, classSyntax);
            }
        }

        /// <summary>
        /// 分析类型语法。
        /// </summary>
        /// <param name="model"></param>
        /// <param name="syntax"></param>
        private void AnalyseClassSyntax(SemanticModel model, ClassDeclarationSyntax syntax)
        {
            var typeSymbol = (ITypeSymbol)model.GetDeclaredSymbol(syntax)!;

            var interfaces = typeSymbol.Interfaces;
            var regAttr = typeSymbol.GetAttributes().FirstOrDefault(s => s.AttributeClass!.ToDisplayString() == RegisterAttributeName);

            var lifetime = string.Empty;
            if (regAttr != null)
            {
                lifetime = GetLifetime((int)regAttr.ConstructorArguments[0].Value!);
            }
            else if (interfaces.Any(s => s.ToDisplayString() == SingletonServiceName))
            {
                lifetime = "Singleton";
            }
            else if (interfaces.Any(s => s.ToDisplayString() == TransientServiceName))
            {
                lifetime = "Transient";
            }
            else if (interfaces.Any(s => s.ToDisplayString() == ScopedServiceName))
            {
                lifetime = "Scoped";
            }

            if (!string.IsNullOrEmpty(lifetime))
            {
                var serviceTypes = GetServiceTypes(interfaces).ToList();
                if (serviceTypes.Count == 0 && (typeSymbol.BaseType?.Name == "Object" || typeSymbol.BaseType?.IsAbstract == false))
                {
                    serviceTypes.Add(typeSymbol);
                }

                _metadatas.Add(new ClassMetadata(typeSymbol, lifetime).AddServiceTypes(serviceTypes));
            }
        }

        private string GetLifetime(int value) => value switch
        {
            0 => "Singleton",
            1 => "Scoped",
            2 => "Transient",
            _ => string.Empty
        };


        private IEnumerable<ITypeSymbol> GetServiceTypes(IEnumerable<INamedTypeSymbol> types)
        {
            foreach (var type in types)
            {
                if (type.ToDisplayString() == SingletonServiceName ||
                    type.ToDisplayString() == TransientServiceName ||
                    type.ToDisplayString() == ScopedServiceName)
                {
                    continue;
                }

                yield return type;
            }
        }

        /// <summary>
        /// 获取类元数据列表。
        /// </summary>
        /// <returns></returns>
        public List<ClassMetadata> GetMetadatas()
        {
            return _metadatas;
        }
    }
}
