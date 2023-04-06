// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace Fireasy.Data.Analyzers.BulkCopyProvider.Generator.Builders
{
    internal class Oracle_ManagedDataAccess : IBulkCopyProviderBuilder
    {
        string IBulkCopyProviderBuilder.BulkCopyProviderTypeName => "Oracle_ManagedDataAccess_BulkCopyProvider";

        SourceText IBulkCopyProviderBuilder.BuildSource()
        {
            var sb = new StringBuilder();

            sb.AppendLine(@"
using Fireasy.Common;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;

namespace Fireasy.Data.Batcher
{
    public class Oracle_ManagedDataAccess_BulkCopyProvider : DisposableBase, IBulkCopyProvider
    {
        private OracleBulkCopy _bulkCopy;

        void IBulkCopyProvider.Initialize(DbConnection connection, DbTransaction? transaction, string tableName, int batchSize)
        {
            if (connection is not OracleConnection conn)
            {
                throw new System.InvalidOperationException(""确保当前项目中的 Oracle 的适配器仅安装了 Oracle.ManagedDataAccess 包。"");
            }

            _bulkCopy = new OracleBulkCopy(conn)
            {
                DestinationTableName = tableName,
                BatchSize = batchSize
            };
        }

        void IBulkCopyProvider.AddColumnMapping(int sourceColumnIndex, string destinationColumn)
        {
            _bulkCopy.ColumnMappings.Add(sourceColumnIndex, destinationColumn);
        }

        Task IBulkCopyProvider.WriteToServerAsync(DbDataReader reader, CancellationToken cancellationToken = default)
        {
            _bulkCopy.WriteToServer(reader);
            return Task.CompletedTask;
        }

        Task IBulkCopyProvider.WriteToServerAsync(DataTable table, CancellationToken cancellationToken = default)
        {
            _bulkCopy.WriteToServer(table);
            return Task.CompletedTask;
        }

        protected override bool Dispose(bool disposing)
        {
            _bulkCopy?.Dispose();
            return base.Dispose(disposing);
        }
    }
}
");

            return SourceText.From(sb.ToString(), Encoding.UTF8);
        }
    }
}