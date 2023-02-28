// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Fireasy.Common.Analyzers.Metadata;
using System.Reflection;

namespace Fireasy.Common.Analyzers.DynamicProxy.Generator
{
    public class DynamicProxyClassBuilder
    {
        private readonly ClassMetadata _metadata;

        public DynamicProxyClassBuilder(ClassMetadata metadata)
        {
            _metadata = metadata;
        }

        public SourceText BuildSource()
        {
            var sb = new StringBuilder();
            foreach (var u in _metadata.Usings)
            {
                sb.AppendLine(u.ToString());
            }

            sb.AppendLine($@"
namespace {_metadata.Namespace}
{{
    public class {_metadata.ProxyTypeName} : {_metadata.TypeFullName}
    {{
        private List<System.Reflection.MemberInfo> _initMarks = new ();
        
        {BuildConstructors()}

        {BuildInitializeMethod()}

        {BuildInterceptMethod()}

        {BuildMethods()}
    }}
}}");

            return SourceText.From(sb.ToString(), Encoding.UTF8);
        }

        private string BuildConstructors()
        {
            var sb = new StringBuilder();
            foreach (var constructor in _metadata.Constructors)
            {
                sb.AppendLine($@"
            public {_metadata.ProxyTypeName}({BuildParameters(constructor)})
                : base ({BuildInvokeParameters(constructor)})
            {{
            }}");
            }

            return sb.ToString();
        }

        private string BuildInitializeMethod()
        {
            var sb = new StringBuilder();
            sb.AppendLine(@"
            private void _Initialize(List<IInterceptor> interceptors, InterceptCallInfo callInfo)
            {
                if (!this._initMarks.Contains(callInfo.Member))
                {
                    for (int i = 0; i < interceptors.Count; i++)
                    {
                        InterceptContext context = new InterceptContext(callInfo.Member, this);
                        interceptors[i].Initialize(context);
                    }
                    this._initMarks.Add(callInfo.Member);
                }
            }

            private async Task _InitializeAsync(List<IAsyncInterceptor> interceptors, InterceptCallInfo callInfo)
            {
                if (!this._initMarks.Contains(callInfo.Member))
                {
                    for (int i = 0; i < interceptors.Count; i++)
                    {
                        InterceptContext context = new InterceptContext(callInfo.Member, this);
                        await interceptors[i].InitializeAsync(context);
                    }
                    this._initMarks.Add(callInfo.Member);
                }
            }");

            return sb.ToString();
        }

        private string BuildInterceptMethod()
        {
            var sb = new StringBuilder();
            sb.AppendLine(@"
            private void _Intercept(List<IInterceptor> interceptors, InterceptCallInfo callInfo, InterceptType interceptType)
            {
                callInfo.InterceptType = interceptType;
                callInfo.Break = false;
                for (int i = 0; i < interceptors.Count; i++)
                {
                    if (callInfo.Break)
                    {
                        break;
                    }

                    interceptors[i].Intercept(callInfo);
                }
            }

            private async Task _InterceptAsync(List<IAsyncInterceptor> interceptors, InterceptCallInfo callInfo, InterceptType interceptType)
            {
                callInfo.InterceptType = interceptType;
                callInfo.Break = false;
                for (int i = 0; i < interceptors.Count; i++)
                {
                    if (callInfo.Break)
                    {
                        break;
                    }

                    await interceptors[i].InterceptAsync(callInfo);
                }
            }");

            return sb.ToString();
        }

        private string BuildMethods()
        {
            var sb = new StringBuilder();
            foreach (var kvp in _metadata.Methods)
            {
                var interceptors = GetInterceptorTypes(kvp.Key, kvp.Value);

                if (interceptors.Count == 0)
                {
                    continue;
                }

                BuildMethod(sb, kvp.Key, interceptors, kvp.Value.ThrowExpcetion);
            }

            return sb.ToString();
        }

        private void BuildMethod(StringBuilder sb, IMethodSymbol method, List<ITypeSymbol> interceptors, bool throwExp)
        {
            var returnType = GetMethodReturnType(method);
            var isAsync = IsAsyncMethod(method);

            var realReturnType = isAsync ? GetMethodReturnTypeOfAsync(method) : returnType;

            sb.AppendLine($@"
            public override {(isAsync ? "async" : string.Empty)} {returnType} {method.Name}{GetGenericTypes(method)}({BuildParameters(method)})
            {{
                var interceptors = new List<{(isAsync ? "IAsyncInterceptor" : "IInterceptor")}> {{ {BuildInterceptors(interceptors)} }};
                var info = new InterceptCallInfo();
                info.Target = this;
                info.Arguments = new object[] {{ {BuildInvokeParameters(method, false)} }};
                info.Member = System.Reflection.MethodInfo.GetCurrentMethod();
                
                try
                {{
                    {(isAsync ? "await _InitializeAsync" : "_Initialize")}(interceptors, info);
                    {(isAsync ? "await _InterceptAsync" : "_Intercept")}(interceptors, info, InterceptType.BeforeMethodCall);
                    {BuildOutParameters(method)}
                    if (info.Cancel)
                    {{
                        {(returnType != "void" ? "return info.ReturnValue == null ? default : (" + realReturnType + ")info.ReturnValue;" : string.Empty)}
                    }}
                    {(returnType != "void" ? "info.ReturnValue = " : string.Empty)} {(isAsync ? "await " : string.Empty)}base.{method.Name}{GetGenericTypes(method)}({BuildInvokeParameters(method)});
                    {(isAsync ? "await _InterceptAsync" : "_Intercept")}(interceptors, info, InterceptType.AfterMethodCall);
                }}
                catch (System.Exception exp)
                {{
                    info.Exception = exp;
                    {(isAsync ? "await _InterceptAsync" : "_Intercept")}(interceptors, info, InterceptType.Catching);
                    {(throwExp ? "throw exp;" : string.Empty)}
                }}
                finally
                {{
                    {(isAsync ? "await _InterceptAsync" : "_Intercept")}(interceptors, info, InterceptType.Finally);
                }}
                {(returnType != "void" ? "return info.ReturnValue == null ? default : (" + realReturnType + ")info.ReturnValue;": string.Empty)}
            }}");
        }

        private string BuildParameters(IMethodSymbol method)
        {
            var sb = new StringBuilder();

            for (var i = 0; i < method.Parameters.Length; i++)
            {
                if (i > 0)
                {
                    sb.Append(", ");
                }
                sb.Append($"{GetParameterRefKind(method.Parameters[i].RefKind)}{method.Parameters[i].Type.ToDisplayString()} {method.Parameters[i].Name}");
            }

            return sb.ToString();
        }

        private string BuildInvokeParameters(IMethodSymbol method, bool hasRefKind = true)
        {
            var sb = new StringBuilder();

            for (var i = 0; i < method.Parameters.Length; i++)
            {
                if (i > 0)
                {
                    sb.Append(", ");
                }
                sb.Append($"{(hasRefKind ? GetParameterRefKind(method.Parameters[i].RefKind) : string.Empty)}{method.Parameters[i].Name}");
            }

            return sb.ToString();
        }

        private string BuildOutParameters(IMethodSymbol method)
        {
            var sb = new StringBuilder();

            for (var i = 0; i < method.Parameters.Length; i++)
            {
                if (method.Parameters[i].RefKind == RefKind.Out)
                {
                    sb.AppendLine($"                    {method.Parameters[i].Name} = default;");
                }
            }

            return sb.ToString();
        }

        private string GetParameterRefKind(RefKind refKind)
        {
            switch (refKind)
            {
                case RefKind.Ref: return "ref ";
                case RefKind.Out: return "out ";
                default: return string.Empty;
            }
        }

        private string BuildInterceptors(List<ITypeSymbol> interceptors)
        {
            var sb = new StringBuilder();
            foreach (var type in interceptors)
            {
                if (sb.Length > 0)
                {
                    sb.Append(", ");
                }

                sb.Append($"new {type.ToDisplayString()}()");
            }

            return sb.ToString();
        }

        private string GetGenericTypes(IMethodSymbol method)
        {
            if (method.IsGenericMethod)
            {
                var sb = new StringBuilder();
                sb.Append("<");
                for (var i = 0; i < method.TypeArguments.Length; i++)
                {
                    if (sb.Length > 1)
                    {
                        sb.Append(',');
                    }
                    sb.Append(method.TypeArguments[i].ToDisplayString());
                }

                sb.Append(">");

                return sb.ToString();
            }

            return string.Empty;
        }

        private List<ITypeSymbol> GetInterceptorTypes(IMethodSymbol method, InterceptorMetadata metadatas)
        {
            var interceptors = new List<ITypeSymbol>();
            var isAsync = IsAsyncMethod(method);

            foreach (var type in metadatas.InterceptorTypes)
            {
                if ((isAsync && type.Interfaces.Any(t => t.Name == "IAsyncInterceptor")) ||
                    (!isAsync && type.Interfaces.Any(t => t.Name == "IInterceptor")))
                {
                    interceptors.Add(type);
                }
            }

            return interceptors;
        }

        private string GetMethodReturnType(IMethodSymbol symbol)
        {
            if (symbol.ReturnsVoid)
            {
                return "void";
            }

            return symbol.ReturnType.ToDisplayString();
        }

        private string GetMethodReturnTypeOfAsync(IMethodSymbol symbol)
        {
            if (symbol.ReturnsVoid)
            {
                return "void";
            }

            if (IsAsyncMethod(symbol) && symbol.ReturnType is INamedTypeSymbol named && named.IsGenericType)
            {
                return named.TypeArguments[0].ToDisplayString();
            }

            return symbol.ReturnType.ToDisplayString();
        }

        private bool IsAsyncMethod(IMethodSymbol symbol)
        {
            if (symbol.ReturnType.Name.StartsWith("Task") ||
                symbol.ReturnType.Name.StartsWith("ValueTask"))
            {
                return true;
            }

            return false;
        }
    }
}
