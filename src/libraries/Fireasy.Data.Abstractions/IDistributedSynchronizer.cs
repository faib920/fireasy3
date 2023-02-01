// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using System.Threading.Tasks;

namespace Fireasy.Data
{
    /// <summary>
    /// 分布式同步器。
    /// </summary>
    public interface IDistributedSynchronizer
    {
        /// <summary>
        /// 捕捉命令的执行。
        /// </summary>
        /// <param name="database"></param>
        /// <param name="queryCommand"></param>
        Task CatchExecutingAsync(IDistributedDatabase database, IQueryCommand queryCommand);

        /// <summary>
        /// 调整分布式模式。
        /// </summary>
        /// <param name="database"></param>
        /// <param name="queryCommand"></param>
        /// <returns></returns>
        Task<DistributedMode> AdjustModeAsync(IDistributedDatabase database, IQueryCommand queryCommand);
    }
}
