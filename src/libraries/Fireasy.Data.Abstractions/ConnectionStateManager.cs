// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Microsoft.Extensions.Logging;
using System.Data.Common;

namespace Fireasy.Data
{
    /// <summary>
    /// <see cref="DbConnection"/> 对象的状态管理器。
    /// </summary>
    public sealed class ConnectionStateManager : DisposableBase
    {
        private readonly DbConnection _connection;
        private ConnectionState _state;

        /// <summary>
        /// 初始化 <see cref="ConnectionStateManager"/> 类的新实例。
        /// </summary>
        /// <param name="connection">一个 <see cref="DbConnection"/> 实例。</param>
        public ConnectionStateManager(DbConnection connection)
        {
            _connection = connection;
        }

        /// <summary>
        /// 获取或设置 <see cref="ILogger"/>
        /// </summary>
        public ILogger<ConnectionStateManager>? Logger { get; set; }

        /// <summary>
        /// 尝试打开链接。
        /// </summary>
        /// <returns></returns>
        public ConnectionStateManager TryOpen()
        {
            _state = _connection.State;
            if (_state != ConnectionState.Open)
            {
                _connection.Open();
            }
            else if (_state == ConnectionState.Broken)
            {
                _connection.Close();
                _connection.Open();
            }

            return this;
        }

        /// <summary>
        /// 尝试打开链接。
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ConnectionStateManager> TryOpenAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            _state = _connection.State;
            if (_state != ConnectionState.Open)
            {
                await _connection.OpenAsync(cancellationToken);
            }
            else if (_state == ConnectionState.Broken)
            {
#if NETFRAMEWORK || NETSTANDARD2_0
                _connection.Close();
#else
                await _connection.CloseAsync().ConfigureAwait(false);
#endif
                await _connection.OpenAsync(cancellationToken).ConfigureAwait(false);
            }

            return this;
        }

        /// <summary>
        /// 尝试关闭链接。
        /// </summary>
        /// <returns></returns>
        public ConnectionStateManager TryClose()
        {
            if (_state == ConnectionState.Closed || _state == ConnectionState.Broken)
            {
                _connection.Close();
            }

            return this;
        }

        /// <summary>
        /// 异步的，尝试关闭链接。
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ConnectionStateManager> TryCloseAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (_state == ConnectionState.Closed || _state == ConnectionState.Broken)
            {
#if NETFRAMEWORK || NETSTANDARD2_0
                _connection.Close();
#else
                await _connection.CloseAsync().ConfigureAwait(false);
#endif
            }

            return this;
        }

        protected override bool Dispose(bool disposing)
        {
            TryClose();
            return base.Dispose(disposing);
        }

        protected override async ValueTask<bool> DisposeAsync(bool disposing)
        {
            await TryCloseAsync();
            return await base.DisposeAsync(disposing);
        }
    }
}
