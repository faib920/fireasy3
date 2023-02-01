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
        /// 使用配置创建 <see cref="IDatabase"/> 实例。
        /// </summary>
        /// <param name="instanceName">配置实例名称。</param>
        /// <returns></returns>
        IDatabase? CreateDatabase(string? instanceName = null);

        /// <summary>
        /// 创建 <see cref="IDatabase"/> 实例。
        /// </summary>
        /// <typeparam name="TProvider"></typeparam>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        IDatabase CreateDatabase<TProvider>(string connectionString) where TProvider : IProvider;
    }
}
