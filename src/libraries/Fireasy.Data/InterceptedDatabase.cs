﻿// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Fireasy.Data.Provider;
using Fireasy.Data.RecordWrapper;

namespace Fireasy.Data
{
    /// <summary>
    /// 提供拦截器的 <see cref="IDatabase"/> 实现类。
    /// </summary>
    public class InterceptedDatabase : Database
    {
        private IDbCommandInterceptor? _interceptor;
        private bool _isInitialized;

        /// <summary>
        /// 初始化 <see cref="InterceptedDatabase"/> 类的新实例。
        /// </summary>
        /// <param name="provider">数据库提供者。</param>
        protected InterceptedDatabase(IProvider provider)
            : base(provider)
        {
        }

        /// <summary>
        /// 初始化 <see cref="InterceptedDatabase"/> 类的新实例。
        /// </summary>
        /// <param name="connectionString">数据库连接字符串。</param>
        /// <param name="provider">数据库提供者。</param>
        public InterceptedDatabase(ConnectionString connectionString, IProvider provider)
            : base(connectionString, provider)
        {
        }

        /// <summary>
        /// 初始化 <see cref="InterceptedDatabase"/> 类的新实例。
        /// </summary>
        /// <param name="connectionStrings">数据库连接字符串组。</param>
        /// <param name="provider">数据库提供者。</param>
        public InterceptedDatabase(List<DistributedConnectionString> connectionStrings, IProvider provider)
            : base(connectionStrings, provider)
        {
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
        public override async Task<IEnumerable<T>> ExecuteEnumerableAsync<T>(IQueryCommand queryCommand, Func<IRecordWrapper, IDataReader, T> rowMapper, IDataSegment? segment = null, ParameterCollection? parameters = null, CancellationToken cancellationToken = default)
        {
            var interceptor = GetInterceptor();
            if (interceptor == null)
            {
                return await base.ExecuteEnumerableAsync(queryCommand, rowMapper, segment, parameters, cancellationToken);
            }

            var ic = new DbCommandInterceptContext<IEnumerable<T>>(this, queryCommand, segment, parameters);
            await interceptor.OnBeforeExecuteEnumerableAsync(ic, cancellationToken);
            if (ic.Handled)
            {
                return ic.Result;
            }

            ic.Result = await base.ExecuteEnumerableAsync(queryCommand, rowMapper, segment, parameters, cancellationToken);

            await interceptor.OnAfterExecuteEnumerableAsync(ic, cancellationToken);

            return ic.Result;
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
        public override async Task<IEnumerable<T>> ExecuteEnumerableAsync<T>(IQueryCommand queryCommand, IDataSegment? segment = null, ParameterCollection? parameters = null, IDataRowMapper<T>? rowMapper = null, CancellationToken cancellationToken = default)
        {
            var interceptor = GetInterceptor();
            if (interceptor == null)
            {
                return await base.ExecuteEnumerableAsync(queryCommand, segment, parameters, rowMapper, cancellationToken);
            }

            var ic = new DbCommandInterceptContext<IEnumerable<T>>(this, queryCommand, segment, parameters);
            await interceptor.OnBeforeExecuteEnumerableAsync(ic, cancellationToken);
            if (ic.Handled)
            {
                return ic.Result;
            }

            ic.Result = await base.ExecuteEnumerableAsync(queryCommand, segment, parameters, rowMapper, cancellationToken);

            await interceptor.OnAfterExecuteEnumerableAsync(ic, cancellationToken);

            return ic.Result;
        }

        /// <summary>
        /// 执行查询文本并将结果并返回动态序列。
        /// </summary>
        /// <param name="queryCommand">查询命令。</param>
        /// <param name="segment">数据分段对象。</param>
        /// <param name="parameters">查询参数集合。</param>
        /// <param name="cancellationToken">取消操作的通知。</param>
        /// <returns>一个动态对象的枚举器。</returns>
        public override async Task<IEnumerable<dynamic>> ExecuteEnumerableAsync(IQueryCommand queryCommand, IDataSegment? segment = null, ParameterCollection? parameters = null, CancellationToken cancellationToken = default)
        {
            var interceptor = GetInterceptor();
            if (interceptor == null)
            {
                return await base.ExecuteEnumerableAsync(queryCommand, segment, parameters, cancellationToken);
            }

            var ic = new DbCommandInterceptContext<IEnumerable<dynamic>>(this, queryCommand, segment, parameters);
            await interceptor.OnBeforeExecuteEnumerableAsync(ic, cancellationToken);
            if (ic.Handled)
            {
                return ic.Result;
            }

            ic.Result = await base.ExecuteEnumerableAsync(queryCommand, segment, parameters, cancellationToken);

            await interceptor.OnAfterExecuteEnumerableAsync(ic, cancellationToken);

            return ic.Result;
        }

        /// <summary>
        /// 执行查询文本，返回受影响的记录数。
        /// </summary>
        /// <param name="queryCommand">查询命令。</param>
        /// <param name="parameters">查询参数集合。</param>
        /// <param name="cancellationToken">取消操作的通知。</param>
        /// <returns>所影响的记录数。</returns>
        public override async Task<int> ExecuteNonQueryAsync(IQueryCommand queryCommand, ParameterCollection? parameters = null, CancellationToken cancellationToken = default)
        {
            var interceptor = GetInterceptor();
            if (interceptor == null)
            {
                return await base.ExecuteNonQueryAsync(queryCommand, parameters, cancellationToken);
            }

            var ic = new DbCommandInterceptContext<int>(this, queryCommand, null, parameters);
            await interceptor.OnBeforeExecuteNonQueryAsync(ic, cancellationToken);
            if (ic.Handled)
            {
                return ic.Result;
            }

            ic.Result = await base.ExecuteNonQueryAsync(queryCommand, parameters, cancellationToken);

            await interceptor.OnAfterExecuteNonQueryAsync(ic, cancellationToken);

            return ic.Result;
        }

        /// <summary>
        /// 执行查询文本，并返回第一行的第一列。
        /// </summary>
        /// <param name="queryCommand">查询命令。</param>
        /// <param name="parameters">查询参数集合。</param>
        /// <param name="cancellationToken">取消操作的通知。</param>
        /// <returns>第一行的第一列数据。</returns>
        public override async Task<object?> ExecuteScalarAsync(IQueryCommand queryCommand, ParameterCollection? parameters = null, CancellationToken cancellationToken = default)
        {
            var interceptor = GetInterceptor();
            if (interceptor == null)
            {
                return await base.ExecuteScalarAsync(queryCommand, parameters, cancellationToken);
            }

            var ic = new DbCommandInterceptContext<object?>(this, queryCommand, null, parameters);
            await interceptor.OnBeforeExecuteScalarAsync(ic, cancellationToken);
            if (ic.Handled)
            {
                return ic.Result;
            }

            ic.Result = await base.ExecuteScalarAsync(queryCommand, parameters, cancellationToken);

            await interceptor.OnAfterExecuteScalarAsync(ic, cancellationToken);

            return ic.Result;
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
        public override async Task<IDataReader> ExecuteReaderAsync(IQueryCommand queryCommand, IDataSegment? segment = null, ParameterCollection? parameters = null, CommandBehavior? behavior = null, CancellationToken cancellationToken = default)
        {
            var interceptor = GetInterceptor();
            if (interceptor == null)
            {
                return await base.ExecuteReaderAsync(queryCommand, segment, parameters, behavior, cancellationToken);
            }

            var ic = new DbCommandInterceptContext<IDataReader>(this, queryCommand, segment, parameters);
            await interceptor.OnBeforeExecuteReaderAsync(ic, cancellationToken);
            if (ic.Handled)
            {
                return ic.Result;
            }

            ic.Result = await base.ExecuteReaderAsync(queryCommand, segment, parameters, behavior, cancellationToken);

            await interceptor.OnAfterExecuteReaderAsync(ic, cancellationToken);

            return ic.Result;
        }

        private IDbCommandInterceptor? GetInterceptor()
        {
            if (!_isInitialized)
            {
                _interceptor = GetService<IDbCommandInterceptor>();
                _isInitialized = true;
            }

            return _interceptor;
        }
    }
}
