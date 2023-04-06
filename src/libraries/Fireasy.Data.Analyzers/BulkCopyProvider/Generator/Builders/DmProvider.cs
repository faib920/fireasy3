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
    internal class DmProvider : IBulkCopyProviderBuilder
    {
        string IBulkCopyProviderBuilder.BulkCopyProviderTypeName => "DmProvider_BulkCopyProvider";

        SourceText IBulkCopyProviderBuilder.BuildSource()
        {
            var sb = new StringBuilder();

            sb.AppendLine(@"
using Fireasy.Common;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using Dm;

namespace Fireasy.Data.Batcher
{
    public class DmProvider_BulkCopyProvider : DisposableBase, IBulkCopyProvider
    {
        private DmBulkCopy _bulkCopy;

        void IBulkCopyProvider.Initialize(DbConnection connection, DbTransaction? transaction, string tableName, int batchSize)
        {
            if (connection is not DmConnection dmconn)
            {
                throw new System.InvalidOperationException(""确保当前项目中的 Dameng 的适配器仅安装了 DmProvider 包。"");
            }

            _bulkCopy = new DmBulkCopy(dmconn, DmBulkCopyOptions.Default, transaction as DmTransaction)
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
            _bulkCopy.WriteToServer(reader.ToDataTable());
            return Task.CompletedTask;
        }

        Task IBulkCopyProvider.WriteToServerAsync(DataTable table, CancellationToken cancellationToken = default)
        {
            _bulkCopy.WriteToServer(table);
            return Task.CompletedTask;
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