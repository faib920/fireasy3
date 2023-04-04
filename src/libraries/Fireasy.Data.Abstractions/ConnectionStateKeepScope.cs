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
    /// <see cref="DbConnection"/> 状态保持作用域。
    /// </summary>
    public class ConnectionStateKeepScope : AsyncDisposableBase
    {
        private readonly DbConnection _connection;
        private readonly CancellationToken _cancellationToken;

        /// <summary>
        /// 初始化 <see cref="ConnectionStateKeepScope"/> 类的新实例。
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="cancellationToken"></param>
        public ConnectionStateKeepScope(DbConnection connection, CancellationToken cancellationToken)
        {
            _connection = connection;
            _cancellationToken = cancellationToken;
        }

        /// <summary>
        /// 销毁对象。
        /// </summary>
        /// <param name="disposing"></param>
        /// <returns></returns>
        protected override async ValueTask<bool> DisposeAsync(bool disposing)
        {
            await _connection.TryCloseAsync(true, _cancellationToken);
            return true;
        }
    }
}
