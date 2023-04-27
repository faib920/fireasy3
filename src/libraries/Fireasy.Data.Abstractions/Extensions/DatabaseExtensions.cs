// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Fireasy.Common.Extensions;
using Fireasy.Data.RecordWrapper;
using System.Data.Common;

namespace Fireasy.Data
{
    /// <summary>
    /// <see cref="IDatabase"/> 扩展类。
    /// </summary>
    public static class DatabaseExtensions
    {
        /// <summary>
        /// 执行查询文本并将结果填充到指定的 <see cref="DataTable"/> 对象中。
        /// </summary>
        /// <param name="database"><see cref="IDatabase"/> 实例。</param>
        /// <param name="sql">查询命令。</param>
        /// <param name="tableName"><see cref="DataTable"/> 的名称。</param>
        /// <param name="segment">数据分段对象。</param>
        /// <param name="parameters">查询参数集合。</param>
        /// <returns>一个 <see cref="DataTable"/> 对象。</returns>
        public static Task<DataTable?> ExecuteDataTableAsync(this IDatabase database, string sql, string? tableName = null, IDataSegment? segment = null, ParameterCollection? parameters = null)
        {
            return database.ExecuteDataTableAsync((SqlCommand)sql, tableName, segment, parameters);
        }

        /// <summary>
        /// 执行查询文本并将结果以一个 <see cref="IEnumerable{T}"/> 的序列返回。
        /// </summary>
        /// <typeparam name="T">查询对象类型。</typeparam>
        /// <param name="database"><see cref="IDatabase"/> 实例。</param>
        /// <param name="sql">查询命令。</param>
        /// <param name="segment">数据分段对象。</param>
        /// <param name="parameters">查询参数集合。</param>
        /// <param name="rowMapper">数据行映射器。</param>
        /// <param name="cancellationToken">取消操作的通知。</param>
        /// <returns>一个 <typeparamref name="T"/> 类型的对象的枚举器。</returns>
        public static IAsyncEnumerable<T> ExecuteAsyncEnumerable<T>(this IDatabase database, string sql, IDataSegment? segment = null, ParameterCollection? parameters = null, IDataRowMapper<T>? rowMapper = null, CancellationToken cancellationToken = default)
        {
            return database.ExecuteAsyncEnumerable<T>((SqlCommand)sql, segment, parameters, rowMapper, cancellationToken);
        }

        /// <summary>
        /// 执行查询文本并将结果并返回动态序列。
        /// </summary>
        /// <param name="database"><see cref="IDatabase"/> 实例。</param>
        /// <param name="sql">查询命令。</param>
        /// <param name="segment">数据分段对象。</param>
        /// <param name="parameters">查询参数集合。</param>
        /// <param name="cancellationToken">取消操作的通知。</param>
        /// <returns>一个动态对象的枚举器。</returns>
        public static IAsyncEnumerable<dynamic> ExecuteAsyncEnumerable(this IDatabase database, string sql, IDataSegment? segment = null, ParameterCollection? parameters = null, CancellationToken cancellationToken = default)
        {
            return database.ExecuteAsyncEnumerable((SqlCommand)sql, segment, parameters, cancellationToken);
        }

        /// <summary>
        /// 执行查询文本并将结果以一个 <see cref="IEnumerable{T}"/> 的序列返回。
        /// </summary>
        /// <typeparam name="T">查询对象类型。</typeparam>
        /// <param name="database"><see cref="IDatabase"/> 实例。</param>
        /// <param name="sql">查询命令。</param>
        /// <param name="segment">数据分段对象。</param>
        /// <param name="parameters">查询参数集合。</param>
        /// <param name="rowMapper">数据行映射器。</param>
        /// <param name="cancellationToken">取消操作的通知。</param>
        /// <returns>一个 <typeparamref name="T"/> 类型的对象的枚举器。</returns>
        public static Task<IEnumerable<T>> ExecuteEnumerableAsync<T>(this IDatabase database, string sql, IDataSegment? segment = null, ParameterCollection? parameters = null, IDataRowMapper<T>? rowMapper = null, CancellationToken cancellationToken = default)
        {
            return database.ExecuteEnumerableAsync<T>((SqlCommand)sql, segment, parameters, rowMapper, cancellationToken);
        }

