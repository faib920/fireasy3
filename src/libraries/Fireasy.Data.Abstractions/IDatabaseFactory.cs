// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Fireasy.Data.Provider;

namespace Fireasy.Data
{
    /// <summary>
    /// <see cref="IDatabase"/> 的创建工厂。
    /// </summary>
    public interface IDatabaseFactory
    {
        /// <summary>
        /// 使用配置实例创建 <see cref="IDatabase"/> 实例。
        /// </summary>
        /// <param name="instanceName">配置实例名称。</param>
        /// <returns></returns>
        IDatabase? CreateDatabase(string? instanceName = null);

        /// <summary>
        /// 使用提供者类型 <typeparamref name="TProvider"/> 及连接串创建 <see cref="IDatabase"/> 实例。
        /// </summary>
        /// <typeparam name="TProvider"></typeparam>
        /// <param name="connectionString">数据库连接字符串。</param>
        /// <returns></returns>
        IDatabase CreateDatabase<TProvider>(string connectionString) where TProvider : IProvider;

        /// <summary>
        /// 使用提供者名称及连接串创建 <see cref="IDatabase"/> 实例。
        /// </summary>
        /// <param name="providerName">提供者名称，可通过 <see cref="IProviderManager.GetSupportedProviderNames"/> 获取。</param>
        /// <param name="connectionString">数据库连接字符串。</param>
        /// <returns></returns>
        IDatabase CreateDatabase(string providerName, string connectionString);
    }
}
