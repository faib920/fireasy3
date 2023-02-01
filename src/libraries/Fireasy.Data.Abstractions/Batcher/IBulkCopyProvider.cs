// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Fireasy.Data.Provider;
using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace Fireasy.Data.Batcher
{
    /// <summary>
    /// 
    /// </summary>
    public interface IBulkCopyProvider : IProviderService, IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="transaction"></param>
        /// <param name="tableName"></param>
        /// <param name="batchSize"></param>
        void Initialize(DbConnection connection, DbTransaction transaction, string tableName, int batchSize);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceColumnIndex"></param>
        /// <param name="destinationColumn"></param>
        void AddColumnMapping(int sourceColumnIndex, string destinationColumn);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task WriteToServerAsync(DbDataReader reader, CancellationToken cancellationToken);
    }
}
