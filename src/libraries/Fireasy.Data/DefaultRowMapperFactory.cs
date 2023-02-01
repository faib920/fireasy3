// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using System;

namespace Fireasy.Data
{
    public class DefaultRowMapperFactory : IRowMapperFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public DefaultRowMapperFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IDataRowMapper<T> CreateRowMapper<T>()
        {
            return typeof(T).IsDbTypeSupported() ?
                (IDataRowMapper<T>)SingleValueRowMapper<T>.Create(_serviceProvider) : new DefaultRowMapper<T>(_serviceProvider);
        }
    }
}
