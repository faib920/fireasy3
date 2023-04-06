// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Fireasy.Data.Extensions;
using Fireasy.Data.Syntax;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;

namespace Fireasy.Data.Batcher
{
    /// <summary>
    /// 为 SQLite 提供的用于批量操作的方法。无法继承此类。
    /// </summary>
    public sealed class SQLiteBatcher : BulkCopyBatcherBase
    {
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
        [SuppressMessage("Sercurity", "CA2100")]
        protected override async Task InsertUseSQLAsync(IDatabase database, ICollection collection, string tableName, IList<PropertyFieldMapping> mapping, Func<IList<PropertyFieldMapping>, DbCommand, int, object, string> valueFunc, int batchSize, Action<int> completePercentage, CancellationToken cancellationToken = default)
        {
            //SQLite使用如 insert into table(f1, f2) values ('a1', 'b1'),('a2', 'b2'),('a3', 'b3') 方式批量插入
            //但一次只能插入500条

            batchSize = 500;

            try
            {
                await database.BeginTransactionAsync();

                using (var command = database.Provider.CreateCommand(database.Connection, database.Transaction, null))
                {
                    var syntax = database.Provider.GetService<ISyntaxProvider>();
                    var valueSeg = new List<string>(batchSize);
                    var count = collection.Count;

                    BatchSplitData(collection, batchSize,
                        (index, batch, item) =>
                            {
                                if (mapping == null)
                                {
                                    mapping = GetNameTypeMapping(item);
                                }

                                valueSeg.Add(string.Format("({0})", valueFunc(mapping, command, batch, item)));
                            },
                        async (index, batch, surplus, lastBatch) =>
                            {
                                var sql = string.Format("INSERT INTO {0}({1}) VALUES {2}",
                                    syntax.Delimit(tableName),
                                    string.Join(",", mapping.Select(s => syntax.Delimit(s.FieldName))), string.Join(",", valueSeg));

                                command.CommandText = sql;
                                await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
                                valueSeg.Clear();
                                command.Parameters.Clear();
                                completePercentage?.Invoke((int)(((index + 1.0) / count) * 100));
                            }, cancellationToken);
                }

                await database.CommitTransactionAsync();
            }
            catch (Exception exp)
            {
                await database.RollbackTransactionAsync();

                throw new BatcherException(collection, exp);
            }
        }
    }
}
