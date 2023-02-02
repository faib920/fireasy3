// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Fireasy.Data.Provider;
using Fireasy.Data.Syntax;
using Microsoft.Extensions.DependencyInjection;
using System.Data.Common;
using System.Linq.Expressions;

namespace Fireasy.Data.Batcher
{
    /// <summary>
    /// 为 System.Data.SqlClient 提供的用于批量操作的方法。无法继承此类。
    /// </summary>
    public sealed class SqlServerBatcher : IBatcherProvider
    {
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// 初始化 <see cref="SqlServerBatcher"/> 类的新实例。
        /// </summary>
        /// <param name="serviceProvider"></param>
        public SqlServerBatcher(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// 将 <see cref="DataTable"/> 的数据批量插入到数据库中。
        /// </summary>
        /// <param name="database">提供给当前插件的 <see cref="IDatabase"/> 对象。</param>
        /// <param name="dataTable">要批量插入的 <see cref="DataTable"/>。</param>
        /// <param name="batchSize">每批次写入的数据量。</param>
        /// <param name="completePercentage">已完成百分比的通知方法。</param>
        public async Task InsertAsync(IDatabase database, DataTable dataTable, int batchSize = 1000, Action<int> completePercentage = null, CancellationToken cancellationToken = default)
        {
            if (!BatcherChecker.CheckDataTable(dataTable))
            {
                return;
            }

            cancellationToken.ThrowIfCancellationRequested();
            ConnectionStateManager? constateMgr = null;

            try
            {
                var connection = await GetConnectionAsync(database);
                constateMgr = await new ConnectionStateManager(connection).TryOpenAsync(cancellationToken);

                var syntax = database.Provider.GetService<ISyntaxProvider>();
                var tableName = syntax!.Delimit(dataTable.TableName);
                using var bulk = GetBulkCopyProvider();
                bulk.Initialize(connection, database.Transaction, tableName, batchSize);
                using var reader = new DataTableBatchReader(dataTable, (i, n) => bulk.AddColumnMapping(i, n));
                await bulk.WriteToServerAsync(reader, cancellationToken);
            }
            catch (Exception exp)
            {
                throw new BatcherException(dataTable.Rows, exp);
            }
            finally
            {
                await constateMgr?.TryCloseAsync(cancellationToken);
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
        public async Task InsertAsync<T>(IDatabase database, IEnumerable<T> list, string tableName, int batchSize = 1000, Action<int> completePercentage = null, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ConnectionStateManager constateMgr = null;

            try
            {
                var connection = await GetConnectionAsync(database);
                constateMgr = await new ConnectionStateManager(connection).TryOpenAsync(cancellationToken);

                using var bulk = GetBulkCopyProvider();
                bulk.Initialize(connection, database.Transaction, tableName, batchSize);
                using var reader = new EnumerableBatchReader<T>(list, (i, n) => bulk.AddColumnMapping(i, n));
                await bulk.WriteToServerAsync(reader, cancellationToken);
            }
            catch (Exception exp)
            {
                throw new BatcherException(list.ToList(), exp);
            }
            finally
            {
                await constateMgr?.TryCloseAsync(cancellationToken);
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
        public async Task InsertAsync(IDatabase database, IDataReader reader, string tableName, int batchSize = 1000, Action<int> completePercentage = null, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ConnectionStateManager constateMgr = null;

            try
            {
                var connection = await GetConnectionAsync(database);
                constateMgr = await new ConnectionStateManager(connection).TryOpenAsync(cancellationToken);

                using var bulk = GetBulkCopyProvider();
                bulk.Initialize(connection, database.Transaction, tableName, batchSize);
                await bulk.WriteToServerAsync((DbDataReader)reader, cancellationToken);
            }
            catch (Exception exp)
            {
                throw new BatcherException(null, exp);
            }
            finally
            {
                await constateMgr?.TryCloseAsync(cancellationToken);
            }
        }

        private async Task<DbConnection?> GetConnectionAsync(IDatabase database)
        {
            return database is IDistributedDatabase distDb ? await distDb.GetConnectionAsync(DistributedMode.Master) : database.Connection;
        }

        private IBulkCopyProvider GetBulkCopyProvider()
        {
            return _serviceProvider.GetService<IBulkCopyProvider>() ?? new DefaultSqlBulkCopyProvider(_serviceProvider);
        }

        internal class DefaultSqlBulkCopyProvider : IBulkCopyProvider
        {
            private object? _bulkCopy;

            private static object _locker = new object();
            private static Func<object>? _createInstanceFunc;
            private static Func<object, DbDataReader, CancellationToken, Task>? _writeToServerFunc;
            private static Func<object, int, string, Task>? _addMappingFunc;
            private readonly IServiceProvider _serviceProvider;

            public DefaultSqlBulkCopyProvider(IServiceProvider serviceProvider)
            {
                _serviceProvider = serviceProvider;
            }

            public void Dispose()
            {
                if (_bulkCopy is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }

            public void Initialize(DbConnection connection, DbTransaction transaction, string tableName, int batchSize)
            {
                lock (_locker)
                {
                    if (_createInstanceFunc == null)
                    {
                        var provider = _serviceProvider.GetService<IProvider>();
                        var assembly = provider.DbProviderFactory.GetType().Assembly;
                        var bulkCopyType = assembly.GetType("SqlBulkCopy");

                        var newExp = Expression.New(bulkCopyType.GetConstructors()[0], Expression.Constant(connection), Expression.Constant(1), Expression.Constant(transaction));
                        var memberInitExp = Expression.MemberInit(newExp,
                            Expression.Bind(bulkCopyType.GetProperty("DestinationTableName"), Expression.Constant(tableName)),
                            Expression.Bind(bulkCopyType.GetProperty("BatchSize"), Expression.Constant(batchSize)));

                        _createInstanceFunc = Expression.Lambda<Func<object>>(memberInitExp).Compile();

                        var mthWriteToServer = bulkCopyType.GetMethod("WriteToServerAsync");

                        var instanceExp = Expression.Parameter(bulkCopyType);
                        var dataReaderExp = Expression.Parameter(typeof(DbDataReader));
                        var cancelTokenExp = Expression.Parameter(typeof(CancellationToken));

                        var bodyExp = Expression.Call(instanceExp, mthWriteToServer, dataReaderExp, cancelTokenExp);
                        _writeToServerFunc = Expression.Lambda<Func<object, DbDataReader, CancellationToken, Task>>(bodyExp, instanceExp, dataReaderExp, cancelTokenExp).Compile();

                        var propMappings = bulkCopyType.GetProperty("ColumnMappings");
                        var mthAdd = propMappings.GetType().GetMethod("Add");

                        var sourceColumnIndexExp = Expression.Parameter(mthAdd.GetParameters()[0].ParameterType);
                        var descColumnExp = Expression.Parameter(mthAdd.GetParameters()[1].ParameterType);

                        var propAccessExp = Expression.MakeMemberAccess(instanceExp, propMappings);
                        bodyExp = Expression.Call(propAccessExp, mthAdd, sourceColumnIndexExp, descColumnExp);

                        _addMappingFunc = Expression.Lambda<Func<object, int, string, Task>>(bodyExp, instanceExp, sourceColumnIndexExp, descColumnExp).Compile();
                    }
                }

                _bulkCopy = _createInstanceFunc();
            }

            public void AddColumnMapping(int sourceColumnIndex, string destinationColumn)
            {
                if (_bulkCopy != null && _addMappingFunc != null)
                {
                    _addMappingFunc(_bulkCopy, sourceColumnIndex, destinationColumn);
                    //_bulkCopy.ColumnMappings.Add(sourceColumnIndex, destinationColumn);
                }
            }

            public async Task WriteToServerAsync(DbDataReader reader, CancellationToken cancellationToken)
            {
                if (_bulkCopy != null && _writeToServerFunc != null)
                {
                    await _writeToServerFunc(_bulkCopy, reader, cancellationToken).ConfigureAwait(false);
                    //await _bulkCopy.WriteToServerAsync(reader, cancellationToken).ConfigureAwait(false);
                }
            }
        }
    }
}
