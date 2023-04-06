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
    internal class Microsoft_Data_SqlClient : IBulkCopyProviderBuilder
    {
        string IBulkCopyProviderBuilder.BulkCopyProviderTypeName => "Microsoft_Data_SqlClient_BulkCopyProvider";

        SourceText IBulkCopyProviderBuilder.BuildSource()
        {
            var sb = new StringBuilder();

            sb.AppendLine(@"
using Fireasy.Common;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace Fireasy.Data.Batcher
{
    public class Microsoft_Data_SqlClient_BulkCopyProvider : DisposableBase, IBulkCopyProvider
    {
        private SqlBulkCopy _bulkCopy;

        void IBulkCopyProvider.Initialize(DbConnection connection, DbTransaction? transaction, string tableName, int batchSize)
        {
            if (connection is not SqlConnection sqlconn)
            {
                throw new System.InvalidOperationException(""确保当前项目中的 SqlServer 的适配器仅安装了 Microsoft.Data.SqlClient 包。"");
            }

            _bulkCopy = new SqlBulkCopy(sqlconn, SqlBulkCopyOptions.KeepIdentity, transaction as SqlTransaction)
            {
                DestinationTableName = tableName,
                BatchSize = batchSize
            };
        }

        void IBulkCopyProvider.AddColumnMapping(int sourceColumnIndex, string destinationColumn)
        {
            _bulkCopy.ColumnMappings.Add(sourceColumnIndex, destinationColumn);
        }

        async Task IBulkCopyProvider.WriteToServerAsync(DbDataReader reader, CancellationToken cancellationToken = default)
        {
            await _bulkCopy.WriteToServerAsync(reader, cancellationToken).ConfigureAwait(false);
        }

        async Task IBulkCopyProvider.WriteToServerAsync(DataTable table, CancellationToken cancellationToken = default)
        {
            await _bulkCopy.WriteToServerAsync(table, cancellationToken).ConfigureAwait(false);
        }

        protected override bool Dispose(bool disposing)
        {
            _bulkCopy?.Close();
            return base.Dispose(disposing);
        }
    }
}
");
            return SourceText.From(sb.ToString(), Encoding.UTF8);
        }
    }
}
