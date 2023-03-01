// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Fireasy.Common.Analyzers.ServiceDiscover.Metadata
{
    /// <summary>
    /// 类的元数据。
    /// </summary>
    public class ClassMetadata
    {
        /// <summary>
        /// 初始化 <see cref="ClassMetadata"/> 类的新实例。
        /// </summary>
        /// <param name="implementationType">实现类的类型。</param>
        /// <param name="lifetime">生命周期。</param>
        public ClassMetadata(ITypeSymbol implementationType, string lifetime)
        {
            ImplementationType = implementationType;
            Lifetime = lifetime;
        }

        /// <summary>
        /// 获取实现类的类型。
        /// </summary>
        public ITypeSymbol ImplementationType { get; }

        /// <summary>
        /// 获取服务类的类型列表。
        /// </summary>
        public List<ITypeSymbol> ServiceTypes { get; } = new();

        /// <summary>
        /// 获取生命周期。
        /// </summary>
        public string Lifetime { get; }

        /// <summary>
        /// 添加服务类型。
        /// </summary>
        /// <param name="serviceTypes">服务类型列表。</param>
        /// <returns></returns>
        public ClassMetadata AddServiceTypes(IEnumerable<ITypeSymbol> serviceTypes)
        {
            ServiceTypes.AddRange(serviceTypes);

            return this;
        }
    }
}
