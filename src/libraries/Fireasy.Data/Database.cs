// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Fireasy.Common.Dynamic;
using Fireasy.Common.Extensions;
using Fireasy.Data.Extensions;
using Fireasy.Data.Identity;
using Fireasy.Data.Internal;
using Fireasy.Data.Provider;
using Fireasy.Data.RecordWrapper;
using Fireasy.Data.Syntax;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Fireasy.Data
{
    /// <summary>
    /// 提供数据库基本操作的方法。
    /// </summary>
    public class Database : DisposableBase, IDatabase, IDistributedDatabase, IServiceProvider
    {
        private TransactionStack? _tranStack;
        private DbConnection? _connMaster;
        private DbConnection? _connSlave;
        private DbTransaction? _transaction;
        private DbTransactionScope? _transactionScope;

        /// <summary>
        /// 初始化 <see cref="Database"/> 类的新实例。
        /// </summary>
        /// <param name="provider">数据库提供者。</param>
        protected Database(IProvider provider)
        {
            Provider = provider;

            var _ = GetService<DynamicDescriptionSupporter>();
        }

        /// <summary>
        /// 初始化 <see cref="Database"/> 类的新实例。
        /// </summary>
        /// <param name="connectionString">数据库连接字符串。</param>
        /// <param name="provider">数据库提供者。</param>
        public Database(ConnectionString connectionString, IProvider provider)
            : this(provider)
        {
            ConnectionString = connectionString;
        }

        /// <summary>
        /// 初始化 <see cref="Database"/> 类的新实例。
        /// </summary>
        /// <param name="connectionStrings">数据库连接字符串组。</param>
        /// <param name="provider">数据库提供者。</param>
        public Database(List<DistributedConnectionString> connectionStrings, IProvider provider)
            : this(provider)
        {
            DistributedConnectionStrings = connectionStrings.AsReadOnly();
            ConnectionString = connectionStrings.Find(s => s.Mode == DistributedMode.Master);
        }

        /// <summary>
        /// 获取或设置数据库连接字符串。
        /// </summary>
        public ConnectionString? ConnectionString { get; set; }

        /// <summary>
        /// 获取分布式数据库连接字符串组。
        /// </summary>
        public ReadOnlyCollection<DistributedConnectionString> DistributedConnectionStrings { get; private set; }

        /// <summary>
        /// 获取服务。
        /// </summary>
        /// <param name="serviceType"></param>
        /// <returns></returns>
        object? IServiceProvider.GetService(Type serviceType)
        {
            return Provider.TryGetServiceProvider()!.GetService(serviceType);
        }

        /// <summary>
        /// 获取服务。
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <returns></returns>
        public TService? GetService<TService>()
        {
            return Provider.ServiceProvider.GetService<TService>();
        }

        /// <summary>
        /// 获取数据库提供者。
        /// </summary>
        public IProvider Provider { get; private set; }

        /// <summary>
        /// 获取或设置超时时间。
        /// </summary>
        public int Timeout { get; set; }

        /// <summary>
        /// 获取当前数据库事务。
        /// </summary>
        public DbTransaction? Transaction => _transaction ?? DbTransactionScope.Current?.GetCurrentTransaction(ConnectionString);

        /// <summary>
        /// 获取当前数据库链接。
        /// </summary>
        public DbConnection? Connection => _connMaster ?? _connSlave;

        /// <summary>
        /// 使用指定锁定行为启动一个数据库事务。
        /// </summary>
        /// <param name="level">事务的锁定行为。</param>
        /// <param name="cancellationToken">取消操作的通知。</param>
        /// <returns>如果当前实例首次启动事务，则为 true，否则为 false。</returns>
        public virtual async Task<bool> BeginTransactionAsync(IsolationLevel level = IsolationLevel.ReadCommitted, CancellationToken cancellationToken = default)
        {
            if (_tranStack == null)
            {
                _tranStack = new TransactionStack();
            }

            _tranStack.Push();

            if (Transaction != null)
            {
                return false;
            }

            if (await TransactionScopeConnections.GetConnectionAsync(this) != null)
            {
                return false;
            }

            var logger = GetService<ILogger<Database>>();
            logger?.LogInformation("Starting transcation.");

            var connection = await GetConnectionAsync(DistributedMode.Master);
            await connection.TryOpenAsync(cancellationToken);

#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
            _transaction = await connection.BeginTransactionAsync(Provider.AmendIsolationLevel(level), cancellationToken).ConfigureAwait(false);
#else
            _transaction = connection.BeginTransaction(Provider.AmendIsolationLevel(level));
#endif
            _transactionScope = new DbTransactionScope(ConnectionString, _transaction);

            return true;
        }

        /// <summary>
        /// 如果与方法 BeginTransaction 匹配，则提交数据库事务。
        /// </summary>
        /// <param name="cancellationToken">取消操作的通知。</param>
        /// <returns>成功提交事务则为 true，否则为 false。</returns>
        public virtual async Task<bool> CommitTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_transaction == null || (_tranStack != null && !_tranStack.Pop()))
            {
                return false;
            }

            var logger = GetService<ILogger<Database>>();
            logger?.LogInformation("Commiting transcation.");

#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
            await _transaction.CommitAsync(cancellationToken).ConfigureAwait(false);
#else
            _transaction.Commit();
#endif
            await _connMaster!.TryCloseAsync();
            _transactionScope?.Dispose();
            _transaction = null;
            _transactionScope = null;

            return true;
        }

        /// <summary>
        /// 如果与方法 BeginTransaction 匹配，则回滚数据库事务。
        /// </summary>
        /// <param name="cancellationToken">取消操作的通知。</param>
        /// <returns>成功回滚事务则为 true，否则为 false。</returns>
        public virtual async Task<bool> RollbackTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_transaction == null || (_tranStack != null && !_tranStack.Pop()))
            {
                return false;
            }

            var logger = GetService<ILogger<Database>>();
            logger?.LogInformation("Rollbacking transcation.");

