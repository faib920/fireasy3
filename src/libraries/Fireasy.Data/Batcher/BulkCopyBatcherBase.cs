// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Fireasy.Data.Syntax;
using System.Data.Common;

namespace Fireasy.Data.Batcher
{
    /// <summary>
    /// 一个使用 <see cref="IBulkCopyProvider"/> 优先处理的抽象类。
    /// </summary>
    public abstract class BulkCopyBatcherBase : BatcherBase, IBatcherProvider
    {
        /// <summary>
        /// 将 <see cref="DataTable"/> 的数据批量插入到数据库中。
        /// </summary>
        /// <param name="database">提供给当前插件的 <see cref="IDatabase"/> 对象。</param>
        /// <param name="dataTable">要批量插入的 <see cref="DataTable"/>。</param>
        /// <param name="batchSize">每批次写入的数据量。</param>
        /// <param name="completePercentage">已完成百分比的通知方法。</param>
        /// <param name="cancellationToken">取消操作的通知。</param>
        public async Task InsertAsync(IDatabase database, DataTable dataTable, int batchSize = 1000, Action<int>? completePercentage = null, CancellationToken cancellationToken = default)
        {
            if (!BatcherChecker.CheckDataTable(dataTable))
            {
                return;
            }

            cancellationToken.ThrowIfCancellationRequested();

            var connection = database.CreateConnection();
            await using var scope = await connection.OpenScopeAsync(cancellationToken);

            var syntax = database.Provider.GetService<ISyntaxProvider>();
            var tableName = syntax!.Delimit(dataTable.TableName);

            var bulkCopyProvider = database.Provider.GetService<IBulkCopyProvider>();
            if (bulkCopyProvider != null)
            {
                using (bulkCopyProvider)
                {
                    bulkCopyProvider.Initialize(connection, database.Transaction, tableName, batchSize);

                    for (int i = 0, n = dataTable.Columns.Count; i < n; i++)
                    {
                        bulkCopyProvider.AddColumnMapping(i, dataTable.Columns[i].ColumnName);
                    }

                    await bulkCopyProvider.WriteToServerAsync(dataTable, cancellationToken);
                }
            }
            else
            {
                var mapping = GetNameTypeMapping(dataTable);

                await InsertUseSQLAsync(database, dataTable.Rows, tableName, mapping, (map, command, r, item) => MapDataRow(database.Provider, map, (DataRow)item, r, command.Parameters), batchSize, completePercentage, cancellationToken);
            }
        }

        /// <summary>
        /// 将一个 <see cref="IList"/> 批量插入到数据库中。 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="database">提供给当前插件的 <see cref="IDatabase"/> 对象。</param>
        /// <param name="list">要写入的数据列表。</param>
        /// <param name="tableName">要写入的数据表的名称。</param>
        /// <param name="batchSize">每批次写入的数据量。</param>
        /// <param name="completePercentage">已完成百分比的通知方法。</param>
        /// <param name="cancellationToken">取消操作的通知。</param>
        public async Task InsertAsync<T>(IDatabase database, IEnumerable<T> list, string tableName, int batchSize = 1000, Action<int>? completePercentage = null, CancellationToken cancellationToken = default)
        {
            if (!BatcherChecker.CheckList(list, tableName))
            {
                return;
            }

            cancellationToken.ThrowIfCancellationRequested();

            var connection = database.CreateConnection();
            await using var scope = await connection.OpenScopeAsync(cancellationToken);

            var bulkCopyProvider = database.Provider.GetService<IBulkCopyProvider>();
            if (bulkCopyProvider != null)
            {
                using (bulkCopyProvider)
                {
                    bulkCopyProvider.Initialize(connection, database.Transaction, tableName, batchSize);

                    using var reader = new EnumerableBatchReader<T>(list, (i, n) => bulkCopyProvider.AddColumnMapping(i, n));
                    await bulkCopyProvider.WriteToServerAsync(reader, cancellationToken);
                }
            }
            else
            {
                await InsertUseSQLAsync(database, ToCollection(list), tableName, null, (map, command, r, item) => MapListItem<T>(database.Provider, map, (T)item, r, command.Parameters), batchSize, completePercentage, cancellationToken);
            }
        }

        /// <summary>
        /// 将 <paramref name="reader"/> 中的数据流批量复制到数据库中。
        /// </summary>
        /// <param name="database">提供给当前插件的 <see cref="IDatabase"/> 对象。</param>
        /// <param name="reader">源数据读取器。</param>
        /// <param name="tableName">要写入的数据表的名称。</param>
        /// <param name="batchSize">每批次写入的数据量。</param>
        /// <param name="completePercentage">已完成百分比的通知方法。</param>
        /// <param name="cancellationToken">取消操作的通知。</param>
        public async Task InsertAsync(IDatabase database, IDataReader reader, string tableName, int batchSize = 1000, Action<int>? completePercentage = null, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 使用 SQL 批量插入集合中的数据。
        /// </summary>
        /// <param name="database">当前的 <see cref="IDatabase"/> 对象。</param>
        /// <param name="collection">要插入的数据的集合。</param>
        /// <param name="tableName">表的名称。</param>
        /// <param name="mapping">名称和类型的映射字典。</param>
        /// <param name="valueFunc">取值函数。</param>
        /// <param name="batchSize">每批次写入的数据量。</param>
        /// <param name="completePercentage">已完成百分比的通知方法。</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected abstract Task InsertUseSQLAsync(IDatabase database, ICollection collection, string tableName, IList<PropertyFieldMapping> mapping, Func<IList<PropertyFieldMapping>, DbCommand, int, object, string> valueFunc, int batchSize, Action<int> completePercentage, CancellationToken cancellationToken = default);
    }
}
