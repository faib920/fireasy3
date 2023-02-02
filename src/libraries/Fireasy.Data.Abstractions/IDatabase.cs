﻿// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Fireasy.Data.Provider;
using Fireasy.Data.RecordWrapper;
using System.Data.Common;

namespace Fireasy.Data
{
    /// <summary>
    /// 提供一组数据库的基本操作方法。
    /// </summary>
    public interface IDatabase : IDisposable, IServiceProvider
    {
        /// <summary>
        /// 获取或设置数据库连接字符串。
        /// </summary>
        ConnectionString? ConnectionString { get; set; }

        /// <summary>
        /// 获取数据库提供者。
        /// </summary>
        IProvider Provider { get; }

        /// <summary>
        /// 获取或设置超时时间。
        /// </summary>
        int Timeout { get; set; }

        /// <summary>
        /// 获取当前数据库事务。
        /// </summary>
        DbTransaction? Transaction { get; }

        /// <summary>
        /// 获取当前数据库链接。
        /// </summary>
        DbConnection? Connection { get; }

        /// <summary>
        /// 使用指定锁定行为启动一个数据库事务。
        /// </summary>
        /// <param name="level">事务的锁定行为。</param>
        /// <param name="cancellationToken">取消操作的通知。</param>
        /// <returns>如果当前实例首次启动事务，则为 true，否则为 false。</returns>
        Task<bool> BeginTransactionAsync(IsolationLevel level = IsolationLevel.ReadCommitted, CancellationToken cancellationToken = default);

        /// <summary>
        /// 如果与方法 BeginTransaction 匹配，则提交数据库事务。
        /// </summary>
        /// <param name="cancellationToken">取消操作的通知。</param>
        /// <returns>成功提交事务则为 true，否则为 false。</returns>
        Task<bool> CommitTransactionAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// 如果与方法 BeginTransaction 匹配，则回滚数据库事务。
        /// </summary>
        /// <param name="cancellationToken">取消操作的通知。</param>
        /// <returns>成功回滚事务则为 true，否则为 false。</returns>
        Task<bool> RollbackTransactionAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// 执行查询文本并将结果填充到指定的 <see cref="DataTable"/> 对象中。
        /// </summary>
        /// <param name="queryCommand">查询命令。</param>
        /// <param name="tableName"><see cref="DataTable"/> 的名称。</param>
        /// <param name="segment">数据分段对象。</param>
        /// <param name="parameters">查询参数集合。</param>
        /// <returns>一个 <see cref="DataTable"/> 对象。</returns>
        Task<DataTable?> ExecuteDataTableAsync(IQueryCommand queryCommand, string? tableName = null, IDataSegment? segment = null, ParameterCollection? parameters = null);

