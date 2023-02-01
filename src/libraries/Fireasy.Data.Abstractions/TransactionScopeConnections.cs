// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Fireasy.Common.Collections;
using Fireasy.Common.Extensions;
using Microsoft.Extensions.Logging;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

namespace Fireasy.Data
{
    /// <summary>
    /// 用于管理处于 <see cref="TransactionScope"/> 中的数据库链接。无法继承此类。
    /// </summary>
    public sealed class TransactionScopeConnections
    {
        private static readonly ExtraConcurrentDictionary<Transaction, ExtraConcurrentDictionary<string, DbConnection>> _transConns =
            new ExtraConcurrentDictionary<Transaction, ExtraConcurrentDictionary<string, DbConnection>>();

        /// <summary>
        /// 从分布式事务环境中获取数据库链接对象。
        /// </summary>
        /// <param name="database">数据库对象。</param>
        /// <param name="cancellationToken"></param>
        /// <returns>如果未启用分布式事务，则为 null，否则为对应 <see cref="IDatabase"/> 的数据库链接对象。</returns>
        public static async Task<DbConnection?> GetConnectionAsync(IDatabase database, CancellationToken cancellationToken = default)
        {
            var curTrans = Transaction.Current;

            if (curTrans == null)
            {
                return null;
            }

            var logger = database!.TryGetServiceProvider()?.TryGetService<ILogger>();

            if (!_transConns.TryGetValue(curTrans, out ExtraConcurrentDictionary<string, DbConnection> connDictionary))
            {
                connDictionary = new ExtraConcurrentDictionary<string, DbConnection>();
                _transConns.TryAdd(curTrans, connDictionary);

                logger?.LogInformation($"Transaction registered.");
                curTrans.TransactionCompleted += OnTransactionCompleted;
            }

            var connStr = database.ConnectionString?.ToString();
            if (connStr != null && !connDictionary.TryGetValue(connStr, out DbConnection? connection))
            {
                connection = database.Provider.DbProviderFactory.CreateConnection();
                if (connection != null)
                {
                    connection.ConnectionString = connStr;
                    await connection.TryOpenAsync(cancellationToken);
                    connection.EnlistTransaction(curTrans);
                    connDictionary.TryAdd(connStr, connection);

                    logger?.LogInformation($"DbConnection of '{connStr}' registered.");
                }
            }
            else
            {
                connection = null;
                logger?.LogInformation($"DbConnection get from cache.");
            }

            return connection;
        }

        /// <summary>
        /// 事务完成后，关闭数据库链接。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void OnTransactionCompleted(object sender, TransactionEventArgs e)
        {
            if (!_transConns.TryGetValue(e.Transaction, out ExtraConcurrentDictionary<string, DbConnection> connDictionary))
            {
                return;
            }

            foreach (var connection in connDictionary.Values)
            {
                connection.Dispose();
            }

            _transConns.TryRemove(e.Transaction, out _);
        }
    }
}
