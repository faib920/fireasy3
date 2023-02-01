// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace Fireasy.Data
{
    /// <summary>
    /// <see cref="DbConnection"/> 扩展类。
    /// </summary>
    public static class DbConnectionExtensions
    {
        /// <summary>
        /// 尝试打开数据库链接。
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<DbConnection> TryOpenAsync(this DbConnection connection, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            await new ConnectionStateManager(connection).TryOpenAsync(cancellationToken);

            return connection;
        }

        /// <summary>
        /// 尝试关闭数据库链接。
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="allowClose"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<DbConnection> TryCloseAsync(this DbConnection connection, bool allowClose = true, CancellationToken cancellationToken = default)
        {
            if (!allowClose)
            {
                return connection;
            }

            await new ConnectionStateManager(connection).TryOpenAsync(cancellationToken);

            return connection;
        }
    }
}
