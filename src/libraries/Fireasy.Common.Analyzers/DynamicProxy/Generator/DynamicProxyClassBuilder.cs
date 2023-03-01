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
    /// 动态代理类生成器。
    /// </summary>
    public class DynamicProxyClassBuilder
    {
        private readonly ClassMetadata _metadata;

        /// <summary>
        /// 初始化 <see cref="DynamicProxyClassBuilder"/> 类的新实例。
        /// </summary>
        /// <param name="metadata"></param>
        public DynamicProxyClassBuilder(ClassMetadata metadata)
        {
            _metadata = metadata;
        }

        /// <summary>
        /// 生成源代码。
        /// </summary>
        /// <returns></returns>
        public SourceText BuildSource()
        {
            var sb = new StringBuilder();
            foreach (var u in _metadata.Usings)
            {
                sb.AppendLine(u.ToString());
            }

            sb.AppendLine("using System.Reflection;");

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

        {BuildProperties()}
    }}
}}");

            return SourceText.From(sb.ToString(), Encoding.UTF8);
        }

        /// <summary>
        /// 生成所有构造函数。
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// 生成初始化方法。
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// 生成拦截调用方法。
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// 生成所有方法。
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// 生成指定的方法。
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="method"></param>
        /// <param name="interceptors"></param>
        /// <param name="throwExp"></param>
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
                info.Member = ((MethodInfo)MethodBase.GetCurrentMethod()).GetBaseDefinition();
                
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

        /// <summary>
        /// 生成所有属性。
        /// </summary>
        /// <returns></returns>
        private string BuildProperties()
        {
            var sb = new StringBuilder();
            foreach (var kvp in _metadata.Properties)
            {
                var interceptors = GetInterceptorTypes(kvp.Key, kvp.Value);

                if (interceptors.Count == 0)
                {
                    continue;
                }

                BuildProperty(sb, kvp.Key, interceptors, kvp.Value.ThrowExpcetion);
            }

            return sb.ToString();
        }

        /// <summary>
        /// 生成指定的属性。
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="property"></param>
        /// <param name="interceptors"></param>
        /// <param name="throwExp"></param>
        private void BuildProperty(StringBuilder sb, IPropertySymbol property, List<ITypeSymbol> interceptors, bool throwExp)
        {
            var propertyType = GetPropertyType(property);

            sb.AppendLine($@"
            public override {propertyType} {property.Name}
            {{");

            if (property.GetMethod != null)
            {
                sb.AppendLine($@"
                get
                {{
                    var interceptors = new List<IInterceptor> {{ {BuildInterceptors(interceptors)} }};
                    var info = new InterceptCallInfo();
                    info.Target = this;
                    info.Arguments = new object[] {{ }};
                    info.Member = this.GetType().GetTypeInfo().GetProperty(""{property.Name}"");
                
                    try
                    {{
                        _Initialize(interceptors, info);
                        _Intercept(interceptors, info, InterceptType.BeforeGetValue);
                        if (info.Cancel)
                        {{
                            return info.ReturnValue == null ? default : ({propertyType})info.ReturnValue;
                        }}
                        info.ReturnValue = base.{property.Name};
                        _Intercept(interceptors, info, InterceptType.AfterGetValue);
                    }}
                    catch (System.Exception exp)
                    {{
                        info.Exception = exp;
                        _Intercept(interceptors, info, InterceptType.Catching);
                        {(throwExp ? "throw exp;" : string.Empty)}
                    }}
                    finally
                    {{
                        _Intercept(interceptors, info, InterceptType.Finally);
                    }}
                    return info.ReturnValue == null ? default : ({propertyType})info.ReturnValue;
                }}");
            }
            if (property.SetMethod != null)
            {
                sb.AppendLine($@"
                set
                {{
                    var interceptors = new List<IInterceptor> {{ {BuildInterceptors(interceptors)} }};
                    var info = new InterceptCallInfo();
                    info.Target = this;
                    info.Arguments = new object[] {{ value }};
                    info.Member = this.GetType().GetTypeInfo().GetProperty(""{property.Name}"");
                
                    try
                    {{
                        _Initialize(interceptors, info);
                        _Intercept(interceptors, info, InterceptType.BeforeSetValue);
                        if (info.Cancel)
                        {{
                            return;
                        }}
                        base.{property.Name} = ({propertyType})info.Arguments[0];
                        _Intercept(interceptors, info, InterceptType.AfterSetValue);
                    }}
                    catch (System.Exception exp)
                    {{
                        info.Exception = exp;
                        _Intercept(interceptors, info, InterceptType.Catching);
                        {(throwExp ? "throw exp;" : string.Empty)}
                    }}
                    finally
                    {{
                        _Intercept(interceptors, info, InterceptType.Finally);
                    }}
                }}");
            }
            sb.AppendLine(@"
            }");
        }

        /// <summary>
        /// 生成参数列表。
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 生成参数的调用列表。
        /// </summary>
        /// <param name="method"></param>
        /// <param name="hasRefKind"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 生成 out 参数赋值列表。
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 获取参数的引用类型。
        /// </summary>
        /// <param name="refKind"></param>
        /// <returns></returns>
        private string GetParameterRefKind(RefKind refKind)
        {
            switch (refKind)
            {
                case RefKind.Ref: return "ref ";
                case RefKind.Out: return "out ";
                default: return string.Empty;
            }
        }

        /// <summary>
        /// 生成初始化拦截器实例列表。
        /// </summary>
        /// <param name="interceptors"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 获取泛型类型列表。
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 获取方法的拦截器类型列表。
        /// </summary>
        /// <param name="method"></param>
        /// <param name="metadatas"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 获取属性的拦截器类型列表。
        /// </summary>
        /// <param name="property"></param>
        /// <param name="metadatas"></param>
        /// <returns></returns>
        private List<ITypeSymbol> GetInterceptorTypes(IPropertySymbol property, InterceptorMetadata metadatas)
        {
            var interceptors = new List<ITypeSymbol>();

            foreach (var type in metadatas.InterceptorTypes)
            {
                if (type.Interfaces.Any(t => t.Name == "IInterceptor"))
                {
                    interceptors.Add(type);
                }
            }

            return interceptors;
        }

        /// <summary>
        /// 获取方法的返回类型。
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        private string GetMethodReturnType(IMethodSymbol symbol)
        {
            if (symbol.ReturnsVoid)
            {
                return "void";
            }

            return symbol.ReturnType.ToDisplayString();
        }

        /// <summary>
        /// 获取属性类型。
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        private string GetPropertyType(IPropertySymbol symbol)
        {
            return symbol.Type.ToDisplayString();
        }

        /// <summary>
        /// 获取异步方法的返回类型，带 Task 或 ValueTask。
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 是否异步方法。
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        private bool IsAsyncMethod(IMethodSymbol symbol)
        {
            if (symbol.ReturnType.Name == "Task" ||
                symbol.ReturnType.Name == "ValueTask")
            {
                return true;
            }

            return false;
        }
    }
}
