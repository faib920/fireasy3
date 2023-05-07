// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Fireasy.Common.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System.Data.Common;

namespace Fireasy.Data.Provider
{
    /// <summary>
    /// 为不同的数据库类型提供创建工厂及插件服务。
    /// </summary>
    public interface IProvider : IServiceProviderAccessor
    {
        /// <summary>
        /// 获取描述实例的名称。
        /// </summary>
        string ProviderName { get; set; }

        /// <summary>
        /// 获取数据库提供者工厂。
        /// </summary>
        DbProviderFactory DbProviderFactory { get; }

        /// <summary>
        /// 获取当前连接的参数。
        /// </summary>
        /// <param name="connectionString">连接字符串对象。</param>
        /// <returns>连接字符串参数对象。</returns>
        ConnectionParameter GetConnectionParameter(ConnectionString? connectionString);

        /// <summary>
        /// 使用参数更新指定的连接。
        /// </summary>
        /// <param name="connectionString">连接字符串对象。</param>
        /// <param name="parameter"></param>
        void UpdateConnectionString(ConnectionString connectionString, ConnectionParameter parameter);

        /// <summary>
        /// 初始化。
        /// </summary>
        /// <param name="context">初始化上下文。</param>
        void Initialize(ProviderInitializeContext context);

        /// <summary>
        /// 获取服务。
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <returns></returns>
        TService? GetService<TService>() where TService : IProviderService;

        /// <summary>
        /// 处理 <see cref="DbConnection"/> 对象。
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        DbConnection PrepareConnection(DbConnection connection);

        /// <summary>
        /// 处理 <see cref="DbCommand"/> 对象。
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        DbCommand PrepareCommand(DbCommand command);

        /// <summary>
        /// 处理 <see cref="DbParameter"/> 对象。
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        DbParameter PrepareParameter(DbParameter parameter);

        /// <summary>
        /// 修正事务隔离级别。
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        IsolationLevel AmendIsolationLevel(IsolationLevel level);
    }
}
