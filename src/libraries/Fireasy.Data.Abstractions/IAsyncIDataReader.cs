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
    /// 为 <see cref="IDataReader"/> 提供异步支持。
    /// </summary>
    public interface IAsyncIDataReader : IDataReader
    {
        /// <summary>
        /// 读取下一条记录。
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<bool> ReadAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// 读取下一个结果。
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<bool> NextResultAsync(CancellationToken cancellationToken = default);
    }
}