        /// <summary>
        /// 执行查询文本并将结果以一个 <see cref="IEnumerable{T}"/> 的序列返回。
        /// </summary>
        /// <typeparam name="T">查询对象类型。</typeparam>
        /// <param name="queryCommand">查询命令。</param>
        /// <param name="segment">数据分段对象。</param>
        /// <param name="parameters">查询参数集合。</param>
        /// <param name="rowMapper">数据行映射器。</param>
        /// <param name="cancellationToken">取消操作的通知。</param>
        /// <returns>一个 <typeparamref name="T"/> 类型的对象的枚举器。</returns>
        IAsyncEnumerable<T> ExecuteAsyncEnumerable<T>(IQueryCommand queryCommand, IDataSegment? segment = null, ParameterCollection? parameters = null, IDataRowMapper<T>? rowMapper = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// 执行查询文本并将结果并返回动态序列。
        /// </summary>
        /// <param name="queryCommand">查询命令。</param>
        /// <param name="segment">数据分段对象。</param>
        /// <param name="parameters">查询参数集合。</param>
        /// <param name="cancellationToken">取消操作的通知。</param>
        /// <returns>一个动态对象的枚举器。</returns>
        IAsyncEnumerable<dynamic> ExecuteAsyncEnumerable(IQueryCommand queryCommand, IDataSegment? segment = null, ParameterCollection? parameters = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// 执行查询文本并将结果以一个 <see cref="IEnumerable{T}"/> 的序列返回。
        /// </summary>
        /// <typeparam name="T">查询对象类型。</typeparam>
        /// <param name="queryCommand">查询命令。</param>
        /// <param name="segment">数据分段对象。</param>
        /// <param name="parameters">查询参数集合。</param>
        /// <param name="rowMapper">数据行映射器。</param>
        /// <param name="cancellationToken">取消操作的通知。</param>
        /// <returns>一个 <typeparamref name="T"/> 类型的对象的枚举器。</returns>
        Task<IEnumerable<T>> ExecuteEnumerableAsync<T>(IQueryCommand queryCommand, IDataSegment? segment = null, ParameterCollection? parameters = null, IDataRowMapper<T>? rowMapper = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// 执行查询文本并将结果以一个 <see cref="IEnumerable{T}"/> 的序列返回。
        /// </summary>
        /// <typeparam name="T">查询对象类型。</typeparam>
        /// <param name="queryCommand">查询命令。</param>
        /// <param name="rowMapper">数据映射函数。</param>
        /// <param name="segment">数据分段对象。</param>
        /// <param name="parameters">查询参数集合。</param>
        /// <param name="cancellationToken">取消操作的通知。</param>
        /// <returns>一个 <typeparamref name="T"/> 类型的对象的枚举器。</returns>
        Task<IEnumerable<T>> ExecuteEnumerableAsync<T>(IQueryCommand queryCommand, Func<IRecordWrapper, IDataReader, T> rowMapper, IDataSegment? segment = null, ParameterCollection? parameters = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// 执行查询文本并将结果并返回动态序列。
        /// </summary>
        /// <param name="queryCommand">查询命令。</param>
        /// <param name="segment">数据分段对象。</param>
        /// <param name="parameters">查询参数集合。</param>
        /// <param name="cancellationToken">取消操作的通知。</param>
        /// <returns>一个动态对象的枚举器。</returns>
        Task<IEnumerable<dynamic>> ExecuteEnumerableAsync(IQueryCommand queryCommand, IDataSegment? segment = null, ParameterCollection? parameters = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// 执行查询文本，返回受影响的记录数。
        /// </summary>
        /// <param name="queryCommand">查询命令。</param>
        /// <param name="parameters">查询参数集合。</param>
        /// <param name="cancellationToken">取消操作的通知。</param>
        /// <returns>所影响的记录数。</returns>
        Task<int> ExecuteNonQueryAsync(IQueryCommand queryCommand, ParameterCollection? parameters = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// 执行批处理文本，返回受影响的记录数。
        /// </summary>
        /// <param name="queryCommands">查询命令。</param>
        /// <param name="parameters">查询参数集合。</param>
        /// <param name="cancellationToken">取消操作的通知。</param>
        /// <returns>所影响的记录数。</returns>
        Task<int> ExecuteBatchAsync(IEnumerable<IQueryCommand> queryCommands, ParameterCollection? parameters = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// 执行查询文本并返回一个 <see cref="IDataReader"/>。
        /// </summary>
        /// <param name="queryCommand">查询命令。</param>
        /// <param name="segment">数据分段对象。</param>
        /// <param name="parameters">查询参数集合。</param>
        /// <param name="behavior"></param>
        /// <param name="cancellationToken">取消操作的通知。</param>
        /// <returns>一个 <see cref="IDataReader"/> 对象。</returns>
        Task<IDataReader> ExecuteReaderAsync(IQueryCommand queryCommand, IDataSegment? segment = null, ParameterCollection? parameters = null, CommandBehavior? behavior = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// 执行查询文本，并返回第一行的第一列。
        /// </summary>
        /// <param name="queryCommand">查询命令。</param>
        /// <param name="parameters">查询参数集合。</param>
        /// <param name="cancellationToken">取消操作的通知。</param>
        /// <returns>第一行的第一列数据。</returns>
        Task<object?> ExecuteScalarAsync(IQueryCommand queryCommand, ParameterCollection? parameters = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// 执行查询文本，并返回第一行的第一列。
        /// </summary>
        /// <param name="queryCommand">查询命令。</param>
        /// <param name="parameters">查询参数集合。</param>
        /// <param name="cancellationToken">取消操作的通知。</param>
        /// <returns>第一行的第一列数据。</returns>
        Task<T?> ExecuteScalarAsync<T>(IQueryCommand queryCommand, ParameterCollection? parameters = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// 执行查询文本并将结果填充到指定的 <see cref="DataSet"/> 对象中。
        /// </summary>
        /// <param name="dataSet">要填充的 <see cref="DataSet"/>。</param>
        /// <param name="queryCommand">查询命令。</param>
        /// <param name="tableName">表的名称，多个表名称使用逗号分隔。</param>
        /// <param name="segment">数据分段对象。</param>
        /// <param name="parameters">查询参数集合。</param>
        Task FillDataSetAsync(DataSet dataSet, IQueryCommand queryCommand, string? tableName = null, IDataSegment? segment = null, ParameterCollection? parameters = null);

        /// <summary>
        /// 将 <see cref="DataTable"/> 的更改保存到数据库中，这类更改包括新增、修改和删除的数据。
        /// </summary>
        /// <param name="dataTable">要更新的数据表对象。</param>
        /// <param name="cancellationToken">取消操作的通知。</param>
        Task UpdateAsync(DataTable dataTable, CancellationToken cancellationToken = default);

        /// <summary>
        /// 将 <see cref="DataTable"/> 的更改保存到数据库中。
        /// </summary>
        /// <param name="dataTable">要更新的数据表对象。</param>
        /// <param name="insertCommand"></param>
        /// <param name="updateCommand"></param>
        /// <param name="deleteCommand"></param>
        /// <param name="cancellationToken">取消操作的通知。</param>
        /// <returns></returns>
        Task<int> UpdateAsync(DataTable dataTable, SqlCommand insertCommand, SqlCommand updateCommand, SqlCommand deleteCommand, CancellationToken cancellationToken = default);

        /// <summary>
        /// 尝试连接数据库。
        /// </summary>
        /// <param name="cancellationToken">取消操作的通知。</param>
        /// <returns>如果连接成功，则为 null，否则为异常对象。</returns>
        Task<Exception> TryConnectAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// 获取服务。
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <returns></returns>
        TService? GetService<TService>();
    }
}