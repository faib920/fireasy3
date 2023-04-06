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
    internal class MySqlConnector : IBulkCopyProviderBuilder
    {
        string IBulkCopyProviderBuilder.BulkCopyProviderTypeName => "MySqlConnector_BulkCopyProvider";

        SourceText IBulkCopyProviderBuilder.BuildSource()
        {
            var sb = new StringBuilder();

            sb.AppendLine(@"
using Fireasy.Common;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using MySqlConnector;

namespace Fireasy.Data.Batcher
{
    public class MySqlConnector_BulkCopyProvider : DisposableBase, IBulkCopyProvider
    {
        private MySqlBulkCopy _bulkCopy;

        void IBulkCopyProvider.Initialize(DbConnection connection, DbTransaction? transaction, string tableName, int batchSize)
        {
            if (connection is not MySqlConnection conn)
            {
                throw new System.InvalidOperationException(""确保当前项目中的 MySql 的适配器仅安装了 MySqlConnector 包。"");
            }

            _bulkCopy = new MySqlBulkCopy(conn, transaction as MySqlTransaction)
            {
                DestinationTableName = tableName
            };
        }

        void IBulkCopyProvider.AddColumnMapping(int sourceColumnIndex, string destinationColumn)
        {
            _bulkCopy.ColumnMappings.Add(new MySqlBulkCopyColumnMapping(sourceColumnIndex, destinationColumn));
        }

        async Task IBulkCopyProvider.WriteToServerAsync(DbDataReader reader, CancellationToken cancellationToken = default)
        {
            await _bulkCopy.WriteToServerAsync(reader, cancellationToken).ConfigureAwait(false);
        }

        async Task IBulkCopyProvider.WriteToServerAsync(DataTable table, CancellationToken cancellationToken = default)
        {
            await _bulkCopy.WriteToServerAsync(table, cancellationToken).ConfigureAwait(false);
        }
    }
}
");

            return SourceText.From(sb.ToString(), Encoding.UTF8);
        }
    }
}
