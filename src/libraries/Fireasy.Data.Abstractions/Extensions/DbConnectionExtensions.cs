// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using System.Data.Common;

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
        /// <returns></returns>
        public static DbConnection TryOpen(this DbConnection connection)
        {
            Guard.ArgumentNull(connection, nameof(connection));

            var _state = connection.State;
            if (_state != ConnectionState.Open)
            {
                connection.Open();
            }
            else if (_state == ConnectionState.Broken)
            {
                connection.Close();
                connection.OpenAsync();
            }

            return connection;
        }

        /// <summary>
        /// 尝试打开数据库链接。
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<DbConnection> TryOpenAsync(this DbConnection connection, CancellationToken cancellationToken = default)
        {
            Guard.ArgumentNull(connection, nameof(connection));

            cancellationToken.ThrowIfCancellationRequested();

            var _state = connection.State;
            if (_state != ConnectionState.Open)
            {
                await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
            }
            else if (_state == ConnectionState.Broken)
            {
#if NETFRAMEWORK || NETSTANDARD2_0
                connection.Close();
#else
                await connection.CloseAsync().ConfigureAwait(false);
#endif
                await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
            }

            return connection;
        }

        /// <summary>
        /// 打开数据库链接，并返回一个作用域，作用域销毁时自动关闭数据库链接。
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<ConnectionStateKeepScope> OpenScopeAsync(this DbConnection connection, CancellationToken cancellationToken = default)
        {
            return new ConnectionStateKeepScope(await connection.TryOpenAsync(cancellationToken), cancellationToken);
        }

        /// <summary>
        /// 尝试关闭数据库链接。
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="closeable">是否可关闭。</param>
        /// <returns></returns>
        public static DbConnection TryClose(this DbConnection connection, bool closeable = true)
        {
            Guard.ArgumentNull(connection, nameof(connection));

            if (!closeable)
            {
                return connection;
            }

            var _state = connection.State;
            if (_state == ConnectionState.Open || _state == ConnectionState.Broken)
            {
                connection.Close();
            }

            return connection;
        }

        /// <summary>
        /// 尝试关闭数据库链接。
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="closeable">是否可关闭。</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<DbConnection> TryCloseAsync(this DbConnection connection, bool closeable = true, CancellationToken cancellationToken = default)
        {
            Guard.ArgumentNull(connection, nameof(connection));

            if (!closeable)
            {
                return connection;
            }

            cancellationToken.ThrowIfCancellationRequested();

            var _state = connection.State;
            if (_state == ConnectionState.Open || _state == ConnectionState.Broken)
            {
#if NETFRAMEWORK || NETSTANDARD2_0
                connection.Close();
#else
                await connection.CloseAsync().ConfigureAwait(false);
#endif
            }

            return connection;
        }
    }
}
