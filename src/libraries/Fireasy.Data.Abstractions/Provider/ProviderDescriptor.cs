// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Fireasy.Data.Provider
{
    /// <summary>
    /// 提供者的描述。
    /// </summary>
    public struct ProviderDescriptor
    {
        /// <summary>
        /// 初始化 <see cref="ProviderDescriptor"/> 结构的新实例。
        /// </summary>
        /// <param name="alais">别名。</param>
        /// <param name="description">描述。</param>
        /// <param name="providerType">提供者类型。</param>
        public ProviderDescriptor(string alais, string description, Type providerType)
        {
            Alais = alais;
            Description = description;
            ProviderType = providerType;
        }

        /// <summary>
        /// 获取别名。
        /// </summary>
        public string Alais { get; }

        /// <summary>
        /// 获取描述。
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// 获取提供者类型。
        /// </summary>
        public Type ProviderType { get; }
    }
}
