// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Fireasy.Data
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class DbCommandInterceptors : IDbCommandInterceptor
    {
        private readonly IEnumerable<IDbCommandInterceptor> _interceptors;

        /// <summary>
        /// 初始化 <see cref="DbCommandInterceptors"/> 类的新实例。
        /// </summary>
        /// <param name="interceptors"><see cref="IDbCommandInterceptor"/> 集合。</param>
        public DbCommandInterceptors(IEnumerable<IDbCommandInterceptor> interceptors)
        {
            _interceptors = interceptors;
        }

        async Task IDbCommandInterceptor.OnAfterExecuteEnumerableAsync(DbCommandInterceptContext<IEnumerable<dynamic>> context, CancellationToken cancellationToken)
        {
            foreach (var interceptor in _interceptors)
            {
                await interceptor.OnAfterExecuteEnumerableAsync(context, cancellationToken);
            }
        }

        async Task IDbCommandInterceptor.OnAfterExecuteEnumerableAsync<T>(DbCommandInterceptContext<IEnumerable<T>> context, CancellationToken cancellationToken)
        {
            foreach (var interceptor in _interceptors)
            {
                await interceptor.OnAfterExecuteEnumerableAsync<T>(context, cancellationToken);
            }
        }

        async Task IDbCommandInterceptor.OnAfterExecuteNonQueryAsync(DbCommandInterceptContext<int> context, CancellationToken cancellationToken)
        {
            foreach (var interceptor in _interceptors)
            {
                await interceptor.OnAfterExecuteNonQueryAsync(context, cancellationToken);
            }
        }

        async Task IDbCommandInterceptor.OnAfterExecuteReaderAsync(DbCommandInterceptContext<IDataReader> context, CancellationToken cancellationToken)
        {
            foreach (var interceptor in _interceptors)
            {
                await interceptor.OnAfterExecuteReaderAsync(context, cancellationToken);
            }
        }

        async Task IDbCommandInterceptor.OnAfterExecuteScalarAsync(DbCommandInterceptContext<object?> context, CancellationToken cancellationToken)
        {
            foreach (var interceptor in _interceptors)
            {
                await interceptor.OnAfterExecuteScalarAsync(context, cancellationToken);
            }
        }

        async Task IDbCommandInterceptor.OnBeforeExecuteEnumerableAsync(DbCommandInterceptContext<IEnumerable<dynamic>> context, CancellationToken cancellationToken)
        {
            foreach (var interceptor in _interceptors)
            {
                await interceptor.OnBeforeExecuteEnumerableAsync(context, cancellationToken);
                if (context.Handled)
                {
                    break;
                }
            }
        }

        async Task IDbCommandInterceptor.OnBeforeExecuteEnumerableAsync<T>(DbCommandInterceptContext<IEnumerable<T>> context, CancellationToken cancellationToken)
        {
            foreach (var interceptor in _interceptors)
            {
                await interceptor.OnBeforeExecuteEnumerableAsync<T>(context, cancellationToken);
                if (context.Handled)
                {
                    break;
                }
            }
        }

        async Task IDbCommandInterceptor.OnBeforeExecuteNonQueryAsync(DbCommandInterceptContext<int> context, CancellationToken cancellationToken)
        {
            foreach (var interceptor in _interceptors)
            {
                await interceptor.OnBeforeExecuteNonQueryAsync(context, cancellationToken);
                if (context.Handled)
                {
                    break;
                }
            }
        }

        async Task IDbCommandInterceptor.OnBeforeExecuteReaderAsync(DbCommandInterceptContext<IDataReader> context, CancellationToken cancellationToken)
        {
            foreach (var interceptor in _interceptors)
            {
                await interceptor.OnBeforeExecuteReaderAsync(context, cancellationToken);
                if (context.Handled)
                {
                    break;
                }
            }
        }

        async Task IDbCommandInterceptor.OnBeforeExecuteScalarAsync(DbCommandInterceptContext<object?> context, CancellationToken cancellationToken)
        {
            foreach (var interceptor in _interceptors)
            {
                await interceptor.OnBeforeExecuteScalarAsync(context, cancellationToken);
                if (context.Handled)
                {
                    break;
                }
            }
        }
    }
}
