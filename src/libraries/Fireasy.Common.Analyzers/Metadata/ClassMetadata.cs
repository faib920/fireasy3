// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Fireasy.Common.Analyzers.Metadata
{
    /// <summary>
    /// 类的元数据。
    /// </summary>
    public class ClassMetadata
    {
        /// <summary>
        /// 初始化 <see cref="ClassMetadata"/> 类的新实例。
        /// </summary>
        /// <param name="type">类型符号。</param>
        public ClassMetadata(ITypeSymbol type)
        {
            Type = type;
        }

        /// <summary>
        /// 获取类型符号。
        /// </summary>
        public ITypeSymbol Type { get; }

        /// <summary>
        /// 获取命名空间。
        /// </summary>
        public string Namespace => Type.ContainingNamespace.ToDisplayString();

        /// <summary>
        /// 获取类型的全名。
        /// </summary>
        public string TypeFullName => Type.ToDisplayString();

        /// <summary>
        /// 获取代理类的名称。
        /// </summary>
        public string ProxyTypeName => $"{Type.Name}_proxy_";

        /// <summary>
        /// 获取代理类的全名。
        /// </summary>
        public string ProxyTypeFullName => $"{Namespace}.{ProxyTypeName}";

        /// <summary>
        /// 获取源代码名称。
        /// </summary>
        public string SourceCodeName => Type.ToDisplayString().Replace(".", "_") + ".cs";

        /// <summary>
        /// 获取构造函数列表。
        /// </summary>
        public List<IMethodSymbol> Constructors { get; } = new();

        /// <summary>
        /// 获取可拦截的方法。
        /// </summary>
        public Dictionary<IMethodSymbol, InterceptorMetadata> Methods { get; } = new();

        /// <summary>
        /// 获取可拦截的属性。
        /// </summary>
        public Dictionary<IPropertySymbol, InterceptorMetadata> Properties { get; } = new();

        /// <summary>
        /// 获取引用的命名空间列表。
        /// </summary>
        public List<string> Usings { get; } = new();

        /// <summary>
        /// 添加可拦截的成员。
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="metadata"></param>
        public void AddMember(ISymbol symbol, InterceptorMetadata metadata)
        {
            if (symbol is IMethodSymbol method)
            {
                Methods.Add(method, metadata);
            }
            else if (symbol is IPropertySymbol property && property.Parameters.Count() == 0) //忽略索引器
            {
                Properties.Add(property, metadata);
            }
        }

        /// <summary>
        /// 添加构造方法。
        /// </summary>
        /// <param name="symbol"></param>
        public void AddConstructor(IMethodSymbol symbol)
        {
            Constructors.Add(symbol);
        }

        /// <summary>
        /// 添加引用的命名空间列表。
        /// </summary>
        /// <param name="usings"></param>
        public void AddUsings(IEnumerable<string> usings)
        {
            Usings.AddRange(usings);
        }

        /// <summary>
        /// 若有可拦截的方法或属性，则此元数据有效。
        /// </summary>
        public bool IsValid => Methods.Any() || Properties.Any();
    }
}
