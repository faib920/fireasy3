// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Fireasy.Common.Extensions;
using Fireasy.Data.Converter;
using Fireasy.Data.Extensions;
using Fireasy.Data.Syntax;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;

namespace Fireasy.Data.Batcher
{
    /// <summary>
    /// 为 Oracle 提供的用于批量操作的方法。无法继承此类。
    /// </summary>
    public sealed class OracleBatcher : BulkCopyBatcherBase
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
            //Oracle.DataAccess将每一列的数据构造成一个数组，然后使用参数进行插入
            try
            {
                await database.Connection!.OpenAsync(cancellationToken);
                using var command = database.Provider.CreateCommand(database.Connection, database.Transaction, null);
                var syntax = database.Provider.GetService<ISyntaxProvider>();
                var valueConvertManager = database.GetService<IValueConvertManager>();

                var sql = string.Format("INSERT INTO {0}({1}) VALUES({2})",
                    syntax.Delimit(tableName),
                    string.Join(",", mapping.Select(s => syntax.Delimit(s.FieldName))), string.Join(",", mapping.Select(s => syntax.ParameterPrefix + s.FieldName)));

                command.CommandText = sql;

                var length = Math.Min(batchSize, collection.Count);
                var count = collection.Count;
                var data = InitArrayData(mapping.Count, length);
                SetArrayBindCount(command, length);

                BatchSplitData(collection, batchSize,
                    (index, batch, item) =>
                        {
                            if (mapping == null)
                            {
                                mapping = GetNameTypeMapping(item);
                            }

                            FillArrayData(valueConvertManager, mapping, item, data, batch);
                        },
                    async (index, batch, surplus, lastBatch) =>
                        {
                            AddOrReplayParameters(syntax, mapping, command.Parameters, data,
                                () => database.Provider.DbProviderFactory.CreateParameter());

                            await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
                            completePercentage?.Invoke((int)(((index + 1.0) / count) * 100));

                            if (!lastBatch)
                            {
                                length = Math.Min(batchSize, surplus);
                                data = InitArrayData(mapping.Count, length);
                                SetArrayBindCount(command, length);
                            }
                        }, cancellationToken);
            }
            catch (Exception exp)
            {
                throw new BatcherException(collection, exp);
            }
        }

        /// <summary>
        /// 设置ArrayBindCount属性。
        /// </summary>
        /// <param name="command"></param>
        /// <param name="batchSize"></param>
        private void SetArrayBindCount(DbCommand command, int batchSize)
        {
            command.GetType().GetProperty("ArrayBindCount")?.SetValue(command, batchSize, null);
        }

        /// <summary>
        /// 创建一个二维数组。
        /// </summary>
        /// <param name="columns">字段的个数。</param>
        /// <param name="length">元素的个数。</param>
        /// <returns></returns>
        private object[][] InitArrayData(int columns, int length)
        {
            var data = new object[columns][];
            for (var i = 0; i < columns; i++)
            {
                data[i] = new object[length];
            }

            return data;
        }

        /// <summary>
        /// 使用当前的记录填充数组。
        /// </summary>
        /// <param name="valueConvertManager">值转换管理器。</param>
        /// <param name="mappings">名称和类型的映射字典。</param>
        /// <param name="item">当前的数据项。</param>
        /// <param name="data">数组的数组。</param>
        /// <param name="batch">当前批次中的索引。</param>
        private void FillArrayData(IValueConvertManager? valueConvertManager, IEnumerable<PropertyFieldMapping> mappings, object item, object[][] data, int batch)
        {
            mappings.ForEach((m, i) =>
            {
                var value = m.ValueFunc(item);
                if (value != null)
                {
                    var converter = valueConvertManager?.GetConverter(m.PropertyType);
                    if (converter != null)
                    {
                        value = converter.ConvertTo(value, m.FieldType);
                    }
                }

                data[i][batch] = value;
            });
        }

        /// <summary>
        /// 添加或替换集合中的参数。
        /// </summary>
        /// <param name="syntax"></param>
        /// <param name="mapping">名称和类型的映射字典。</param>
        /// <param name="parameters"><see cref="DbCommand"/> 中的参数集合。</param>
        /// <param name="data">数组的数组。</param>
        /// <param name="parFunc">创建 <see cref="DbParameter"/> 对象的函数。</param>
        private void AddOrReplayParameters(ISyntaxProvider syntax, IEnumerable<PropertyFieldMapping> mapping, DbParameterCollection parameters, object[][] data, Func<DbParameter> parFunc)
        {
            mapping.ForEach((m, i) =>
            {
                if (parameters.Contains(m.FieldName))
                {
                    parameters[m.FieldName].Value = data[i];
                }
                else
                {
                    var parameter = parFunc();
                    parameter.ParameterName = m.FieldName;
                    parameter.Direction = ParameterDirection.Input;
                    parameter.DbType = m.FieldType;
                    parameter.Value = data[i];
                    parameters.Add(parameter);
                }
            });
        }
    }
}
