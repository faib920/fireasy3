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
    /// <see cref="IDataRowMapper{T}"/> 工厂。
    /// </summary>
    public interface IRowMapperFactory
    {
        /// <summary>
        /// 创建一个 <see cref="IDataRowMapper{T}"/> 实例。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IDataRowMapper<T> CreateRowMapper<T>();
    }
}
