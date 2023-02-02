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
    /// 提供对 <see cref="IProvider"/> 的管理。
    /// </summary>
    public interface IProviderManager
    {
        /// <summary>
        /// 根据 <paramref name="providerName"/> 获取对应的 <see cref="IProvider"/> 对象。
        /// </summary>
        /// <param name="providerName">提供者名称。</param>
        /// <returns></returns>
        IProvider? GetDefinedProvider(string providerName);

        /// <summary>
        /// 获取注册的所有数据库提供者名称。
        /// </summary>
        /// <returns></returns>
        string[] GetSupportedProviderNames();
    }
}
