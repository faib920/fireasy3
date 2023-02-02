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
    /// 缺省的 <see cref="IRowMapperFactory"/> 工厂。
    /// </summary>
    public class DefaultRowMapperFactory : IRowMapperFactory
    {
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// 初始化 <see cref="DefaultRowMapperFactory"/> 类的新实例。
        /// </summary>
        /// <param name="serviceProvider"></param>
        public DefaultRowMapperFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// 创建一个 <see cref="IDataRowMapper{T}"/> 实例。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IDataRowMapper<T> CreateRowMapper<T>()
        {
            return typeof(T).IsDbTypeSupported() ?
                (IDataRowMapper<T>)SingleValueRowMapper<T>.Create(_serviceProvider) : new DefaultRowMapper<T>(_serviceProvider);
        }
    }
}