        /// <summary>
        /// 执行查询文本并将结果以一个 <see cref="IEnumerable{T}"/> 的序列返回。
        /// </summary>
        /// <typeparam name="T">查询对象类型。</typeparam>
        /// <param name="database"><see cref="IDatabase"/> 实例。</param>
        /// <param name="sql">查询命令。</param>
        /// <param name="rowMapper">数据映射函数。</param>
        /// <param name="segment">数据分段对象。</param>
        /// <param name="parameters">查询参数集合。</param>
        /// <param name="cancellationToken">取消操作的通知。</param>
        /// <returns>一个 <typeparamref name="T"/> 类型的对象的枚举器。</returns>
        public static Task<IEnumerable<T>> ExecuteEnumerableAsync<T>(this IDatabase database, string sql, Func<IRecordWrapper, IDataReader, T> rowMapper, IDataSegment? segment = null, ParameterCollection? parameters = null, CancellationToken cancellationToken = default)
        {
            return database.ExecuteEnumerableAsync((SqlCommand)sql, rowMapper, segment, parameters, cancellationToken);
        }

        /// <summary>
        /// 执行查询文本并将结果并返回动态序列。
        /// </summary>
        /// <param name="database"><see cref="IDatabase"/> 实例。</param>
        /// <param name="sql">查询命令。</param>
        /// <param name="segment">数据分段对象。</param>
        /// <param name="parameters">查询参数集合。</param>
        /// <param name="cancellationToken">取消操作的通知。</param>
        /// <returns>一个动态对象的枚举器。</returns>
        public static Task<IEnumerable<dynamic>> ExecuteEnumerableAsync(this IDatabase database, string sql, IDataSegment? segment = null, ParameterCollection? parameters = null, CancellationToken cancellationToken = default)
        {
            return database.ExecuteEnumerableAsync((SqlCommand)sql, segment, parameters, cancellationToken);
        }

        /// <summary>
        /// 执行查询文本，返回受影响的记录数。
        /// </summary>
        /// <param name="database"><see cref="IDatabase"/> 实例。</param>
        /// <param name="sql">查询命令。</param>
        /// <param name="parameters">查询参数集合。</param>
        /// <param name="cancellationToken">取消操作的通知。</param>
        /// <returns>所影响的记录数。</returns>
        public static Task<int> ExecuteNonQueryAsync(this IDatabase database, string sql, ParameterCollection? parameters = null, CancellationToken cancellationToken = default)
        {
            return database.ExecuteNonQueryAsync((SqlCommand)sql, parameters, cancellationToken);
        }

        /// <summary>
        /// 执行批处理文本，返回受影响的记录数。
        /// </summary>
        /// <param name="database"><see cref="IDatabase"/> 实例。</param>
        /// <param name="sqls">查询命令。</param>
        /// <param name="parameters">查询参数集合。</param>
        /// <param name="cancellationToken">取消操作的通知。</param>
        /// <returns>所影响的记录数。</returns>
        public static Task<int> ExecuteBatchAsync(this IDatabase database, IEnumerable<string> sqls, ParameterCollection? parameters = null, CancellationToken cancellationToken = default)
        {
            return database.ExecuteBatchAsync(sqls.Select(s => (SqlCommand)s), parameters, cancellationToken);
        }

        /// <summary>
        /// 执行查询文本并返回一个 <see cref="IDataReader"/>。
        /// </summary>
        /// <param name="database"><see cref="IDatabase"/> 实例。</param>
        /// <param name="sql">查询命令。</param>
        /// <param name="segment">数据分段对象。</param>
        /// <param name="parameters">查询参数集合。</param>
        /// <param name="behavior"></param>
        /// <param name="cancellationToken">取消操作的通知。</param>
        /// <returns>一个 <see cref="IDataReader"/> 对象。</returns>
        public static Task<IDataReader> ExecuteReaderAsync(this IDatabase database, string sql, IDataSegment? segment = null, ParameterCollection? parameters = null, CommandBehavior? behavior = null, CancellationToken cancellationToken = default)
        {
            return database.ExecuteReaderAsync((SqlCommand)sql, segment, parameters, behavior, cancellationToken);
        }

