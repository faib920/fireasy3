// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Fireasy.Common.Analyzers.DynamicProxy.Metadata;

namespace Fireasy.Common.Analyzers.DynamicProxy.Generator
{
    /// <summary>
    /// 动态代理语法接收器。
    /// </summary>
    public class DynamicProxySyntaxReceiver : ISyntaxContextReceiver
    {
        private const string InterceptorAttributeName = "Fireasy.Common.DynamicProxy.InterceptAttribute";
        private const string IgnoreThrowExceptionAttributeName = "Fireasy.Common.DynamicProxy.IgnoreThrowExceptionAttribute";
        private const string InterceptorTagName = "Fireasy.Common.DynamicProxy.IInterceptor";
        private const string AsyncInterceptorTagName = "Fireasy.Common.DynamicProxy.IAsyncInterceptor";

        private List<ClassMetadata> _metadata = new();

        /// <summary>
        /// 所有的AopInterceptor
        /// </summary>
        public List<string> AopAttributeList = new List<string>();

        void ISyntaxContextReceiver.OnVisitSyntaxNode(GeneratorSyntaxContext context)
        {
            if (context.Node is InterfaceDeclarationSyntax interfaceSyntax)
            {
            }

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
            if (typeSymbol.IsSealed)
            {
                return;
            }

            var interceptorMetadataOfClass = FindInterceptorMetadata(typeSymbol);

            var metadata = new ClassMetadata(typeSymbol);

            foreach (var memberSymbol in typeSymbol.GetMembers())
            {
                //如果不是方法或属性
                if (memberSymbol.Kind != SymbolKind.Method && memberSymbol.Kind != SymbolKind.Property)
                {
                    continue;
                }

                if (memberSymbol is IMethodSymbol method)
                {
                    if (method.MethodKind == MethodKind.Constructor)
                    {
                        metadata.AddConstructor(method);
                        continue;
                    }

                    if (method.MethodKind != MethodKind.Ordinary)
                    {
                        continue;
                    }
                }

                if (!memberSymbol.IsVirtual || memberSymbol.DeclaredAccessibility != Accessibility.Public)
                {
                    continue;
                }

                if (memberSymbol.GetAttributes().Any(s => s.AttributeClass!.ToDisplayString() == InterceptorAttributeName))
                {
                    var interceptorMetadataOfMember = FindInterceptorMetadata(memberSymbol);
                    if (interceptorMetadataOfMember != null)
                    {
                        metadata.AddMember(memberSymbol, interceptorMetadataOfMember);
                    }
                }
                else if (interceptorMetadataOfClass != null)
                {
                    var hasIgnoreThrowExpAttr = HasIgnoreThrowExceptionAttribute(memberSymbol);
                    metadata.AddMember(memberSymbol, interceptorMetadataOfClass.Clone(!hasIgnoreThrowExpAttr));
                }
            }

            if (metadata.IsValid)
            {
                _metadata.Add(FindUsings(syntax, metadata));
            }
        }

        /// <summary>
        /// 获取拦截器的元数据。
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        private InterceptorMetadata? FindInterceptorMetadata(ISymbol symbol)
        {
            var types = new List<ITypeSymbol>();
            foreach (AttributeData classAttr in symbol.GetAttributes().Where(s => s.AttributeClass!.ToDisplayString() == InterceptorAttributeName))
            {
                var interceptorType = GetInterceptorType(classAttr.ConstructorArguments[0].Value);
                if (interceptorType != null)
                {
                    types.Add(interceptorType);
                }
            }

            if (!types.Any())
            {
                return null;
            }

            var hasIgnoreThrowExpAttr = HasIgnoreThrowExceptionAttribute(symbol);

            return new InterceptorMetadata(types, !hasIgnoreThrowExpAttr);
        }

        /// <summary>
        /// 获取是否有忽略抛出异常的特性。
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        private bool HasIgnoreThrowExceptionAttribute(ISymbol symbol)
        {
            var ignoreThrowExpAttr = symbol.GetAttributes().FirstOrDefault(s => s.AttributeClass!.ToDisplayString() == IgnoreThrowExceptionAttributeName);

            return ignoreThrowExpAttr != null;
        }

        /// <summary>
        /// 查找引用的命名空间。
        /// </summary>
        /// <param name="syntax"></param>
        /// <param name="metadata"></param>
        /// <returns></returns>
        private ClassMetadata FindUsings(SyntaxNode syntax, ClassMetadata metadata)
        {
            var parent = syntax.Parent;
            while (parent != null)
            {
                if (parent is CompilationUnitSyntax compUnitSyntax)
                {
                    metadata.AddUsings(compUnitSyntax.Usings.Select(s => s.ToString()));
                }

                parent = parent.Parent;
            }

            return metadata;
        }

        /// <summary>
        /// 获取类元数据列表。
        /// </summary>
        /// <returns></returns>
        public List<ClassMetadata> GetMetadatas()
        {
            return _metadata;
        }

        /// <summary>
        /// 获取拦截器类型。
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        private ITypeSymbol? GetInterceptorType(object? symbol)
        {
            if (symbol is not ITypeSymbol type)
            {
                return null;
            }

            if (type.Interfaces.Any(s => s.ToDisplayString() == InterceptorTagName || s.ToDisplayString() == AsyncInterceptorTagName))
            {
                return type;
            }

            return null;
        }
    }
}
