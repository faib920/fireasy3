﻿using Microsoft.CodeAnalysis.Text;
using System;
using System.Text;

namespace Fireasy.Data.Analyzers.BulkCopyProvider.Builders
{
    internal class Kdbndp : IBulkCopyProviderBuilder
    {
        string IBulkCopyProviderBuilder.BulkCopyProviderTypeName => "Kdbndp_BulkCopyProvider";

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
using Kdbndp;

namespace Fireasy.Data.Batcher
{
    public class Kdbndp_BulkCopyProvider : DisposableBase, IBulkCopyProvider
    {
        private KdbndpConnection _conn;
        private KdbndpBinaryImporter _writer;
        private List<string> _columns = new List<string>();
        private string _tableName;
        private ISyntaxProvider _syntax;

        public Kdbndp_BulkCopyProvider(ISyntaxProvider syntax)
        {
            _syntax = syntax;
        }

        void IBulkCopyProvider.Initialize(DbConnection connection, DbTransaction? transaction, string tableName, int batchSize)
        {
            if (connection is not KdbndpConnection kingconn)
            {
                throw new System.InvalidOperationException(""确保当前项目中的 Kingbase 的适配器仅安装了 Kdbndp 包。"");
            }

            _conn = kingconn;
            _tableName = _syntax.Delimit(tableName);
        }

        void IBulkCopyProvider.AddColumnMapping(int sourceColumnIndex, string destinationColumn)
        {
            _columns.Add(destinationColumn);
        }

        Task IBulkCopyProvider.WriteToServerAsync(DbDataReader reader, CancellationToken cancellationToken = default)
        {
            var columnNames = string.Join("","", _columns.Select(s => _syntax.Delimit(s)));
            _writer = _conn.BeginBinaryImport($""Copy {_tableName}({columnNames}) FROM STDIN (FORMAT BINARY)"");
            while (reader.Read())
            {
                var values = new object[reader.FieldCount];
                for (var i = 0; i < reader.FieldCount; i++)
                {
                    values[i] = reader.GetValue(i);
                }
                _writer.WriteRow(values);
            }
            _writer.Complete();
            return Task.CompletedTask;
        }

        Task IBulkCopyProvider.WriteToServerAsync(DataTable table, CancellationToken cancellationToken = default)
        {
            var columnNames = string.Join("","", _columns.Select(s => _syntax.Delimit(s)));
            _writer = _conn.BeginBinaryImport($""Copy {_tableName}({columnNames}) FROM STDIN (FORMAT BINARY)"");
            foreach (DataRow row in table.Rows)
            {
                _writer.WriteRow(cancellationToken, row.ItemArray);
            }
            _writer.Complete();
            return Task.CompletedTask;
        }

        protected override bool Dispose(bool disposing)
        {
            _writer?.Dispose();
            return base.Dispose(disposing);
        }
    }
}
");
            return SourceText.From(sb.ToString(), Encoding.UTF8);
        }
    }
}