#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
            await _transaction.RollbackAsync(cancellationToken).ConfigureAwait(false);
#else
            _transaction.Rollback();
#endif
            await _connMaster!.TryCloseAsync();
            _transactionScope?.Dispose();
            _transaction = null;
            _transactionScope = null;

            return true;
        }

        /// <summary>
        /// 执行查询文本并将结果填充到指定的 <see cref="DataTable"/> 对象中。
        /// </summary>
        /// <param name="queryCommand">查询命令。</param>
        /// <param name="tableName"><see cref="DataTable"/> 的名称。</param>
        /// <param name="segment">数据分段对象。</param>
        /// <param name="parameters">查询参数集合。</param>
        /// <returns>一个 <see cref="DataTable"/> 对象。</returns>
        public virtual async Task<DataTable?> ExecuteDataTableAsync(IQueryCommand queryCommand, string? tableName = null, IDataSegment? segment = null, ParameterCollection? parameters = null)
        {
            Guard.ArgumentNull(queryCommand, nameof(queryCommand));
            var ds = new DataSet();
            await FillDataSetAsync(ds, queryCommand, tableName, segment, parameters);
            return ds.Tables.Count == 0 ? null : ds.Tables[0];
        }

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
        public async virtual IAsyncEnumerable<T> ExecuteAsyncEnumerable<T>(IQueryCommand queryCommand, IDataSegment? segment = null, ParameterCollection? parameters = null, IDataRowMapper<T>? rowMapper = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Guard.ArgumentNull(queryCommand, nameof(queryCommand));

            var rowMapperFactory = GetService<IRowMapperFactory>();

            rowMapper ??= rowMapperFactory?.CreateRowMapper<T>();
            rowMapper!.RecordWrapper = Provider.GetService<IRecordWrapper>();

            using var reader = (IAsyncDataReader)await ExecuteReaderAsync(queryCommand, segment, parameters, null, cancellationToken);
            while (await reader!.ReadAsync(cancellationToken))
            {
                yield return rowMapper.Map(reader);
            }

            while (await reader!.NextResultAsync(cancellationToken)) ;
        }

        /// <summary>
        /// 执行查询文本并将结果并返回动态序列。
        /// </summary>
        /// <param name="queryCommand">查询命令。</param>
        /// <param name="segment">数据分段对象。</param>
        /// <param name="parameters">查询参数集合。</param>
        /// <param name="cancellationToken">取消操作的通知。</param>
        /// <returns>一个动态对象的枚举器。</returns>
        public virtual async IAsyncEnumerable<dynamic> ExecuteAsyncEnumerable(IQueryCommand queryCommand, IDataSegment? segment = null, ParameterCollection? parameters = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            Guard.ArgumentNull(queryCommand, nameof(queryCommand));

            using var reader = (IAsyncDataReader)await ExecuteReaderAsync(queryCommand, segment, parameters, null, cancellationToken);
            var wrapper = Provider.GetService<IRecordWrapper>();

            while (await reader!.ReadAsync(cancellationToken))
            {
                var expando = new DynamicExpandoObject();
                var dictionary = (IDictionary<string, object>)expando!;

                for (var i = 0; i < reader.FieldCount; i++)
                {
                    var name = wrapper.GetFieldName(reader, i);
                    if (name.Equals("ROW_NUM"))
                    {
                        continue;
                    }

                    dictionary.Add(wrapper.GetFieldName(reader, i), wrapper.GetValue(reader, i));
                }

                yield return expando;
            }

            while (await reader!.NextResultAsync(cancellationToken)) ;
        }

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
        public async virtual Task<IEnumerable<T>> ExecuteEnumerableAsync<T>(IQueryCommand queryCommand, IDataSegment? segment = null, ParameterCollection? parameters = null, IDataRowMapper<T>? rowMapper = null, CancellationToken cancellationToken = default)
        {
            Guard.ArgumentNull(queryCommand, nameof(queryCommand));
            cancellationToken.ThrowIfCancellationRequested();

            var rowMapperFactory = GetService<IRowMapperFactory>();

            var result = new List<T>();
            rowMapper ??= rowMapperFactory?.CreateRowMapper<T>();
            rowMapper!.RecordWrapper = Provider.GetService<IRecordWrapper>();

            using var reader = (IAsyncDataReader)await ExecuteReaderAsync(queryCommand, segment, parameters, null, cancellationToken);
            while (await reader!.ReadAsync(cancellationToken))
            {
                result.Add(rowMapper.Map(reader));
            }

            while (await reader!.NextResultAsync(cancellationToken)) ;

            return result;
        }

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
        public async virtual Task<IEnumerable<T>> ExecuteEnumerableAsync<T>(IQueryCommand queryCommand, Func<IRecordWrapper, IDataReader, T> rowMapper, IDataSegment? segment = null, ParameterCollection? parameters = null, CancellationToken cancellationToken = default)
        {
            Guard.ArgumentNull(queryCommand, nameof(queryCommand));
            Guard.ArgumentNull(rowMapper, nameof(rowMapper));
            cancellationToken.ThrowIfCancellationRequested();

            var result = new List<T>();
            var wrapper = Provider.GetService<IRecordWrapper>();

            using var reader = (IAsyncDataReader)await ExecuteReaderAsync(queryCommand, segment, parameters, null, cancellationToken);
            while (await reader!.ReadAsync(cancellationToken))
            {
                result.Add(rowMapper(wrapper, reader));
            }

            while (await reader!.NextResultAsync(cancellationToken)) ;

            return result;
        }

        /// <summary>
        /// 执行查询文本并将结果并返回动态序列。
        /// </summary>
        /// <param name="queryCommand">查询命令。</param>
        /// <param name="segment">数据分段对象。</param>
        /// <param name="parameters">查询参数集合。</param>
        /// <param name="cancellationToken">取消操作的通知。</param>
        /// <returns>一个动态对象的枚举器。</returns>
        public virtual async Task<IEnumerable<dynamic>> ExecuteEnumerableAsync(IQueryCommand queryCommand, IDataSegment? segment = null, ParameterCollection? parameters = null, CancellationToken cancellationToken = default)
        {
            Guard.ArgumentNull(queryCommand, nameof(queryCommand));
            cancellationToken.ThrowIfCancellationRequested();

            var result = new List<dynamic>();
            using var reader = (IAsyncDataReader)await ExecuteReaderAsync(queryCommand, segment, parameters, null, cancellationToken);
            var wrapper = Provider.GetService<IRecordWrapper>();

            while (await reader!.ReadAsync(cancellationToken))
            {
                var expando = new DynamicExpandoObject();
                var dictionary = (IDictionary<string, object>)expando!;

                for (int i = 0, n = reader.FieldCount; i < n; i++)
                {
                    var name = wrapper.GetFieldName(reader, i);
                    if (name.Equals("ROW_NUM"))
                    {
                        continue;
                    }

                    dictionary.Add(wrapper.GetFieldName(reader, i), wrapper.GetValue(reader, i));
                }

                result.Add(expando);
            }

            while (await reader!.NextResultAsync(cancellationToken)) ;

            return result;
        }

        /// <summary>
        /// 执行查询文本，返回受影响的记录数。
        /// </summary>
        /// <param name="queryCommand">查询命令。</param>
        /// <param name="parameters">查询参数集合。</param>
        /// <param name="cancellationToken">取消操作的通知。</param>
        /// <returns>所影响的记录数。</returns>
        public async virtual Task<int> ExecuteNonQueryAsync(IQueryCommand queryCommand, ParameterCollection? parameters = null, CancellationToken cancellationToken = default)
        {
            Guard.ArgumentNull(queryCommand, nameof(queryCommand));
            cancellationToken.ThrowIfCancellationRequested();

            InitiaizeDistributedSynchronizer(queryCommand);

            var connection = await GetConnectionAsync(DistributedMode.Master);
            var constateMgr = await new ConnectionStateManager(connection).TryOpenAsync(cancellationToken);

            using var command = CreateDbCommand(connection, queryCommand, parameters);
            try
            {
                return await HandleCommandExecutedAsync(command, parameters, (command, ct) => command.ExecuteNonQueryAsync(ct), cancellationToken);
            }
            catch (DbException exp)
            {
                throw await HandleExceptionAsync(command, exp, cancellationToken);
            }
            finally
            {
                await constateMgr.TryCloseAsync(cancellationToken);
            }
        }

        /// <summary>
        /// 执行批处理文本，返回受影响的记录数。
        /// </summary>
        /// <param name="queryCommands">一组查询命令。</param>
        /// <param name="parameters">查询参数集合。</param>
        /// <param name="cancellationToken">取消操作的通知。</param>
        /// <returns>所影响的记录数。</returns>
        public async virtual Task<int> ExecuteBatchAsync(IEnumerable<IQueryCommand> queryCommands, ParameterCollection? parameters = null, CancellationToken cancellationToken = default)
        {
            if (queryCommands == null || queryCommands.Count() == 0)
            {
                return -1;
            }

            var syntax = Provider.GetService<ISyntaxProvider>();

            SqlCommand command = string.Join(syntax.StatementTerminator, queryCommands);

            return await ExecuteNonQueryAsync(command, parameters, cancellationToken);
        }

        /// <summary>
        /// 执行查询文本并返回一个 <see cref="IDataReader"/>。
        /// </summary>
        /// <param name="queryCommand">查询命令。</param>
        /// <param name="segment">数据分段对象。</param>
        /// <param name="parameters">查询参数集合。</param>
        /// <param name="behavior"></param>
        /// <param name="cancellationToken">取消操作的通知。</param>
        /// <returns>一个 <see cref="IDataReader"/> 对象。</returns>
        public async virtual Task<IDataReader> ExecuteReaderAsync(IQueryCommand queryCommand, IDataSegment? segment = null, ParameterCollection? parameters = null, CommandBehavior? behavior = null, CancellationToken cancellationToken = default)
        {
            Guard.ArgumentNull(queryCommand, nameof(queryCommand));
            cancellationToken.ThrowIfCancellationRequested();

            var mode = await CheckForceUseMasterAsync(() => AdjustModeAsync(queryCommand));

            var connection = await GetConnectionAsync(mode);
            var wasClosed = connection.State == ConnectionState.Closed;

            var command = new InternalDbCommand(CreateDbCommand(connection, queryCommand, parameters));
            try
            {
                var context = new CommandContext(this, queryCommand, command, segment, parameters);
                await HandleSegmentCommandAsync(context, cancellationToken);

                return await HandleCommandExecutedAsync(command, parameters, (command, cancelToken) => command.ExecuteReaderAsync(GetCommandBehavior(wasClosed, behavior), cancelToken), cancellationToken);
            }
            catch (DbException exp)
            {
                throw await HandleExceptionAsync(command, exp, cancellationToken);
            }
        }

        /// <summary>
        /// 执行查询文本，并返回第一行的第一列。
        /// </summary>
        /// <param name="queryCommand">查询命令。</param>
        /// <param name="parameters">查询参数集合。</param>
        /// <param name="cancellationToken">取消操作的通知。</param>
        /// <returns>第一行的第一列数据。</returns>
        public async virtual Task<object?> ExecuteScalarAsync(IQueryCommand queryCommand, ParameterCollection? parameters = null, CancellationToken cancellationToken = default)
        {
            Guard.ArgumentNull(queryCommand, nameof(queryCommand));
            cancellationToken.ThrowIfCancellationRequested();

            var mode = await CheckForceUseMasterAsync(() => AdjustModeAsync(queryCommand));

            var connection = await GetConnectionAsync(mode);
            var constateMgr = await new ConnectionStateManager(connection).TryOpenAsync(cancellationToken);

            using var command = CreateDbCommand(connection, queryCommand, parameters);
            try
            {
                return await HandleCommandExecutedAsync(command, parameters, (command, cancelToken) => command.ExecuteScalarAsync(cancelToken), cancellationToken);
            }
            catch (DbException exp)
            {
                throw await HandleExceptionAsync(command, exp, cancellationToken);
            }
            finally
            {
                await constateMgr.TryCloseAsync(cancellationToken);
            }
        }

        /// <summary>
        /// 执行查询文本，并返回第一行的第一列。
        /// </summary>
        /// <param name="queryCommand">查询命令。</param>
        /// <param name="parameters">查询参数集合。</param>
        /// <param name="cancellationToken">取消操作的通知。</param>
        /// <returns>第一行的第一列数据。</returns>
        public async virtual Task<T?> ExecuteScalarAsync<T>(IQueryCommand queryCommand, ParameterCollection? parameters = null, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var result = await ExecuteScalarAsync(queryCommand, parameters, cancellationToken);

            return result.To<T>();
        }

        /// <summary>
        /// 执行查询文本并将结果填充到指定的 <see cref="DataSet"/> 对象中。
        /// </summary>
        /// <param name="dataSet">要填充的 <see cref="DataSet"/>。</param>
        /// <param name="queryCommand">查询命令。</param>
        /// <param name="tableName">表的名称，多个表名称使用逗号分隔。</param>
        /// <param name="segment">数据分段对象。</param>
        /// <param name="parameters">查询参数集合。</param>
        public virtual async Task FillDataSetAsync(DataSet dataSet, IQueryCommand queryCommand, string? tableName = null, IDataSegment? segment = null, ParameterCollection? parameters = null)
        {
            Guard.ArgumentNull(queryCommand, nameof(queryCommand));
            var adapter = Provider.DbProviderFactory.CreateDataAdapter();
            if (adapter == null)
            {
                throw new NotSupportedException(nameof(DataAdapter));
            }

            var mode = await CheckForceUseMasterAsync(() => AdjustModeAsync(queryCommand));
            var connection = await GetConnectionAsync(mode);
            var constateMgr = await new ConnectionStateManager(connection).TryOpenAsync();

            using var command = CreateDbCommand(connection, queryCommand, parameters);
            adapter.SelectCommand = command;

            //如果要使用Update更新DataSet，则必须指定MissingSchemaAction.AddWithKey，
            //但在Oracle使用分页时，却不能设置该属性，否则抛出“应为标识符或带引号的标识符”
            //因此，如果要实现Update，只有手动添加DataSet的PrimaryKeys
            //adapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;
            dataSet.EnforceConstraints = false;
            HandleAdapterTableMapping(adapter, tableName);

            try
            {
                var context = new CommandContext(this, queryCommand, command, segment, parameters);

                //无法分页时才采用 adapter.Fill(dataSet, startRecord, maxRecords, "Table")
                if (segment != null && !await HandleSegmentCommandAsync(context, default))
                {
                    adapter.Fill(dataSet, segment.Start.Value, segment.Length, "Table");
                }
                else
                {
                    adapter.Fill(dataSet);

                }
            }
            catch (DbException exp)
            {
                throw await HandleExceptionAsync(command, exp, default);
            }
            finally
            {
                await constateMgr.TryCloseAsync();
            }
        }

        /// <summary>
        /// 异步的，尝试连接数据库。
        /// </summary>
        /// <param name="cancellationToken">取消操作的通知。</param>
        /// <returns>如果连接成功，则为 null，否则为异常对象。</returns>
        public virtual async Task<Exception> TryConnectAsync(CancellationToken cancellationToken = default)
        {
            using var connection = this.CreateConnection();
            var constateMgr = new ConnectionStateManager(connection);
            try
            {
                await constateMgr.TryOpenAsync(cancellationToken);

                return null;
            }
            catch (DbException exp)
            {
                return exp;
            }
            finally
            {
                await constateMgr.TryCloseAsync(cancellationToken);
            }
        }

        /// <summary>
        /// 将 <see cref="DataTable"/> 的更改保存到数据库中，这类更改包括新增、修改和删除的数据。
        /// </summary>
        /// <param name="dataTable">要更新的数据表对象。</param>
        /// <param name="cancellationToken">取消操作的通知。</param>
        public async Task UpdateAsync(DataTable dataTable, CancellationToken cancellationToken = default)
        {
            Guard.ArgumentNull(dataTable, nameof(dataTable));
            var connection = await GetConnectionAsync(DistributedMode.Master);
            var constateMgr = await new ConnectionStateManager(connection).TryOpenAsync();

            var builder = new CommandBuilder(Provider, dataTable, connection, Transaction);
            var adapter = Provider.DbProviderFactory.CreateDataAdapter();
            if (adapter == null)
            {
                throw new NotSupportedException(nameof(DataAdapter));
            }

            try
            {
                builder.FillAdapter(adapter);
                adapter.Update(dataTable);
            }
            finally
            {
                await constateMgr.TryCloseAsync();
            }
        }

        /// <summary>
        /// 将 <see cref="DataTable"/> 的更改保存到数据库中。
        /// </summary>
        /// <param name="dataTable">要更新的数据表对象。</param>
        /// <param name="insertCommand"></param>
        /// <param name="updateCommand"></param>
        /// <param name="deleteCommand"></param>
        /// <param name="cancellationToken">取消操作的通知。</param>
        /// <returns></returns>
        public async Task<int> UpdateAsync(DataTable dataTable, SqlCommand insertCommand, SqlCommand updateCommand, SqlCommand deleteCommand, CancellationToken cancellationToken = default)
        {
            Guard.ArgumentNull(dataTable, nameof(dataTable));

            InitiaizeDistributedSynchronizer(insertCommand);

            var connection = await GetConnectionAsync(DistributedMode.Master);
            var constateMgr = await new ConnectionStateManager(connection).TryOpenAsync();

            HandleDynamicDataTable(dataTable);

            var parameters = GetTableParameters(dataTable);
            var adapter = Provider.DbProviderFactory.CreateDataAdapter();
            if (adapter == null)
            {
                return await UpdateManuallyAsync(dataTable, parameters, insertCommand, updateCommand, deleteCommand);
            }

            if (insertCommand != null)
            {
                adapter.InsertCommand = CreateDbCommand(connection, insertCommand, parameters);
                adapter.InsertCommand.UpdatedRowSource = UpdateRowSource.Both;
            }

            try
            {
                var watch = Stopwatch.StartNew();
                var result = adapter.Update(dataTable);

                var logger = GetService<ILogger<Database>>();

                logger?.LogInformation($"The DbDataAdapter was executed ({watch.Elapsed.Milliseconds}ms):\n{adapter.InsertCommand.Output()}");
                watch.Stop();

                if (ConnectionString!.IsTracking)
                {
                    var commandTracker = GetService<IDbCommandTracker>();
                    await commandTracker?.OnExecuteAsync(adapter!.InsertCommand, watch!.Elapsed, cancellationToken);
                }

                return result;
            }
            catch (Exception exp)
            {
                throw await HandleExceptionAsync(adapter.InsertCommand, exp, default);
            }
            finally
            {
                await constateMgr.TryCloseAsync();
            }
        }

        protected override async ValueTask<bool> DisposeAsync(bool disposing)
        {
            var logger = GetService<ILogger<Database>>();

            logger?.LogInformation("The Database is Disposing.");

            await RollbackTransactionAsync();

            if (_connMaster != null)
            {
                await _connMaster.TryCloseAsync();

                if (disposing)
                {
                    _connMaster.Dispose();
                    _connMaster = null;
                }
            }

            if (_connSlave != null)
            {
                await _connSlave.TryCloseAsync();

                if (disposing)
                {
                    _connSlave.Dispose();
                    _connSlave = null;
                }
            }

            var sp = Provider.TryGetServiceProvider();
            if (sp is IDisposable disposable)
            {
                disposable.Dispose();
            }

            return await base.DisposeAsync(disposing);
        }

        /// <summary>
        /// 创建一个 DbCommand 对象。
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="queryCommand">查询命令。</param>
        /// <param name="parameters">参数集合。</param>
        /// <returns>一个由 <see cref="IQueryCommand"/> 和参数集合组成的 <see cref="DbCommand"/> 对象。</returns>
        [SuppressMessage("Security", "CA2100")]
        private DbCommand CreateDbCommand(DbConnection connection, IQueryCommand queryCommand, ParameterCollection? parameters)
        {
            var command = connection.CreateCommand();
            Guard.NullReference(command);
            command.CommandType = queryCommand.CommandType;
            command.CommandText = HandleCommandParameterPrefix(queryCommand.ToString());
            command.Transaction = Transaction;
            command.CommandTimeout = 60;

            if (Timeout != 0)
            {
                command.CommandTimeout = Timeout;
            }

            if (parameters != null)
            {
                command.PrepareParameters(Provider, parameters);
            }

            Provider.PrepareCommand(command);

            return command;
        }

        Task<DbConnection> IDistributedDatabase.GetConnectionAsync(DistributedMode mode)
        {
            return GetConnectionAsync(mode);
        }

        private async Task<DbConnection> GetConnectionAsync(DistributedMode mode = DistributedMode.Master)
        {
            if (Transaction != null)
            {
                return Transaction.Connection;
            }
            else
            {
                var connection = await TransactionScopeConnections.GetConnectionAsync(this);
                if (connection != null)
                {
                    return connection;
                }

                var isNew = false;

                if (mode == DistributedMode.Slave)
                {
                    connection = TryCreateConnection(mode, ref _connSlave, ref isNew);
                }
                else if (mode == DistributedMode.Master)
                {
                    connection = TryCreateConnection(mode, ref _connMaster, ref isNew);
                }

                if (isNew)
                {
                    var stateChangeTracker = GetService<IDbConnectionStateChangeTracker>();
                    if (stateChangeTracker != null)
                    {
                        connection.StateChange += (o, e) => stateChangeTracker.OnChange((DbConnection)o, e.OriginalState, e.CurrentState);
                    }
                }

                return connection;
            }
        }

        private DbConnection TryCreateConnection(DistributedMode mode, ref DbConnection? refConnection, ref bool isNew)
        {
            if (refConnection == null)
            {
                lock (this)
                {
                    if (refConnection == null)
                    {
                        refConnection = Provider.PrepareConnection(this.CreateConnection(mode));
                        isNew = true;
                    }
                }
            }

            return refConnection;
        }

        /// <summary>
        /// 处理表名映射。
        /// </summary>
        /// <param name="adapter">适配器。</param>
        /// <param name="tableName">表的名称。</param>
        private void HandleAdapterTableMapping(IDataAdapter adapter, string? tableName)
        {
            const string defaultTableName = "Table";

            //处理表名
            if (string.IsNullOrEmpty(tableName))
            {
                adapter.TableMappings.Add(defaultTableName, defaultTableName);
            }
            else if (tableName.IndexOf(',') != -1)
            {
                //如果使用|连接多个表名
                //命名为Table、Table1、Table2...
                const string sysTableNameRoot = defaultTableName;
                var tableNames = tableName.Split(',');
                for (int i = 0, n = tableNames.Length; i < n; i++)
                {
                    var sysTableName = i == 0 ? sysTableNameRoot : sysTableNameRoot + i;
                    adapter.TableMappings.Add(sysTableName, tableNames[i]);
                }
            }
            else
            {
                adapter.TableMappings.Add(defaultTableName, tableName);
            }
        }

        /// <summary>
        /// 格式化执行的SQL脚本，将 @ 替换为对应数据库的参数符号。
        /// </summary>
        /// <param name="commandText"></param>
        /// <returns></returns>
        private string HandleCommandParameterPrefix(string commandText)
        {
            var syntax = Provider.GetService<ISyntaxProvider>();
            if (string.IsNullOrEmpty(syntax?.ParameterPrefix))
            {
                return commandText;
            }

            if (Regex.IsMatch(commandText, "(\\" + syntax?.ParameterPrefix + ")"))
            {
                return commandText;
            }

            if (syntax != null && !syntax.ParameterPrefix.Equals("@"))
            {
                const string replace = "^^";
                var hasRep = false;

                if (Regex.IsMatch(commandText, "(@@)"))
                {
                    commandText = Regex.Replace(commandText, "(@@)", replace);
                    hasRep = true;
                }

                if (Regex.IsMatch(commandText, "(@)"))
                {
                    commandText = Regex.Replace(commandText, "(@)", syntax.ParameterPrefix);
                }

                if (hasRep)
                {
                    commandText = commandText.Replace("^^", "@@");
                }
            }

            return commandText;
        }

        /// <summary>
        /// 对执行的SQL脚本使用分页参数。
        /// </summary>
        /// <param name="context"></param>
        private async Task<bool> HandleSegmentCommandAsync(CommandContext context, CancellationToken cancellationToken)
        {
            //使用数据分段
            if (context.Segment != null &&
                context.Command.CommandType == CommandType.Text)
            {
                try
                {
                    var syntax = Provider.GetService<ISyntaxProvider>();
                    return syntax.Segment(await HandlePageEvaluatorAsync(context, cancellationToken));
                }
                catch (SegmentNotSupportedException)
                {
                    throw;
                }
            }

            return false;
        }

        /// <summary>
        /// 异步的，处理分页评估器。
        /// </summary>
        /// <param name="context"></param>
        private async Task<CommandContext> HandlePageEvaluatorAsync(CommandContext context, CancellationToken cancellationToken)
        {
            if (context.Segment is IDataPageEvaluatable evaluatable && evaluatable.Evaluator != null)
            {
                await evaluatable.Evaluator.EvaluateAsync(context, cancellationToken);
            }

            return context;
        }

        private ParameterCollection GetTableParameters(DataTable table)
        {
            var parameters = new ParameterCollection();
            foreach (DataColumn column in table.Columns)
            {
                var par = new Parameter(column.ColumnName) { SourceColumn = column.ColumnName };
                par.DbType = column.DataType.GetDbType();
                parameters.Add(par);
            }

            return parameters;
        }

        private async Task<int> UpdateManuallyAsync(DataTable dataTable, ParameterCollection parameters, SqlCommand insertCommand, SqlCommand updateCommand, SqlCommand deleteCommand)
        {
            if (updateCommand == null && deleteCommand == null && insertCommand != null)
            {
                return await UpdateSimpleAsync(dataTable, parameters, insertCommand);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        private async Task<int> UpdateSimpleAsync(DataTable dataTable, ParameterCollection parameters, SqlCommand sqlCommand)
        {
            const string COLUMN_RESULT = "_Result";

            if (dataTable.Columns[COLUMN_RESULT] == null)
            {
                dataTable.Columns.Add(COLUMN_RESULT, typeof(int));
            }

            var connection = await GetConnectionAsync(DistributedMode.Master);
            var connstateMgr = new ConnectionStateManager(connection);

            await BeginTransactionAsync();

            using var command = CreateDbCommand(connection, sqlCommand, parameters);
            try
            {
                var result = 0;
                foreach (DataRow row in dataTable.Rows)
                {
                    UpdateParameters(command.Parameters, row);

                    row[COLUMN_RESULT] = command.ExecuteScalar() ?? 0;

                    result++;
                }

                await CommitTransactionAsync();

                return result;
            }
            catch (DbException exp)
            {
                await RollbackTransactionAsync();
                throw await HandleExceptionAsync(command, exp, default);
            }
            finally
            {
                await connstateMgr?.TryCloseAsync();
            }
        }

        private void UpdateParameters(DbParameterCollection parameters, DataRow row)
        {
            foreach (DbParameter parameter in parameters)
            {
                if (row.Table.Columns[parameter.ParameterName] != null)
                {
                    parameter.Value = row[parameter.ParameterName];
                }
            }
        }

        /// <summary>
        /// 异步的，通知应用程序，一个 <see cref="DbCommand"/> 已经执行。
        /// </summary>
        /// <param name="command">所执行的 <see cref="DbCommand"/> 对象。</param>
        /// <param name="func">执行的方法。</param>
        /// <param name="parameters"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<TResult> HandleCommandExecutedAsync<TCommand, TResult>(TCommand command, ParameterCollection? parameters, Func<TCommand, CancellationToken, Task<TResult>> func, CancellationToken cancellationToken) where TCommand : IDbCommand
        {
            var watch = Stopwatch.StartNew();
            var result = await func(command, cancellationToken).ConfigureAwait(false);
            watch.Stop();

            var logger = GetService<ILogger<Database>>();
            logger?.LogInformation($"The DbCommand was executed ({Thread.CurrentThread.ManagedThreadId}th, {(int)watch.Elapsed.TotalMilliseconds}ms):\n{command.Output()}");

            var commandTracker = GetService<IDbCommandTracker>();
            if (ConnectionString!.IsTracking && commandTracker != null)
            {
                await commandTracker?.OnExecuteAsync(command, watch.Elapsed, cancellationToken);
            }

            command.SyncParameters(parameters);
            command.ClearParameters();

            return result;
        }

        /// <summary>
        /// 异步的，处理异常。
        /// </summary>
        /// <param name="command"></param>
        /// <param name="exp"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<Exception> HandleExceptionAsync(IDbCommand command, Exception exp, CancellationToken cancellationToken)
        {
            var logger = GetService<ILogger<Database>>();
            logger?.LogInformation($"The DbCommand was throw exception:\n{command.Output()}\n{exp.Output()}");

            var commandTracker = GetService<IDbCommandTracker>();
            if (ConnectionString!.IsTracking && commandTracker != null)
            {
                await commandTracker.OnErrorAsync(command, exp, cancellationToken);
            }

            return new CommandException(command, exp);
        }

        /// <summary>
        /// 处理使用了 <see cref="IGeneratorProvider"/> 生成数据的数据表。
        /// </summary>
        /// <param name="dataTable"></param>
        private void HandleDynamicDataTable(DataTable dataTable)
        {
            DataColumn genColumn = null;
            DataColumn newColumn = null;
            for (var i = dataTable.Columns.Count - 1; i >= 0; i--)
            {
                var column = dataTable.Columns[i];

                Type parType;
                if ((parType = DataExpressionRow.GetParameterType(column.DataType)) != null)
                {
                    newColumn = dataTable.Columns.Add(column.ColumnName + "_NEW", parType);
                    genColumn = column;
                }
            }

            if (genColumn != null && newColumn != null)
            {
                foreach (DataRow row in dataTable.Rows)
                {
                    var id = (row[genColumn] as DataExpressionRow).GetValue(this);
                    row[genColumn.ColumnName + "_NEW"] = id;
                }

                //移除原来的，新的改名
                dataTable.Columns.Remove(genColumn);
                newColumn.ColumnName = genColumn.ColumnName;
            }
        }

        /// <summary>
        /// 检查是否强制使用主库查询。
        /// </summary>
        /// <returns></returns>
        private DistributedMode CheckForceUseMaster()
        {
            var controller = GetService<DistributedController>();
            return controller?.ForceUseMaster == true ? DistributedMode.Master : DistributedMode.Slave;
        }

        /// <summary>
        /// 检查是否强制使用主库查询。
        /// </summary>
        /// <param name="otherwise">返回如果没有使用 <see cref="DistributedController"/> 则采用的模式。</param>
        /// <returns></returns>
        private async Task<DistributedMode> CheckForceUseMasterAsync(Func<Task<DistributedMode>> otherwise)
        {
            var controller = GetService<DistributedController>();
            if (controller?.ForceUseMaster == true || Transaction != null)
            {
                return DistributedMode.Master;
            }

            return await otherwise();
        }

        /// <summary>
        /// 调整主从模式。
        /// </summary>
        /// <param name="queryCommand"></param>
        /// <returns></returns>
        private async Task<DistributedMode> AdjustModeAsync(IQueryCommand queryCommand)
        {
            var distributedSynchronizer = GetService<IDistributedSynchronizer>();
            if (distributedSynchronizer == null)
            {
                return DistributedMode.Slave;
            }

            return await distributedSynchronizer.AdjustModeAsync(this, queryCommand);
        }

        /// <summary>
        /// 初始化分布式同步器。
        /// </summary>
        /// <param name="queryCommand"></param>
        private async Task InitiaizeDistributedSynchronizer(IQueryCommand queryCommand)
        {
            var distributedSynchronizer = GetService<IDistributedSynchronizer>();
            if (distributedSynchronizer != null)
            {
                await distributedSynchronizer.CatchExecutingAsync(this, queryCommand);
            }
        }

        private CommandBehavior GetCommandBehavior(bool wasClosed, CommandBehavior? behavior)
        {
            behavior ??= CommandBehavior.SingleResult;
            if (wasClosed)
            {
                behavior |= CommandBehavior.CloseConnection;
            }

            return (CommandBehavior)behavior;
        }
    }
}