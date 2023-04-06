using Microsoft.CodeAnalysis.Text;
using System;
using System.Text;

namespace Fireasy.Data.Analyzers.BulkCopyProvider.Generator.Builders
{
    internal class Npgsql : IBulkCopyProviderBuilder
    {
        string IBulkCopyProviderBuilder.BulkCopyProviderTypeName => "Npgsql_BulkCopyProvider";

        SourceText IBulkCopyProviderBuilder.BuildSource()
        {
            var sb = new StringBuilder();

            sb.AppendLine(@"
using Fireasy.Common;
using Fireasy.Data.Syntax;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using System.Linq;
using Npgsql;

namespace Fireasy.Data.Batcher
{
    public class Npgsql_BulkCopyProvider : DisposableBase, IBulkCopyProvider
    {
        private NpgsqlConnection _conn;
        private NpgsqlBinaryImporter _importor;
        private List<string> _columns = new List<string>();
        private string _tableName;
        private ISyntaxProvider _syntax;

        public Npgsql_BulkCopyProvider(ISyntaxProvider syntax)
        {
            _syntax = syntax;
        }

        void IBulkCopyProvider.Initialize(DbConnection connection, DbTransaction? transaction, string tableName, int batchSize)
        {
            if (connection is not NpgsqlConnection npgsqlconn)
            {
                throw new System.InvalidOperationException(""确保当前项目中的 PostgreSql 的适配器仅安装了 Npgsql 包。"");
            }

            _conn = npgsqlconn;
            _tableName = _syntax.Delimit(tableName);
        }

        void IBulkCopyProvider.AddColumnMapping(int sourceColumnIndex, string destinationColumn)
        {
            _columns.Add(destinationColumn);
        }

        async Task IBulkCopyProvider.WriteToServerAsync(DbDataReader reader, CancellationToken cancellationToken = default)
        {
            var columnNames = string.Join("","", _columns.Select(s => _syntax.Delimit(s)));
            _importor = await _conn.BeginBinaryImportAsync($""Copy {_tableName}({columnNames}) FROM STDIN (FORMAT BINARY)"");
            while (reader.Read())
            {
                await _importor.StartRowAsync(cancellationToken);
                var values = new object[reader.FieldCount];
                for (var i = 0; i < reader.FieldCount; i++)
                {
                    values[i] = reader.GetValue(i);
                }
                await _importor.WriteRowAsync(cancellationToken, values);
            }
            await _importor.CompleteAsync(cancellationToken);
        }

        async Task IBulkCopyProvider.WriteToServerAsync(DataTable table, CancellationToken cancellationToken = default)
        {
            var columnNames = string.Join("","", _columns.Select(s => _syntax.Delimit(s)));
            _importor = await _conn.BeginBinaryImportAsync($""Copy {_tableName}({columnNames}) FROM STDIN (FORMAT BINARY)"");
            foreach (DataRow row in table.Rows)
            {
                await _importor.WriteRowAsync(cancellationToken, row.ItemArray);
            }
            await _importor.CompleteAsync(cancellationToken);
        }

        protected override bool Dispose(bool disposing)
        {
            _importor?.Dispose();
            return base.Dispose(disposing);
        }
    }
}
");
            return SourceText.From(sb.ToString(), Encoding.UTF8);
        }
    }
}