        /// <summary>
        /// 执行查询文本，并返回第一行的第一列。
        /// </summary>
        /// <param name="database"><see cref="IDatabase"/> 实例。</param>
        /// <param name="sql">查询命令。</param>
        /// <param name="parameters">查询参数集合。</param>
        /// <param name="cancellationToken">取消操作的通知。</param>
        /// <returns>第一行的第一列数据。</returns>
        public static Task<object?> ExecuteScalarAsync(this IDatabase database, string sql, ParameterCollection? parameters = null, CancellationToken cancellationToken = default)
        {
            return database.ExecuteScalarAsync((SqlCommand)sql, parameters, cancellationToken);
        }

        /// <summary>
        /// 执行查询文本，并返回第一行的第一列。
        /// </summary>
        /// <param name="database"><see cref="IDatabase"/> 实例。</param>
        /// <param name="sql">查询命令。</param>
        /// <param name="parameters">查询参数集合。</param>
        /// <param name="cancellationToken">取消操作的通知。</param>
        /// <returns>第一行的第一列数据。</returns>
        public static Task<T?> ExecuteScalarAsync<T>(this IDatabase database, string sql, ParameterCollection? parameters = null, CancellationToken cancellationToken = default)
        {
            return database.ExecuteScalarAsync<T>((SqlCommand)sql, parameters, cancellationToken);
        }

        /// <summary>
        /// 执行查询文本并将结果填充到指定的 <see cref="DataSet"/> 对象中。
        /// </summary>
        /// <param name="database"><see cref="IDatabase"/> 实例。</param>
        /// <param name="dataSet">要填充的 <see cref="DataSet"/>。</param>
        /// <param name="sql">查询命令。</param>
        /// <param name="tableName">表的名称，多个表名称使用逗号分隔。</param>
        /// <param name="segment">数据分段对象。</param>
        /// <param name="parameters">查询参数集合。</param>
        public static Task FillDataSetAsync(this IDatabase database, DataSet dataSet, string sql, string? tableName = null, IDataSegment? segment = null, ParameterCollection? parameters = null)
        {
            return database.FillDataSetAsync(dataSet, (SqlCommand)sql, tableName, segment, parameters);
        }

        /// <summary>
        /// 开启一个事务，用来执行一组方法。
        /// </summary>
        /// <param name="database">当前的 <see cref="IDatabase"/>。</param>
        /// <param name="actExec">要执行的一组方法。</param>
        /// <param name="funCatch">捕获异常，如果返回 true 则仍抛出异常。</param>
        /// <param name="level">事务的级别。</param>
        public static async Task WithTransactionAsync(this IDatabase database, Func<IDatabase, Task> actExec, Func<IDatabase, Exception, Task<bool>>? funCatch = null, IsolationLevel level = IsolationLevel.ReadCommitted)
        {
            if (actExec == null)
            {
                return;
            }

            await database.BeginTransactionAsync(level);
            try
            {
                await actExec(database);
                await database.CommitTransactionAsync();
            }
            catch (Exception ex)
            {
                await database.RollbackTransactionAsync();
                if (funCatch != null)
                {
                    if (await funCatch(database, ex))
                    {
                        throw;
                    }
                }
                else
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// 创建一个新的 <see cref="DbConnection"/> 对象。
        /// </summary>
        /// <param name="database"><see cref="IDatabase"/> 对象。</param>
        /// <param name="mode">分布式模式。</param>
        /// <returns><paramref name="database"/> 创建的 <see cref="DbConnection"/> 对象。</returns>
        public static DbConnection? CreateConnection(this IDatabase database, DistributedMode mode = DistributedMode.Master)
        {
            Guard.ArgumentNull(database, nameof(database));

            ConnectionString? connStr = null;

            if (mode == DistributedMode.Slave && database is IDistributedDatabase distDb &&
                distDb.DistributedConnectionStrings != null && distDb.DistributedConnectionStrings.Count > 0)
            {
                var serviceProvider = database.TryGetServiceProvider();
                var manager = serviceProvider?.TryGetService<IDistributedConnectionManager>();

                connStr = manager?.GetConnection(distDb);
            }

            var connection = database!.Provider.DbProviderFactory.CreateConnection();

            var encryptor = database.GetService<IConnectionStringEncryptor>();

            connStr ??= database.ConnectionString;
            if (encryptor != null && connStr != null)
            {
                connStr = encryptor.Decrypt(connStr);
            }

            if (connection != null)
            {
                connection.ConnectionString = connStr?.ToString();
            }

            return connection;
        }
    }
}
