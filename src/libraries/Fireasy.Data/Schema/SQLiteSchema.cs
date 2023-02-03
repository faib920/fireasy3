// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Fireasy.Common.Extensions;
using System.Diagnostics.CodeAnalysis;

namespace Fireasy.Data.Schema
{
    /// <summary>
    /// SQLite 数据库架构信息的获取方法。
    /// </summary>
    public class SQLiteSchema : SchemaBase
    {
        /// <summary>
        /// 
        /// </summary>
        public SQLiteSchema()
        {
            AddRestriction<Table>(s => s.Name, s => s.Type);
            AddRestriction<Column>(s => s.TableName, s => s.Name);
            AddRestriction<View>(s => s.Name);
            AddRestriction<ViewColumn>(s => s.ViewName, s => s.Name);
            AddRestriction<User>(s => s.Name);
            AddRestriction<Procedure>(s => s.Name, s => s.Type);
            AddRestriction<ProcedureParameter>(s => s.ProcedureName, s => s.Name);
            AddRestriction<Index>(s => s.TableName, s => s.Name);
            AddRestriction<IndexColumn>(s => s.TableName, s => s.IndexName, s => s.ColumnName);
            AddRestriction<ForeignKey>(s => s.TableName, s => s.Name);
        }

        /// <summary>
        /// 添加数据类型映射。
        /// </summary>
        protected override void AddDataTypeMappers()
        {
            AddDataType("bit", DbType.Boolean, typeof(bool));
            AddDataType("yesno", DbType.Boolean, typeof(bool));
            AddDataType("logical", DbType.Boolean, typeof(bool));
            AddDataType("bool", DbType.Boolean, typeof(bool));
            AddDataType("boolean", DbType.Boolean, typeof(bool));
            AddDataType("smallint", DbType.Int16, typeof(short));
            AddDataType("tinyint", DbType.Byte, typeof(byte));
            AddDataType("integer", DbType.Int32, typeof(int));
            AddDataType("counter", DbType.Int64, typeof(long));
            AddDataType("autoincrement", DbType.Int64, typeof(long));
            AddDataType("identity", DbType.Int64, typeof(long));
            AddDataType("long", DbType.Int64, typeof(long));
            AddDataType("bigint", DbType.Int64, typeof(long));
            AddDataType("real", DbType.Single, typeof(float));
            AddDataType("single", DbType.Single, typeof(float));
            AddDataType("float", DbType.Double, typeof(double));
            AddDataType("double", DbType.Double, typeof(double));
            AddDataType("money", DbType.Decimal, typeof(decimal));
            AddDataType("currency", DbType.Decimal, typeof(decimal));
            AddDataType("decimal", DbType.Decimal, typeof(decimal));
            AddDataType("numeric", DbType.Decimal, typeof(decimal));
            AddDataType("varbinary", DbType.Binary, typeof(byte[]));
            AddDataType("blob", DbType.Binary, typeof(byte[]));
            AddDataType("binary", DbType.Binary, typeof(byte[]));
            AddDataType("image", DbType.Binary, typeof(byte[]));
            AddDataType("general", DbType.Binary, typeof(byte[]));
            AddDataType("oleobject", DbType.Binary, typeof(byte[]));
            AddDataType("char", DbType.String, typeof(string));
            AddDataType("nchar", DbType.String, typeof(string));
            AddDataType("varchar", DbType.String, typeof(string));
            AddDataType("nvarchar", DbType.String, typeof(string));
            AddDataType("memo", DbType.String, typeof(string));
            AddDataType("note", DbType.String, typeof(string));
            AddDataType("string", DbType.String, typeof(string));
            AddDataType("text", DbType.String, typeof(string));
            AddDataType("ntext", DbType.String, typeof(string));
            AddDataType("longtext", DbType.String, typeof(string));
            AddDataType("xml", DbType.Xml, typeof(string));
            AddDataType("decimal", DbType.Decimal, typeof(decimal));
            AddDataType("numeric", DbType.Decimal, typeof(decimal));
            AddDataType("guid", DbType.Guid, typeof(Guid));
            AddDataType("uniqueidentifier", DbType.Guid, typeof(Guid));
            AddDataType("datetime", DbType.DateTime, typeof(DateTime));
            AddDataType("smalldate", DbType.DateTime, typeof(DateTime));
            AddDataType("date", DbType.Date, typeof(DateTime));
            AddDataType("time", DbType.Time, typeof(DateTime));
            AddDataType("timestamp", DbType.DateTime, typeof(DateTime));
        }

        /// <summary>
        /// 获取 <see cref="Database"/> 元数据序列。
        /// </summary>
        /// <param name="database"></param>
        /// <param name="restrictionValues"></param>
        /// <returns></returns>
        protected override IAsyncEnumerable<Database> GetDatabasesAsync(IDatabase database, RestrictionDictionary restrictionValues)
        {
            var parameters = new ParameterCollection();

            SpecialCommand sql = $@"
PRAGMA main.database_list";

            return ExecuteAndParseMetadataAsync(database, sql, parameters, (wrapper, reader) => new Database
            {
                Name = wrapper!.GetString(reader, 0)
            });
        }

        /// <summary>
        /// 获取 <see cref="Table"/> 元数据序列。
        /// </summary>
        /// <param name="database"></param>
        /// <param name="restrictionValues"></param>
        /// <returns></returns>
        protected override IAsyncEnumerable<Table> GetTablesAsync(IDatabase database, RestrictionDictionary restrictionValues)
        {
            var parameters = new ParameterCollection();

            SpecialCommand sql = $@"
SELECT name, type FROM main.sqlite_master
WHERE type LIKE 'table'
AND (name = @NAME OR @NAME IS NULL)";

            restrictionValues.Parameterize(parameters, "NAME", nameof(Table.Name));

            return ExecuteAndParseMetadataAsync(database, sql, parameters, (wrapper, reader) => new Table
            {
                Catalog = "main",
                Name = wrapper!.GetString(reader, 0),
                Type = TableType.BaseTable
            });
        }

        /// <summary>
        /// 获取 <see cref="Column"/> 元数据序列。
        /// </summary>
        /// <param name="database"></param>
        /// <param name="restrictionValues"></param>
        /// <returns></returns>
        protected override async IAsyncEnumerable<Column> GetColumnsAsync(IDatabase database, RestrictionDictionary restrictionValues)
        {
            var parameters = new ParameterCollection();

            var columns = new List<Column>();

            //如果指定使用表名查询
            if (restrictionValues.TryGetValue(nameof(Column.TableName), out string tableName))
            {
                SpecialCommand sql = $@"
PRAGMA main.TABLE_INFO('{tableName}')";

                columns.AddRange(await GetColumnsAsync(database, tableName));
            }
            else
            {
                //循环所有表，对每个表进行查询
                await foreach (var tb in GetTablesAsync(database, RestrictionDictionary.Empty))
                {
                    SpecialCommand sql = $@"
PRAGMA main.TABLE_INFO('{tb.Name}')";

                    columns.AddRange(await GetColumnsAsync(database, tb.Name));
                }
            }

            //如果使用列名进行查询
            if (restrictionValues.TryGetValue(nameof(Column.Name), out string columnName))
            {
                columns = columns.Where(s => s.Name == columnName).ToList();
            }

            foreach (var column in columns)
            {
                yield return column;
            }
        }

        /// <summary>
        /// 获取 <see cref="View"/> 元数据序列。
        /// </summary>
        /// <param name="database"></param>
        /// <param name="restrictionValues"></param>
        /// <returns></returns>
        protected override IAsyncEnumerable<View> GetViewsAsync(IDatabase database, RestrictionDictionary restrictionValues)
        {
            var parameters = new ParameterCollection();

            SpecialCommand sql = $@"
SELECT name, type FROM main.sqlite_master
WHERE type LIKE 'view'
AND (name = @NAME OR @NAME IS NULL)";

            restrictionValues.Parameterize(parameters, "NAME", nameof(View.Name));

            return ExecuteAndParseMetadataAsync(database, sql, parameters, (wrapper, reader) => new View
            {
                Catalog = "main",
                Name = wrapper!.GetString(reader, 0)
            });
        }

        /// <summary>
        /// 获取 <see cref="ViewColumn"/> 元数据序列。
        /// </summary>
        /// <param name="database"></param>
        /// <param name="restrictionValues"></param>
        /// <returns></returns>
        protected override async IAsyncEnumerable<ViewColumn> GetViewColumnsAsync(IDatabase database, RestrictionDictionary restrictionValues)
        {
            var parameters = new ParameterCollection();

            var columns = new List<ViewColumn>();

            //如果指定使用表名查询
            if (restrictionValues.TryGetValue(nameof(ViewColumn.ViewName), out string tableName))
            {
                SqlCommand sql = $@"
PRAGMA main.TABLE_INFO('{tableName}')";

                columns.AddRange(await GetViewColumnsAsync(database, tableName));
            }
            else
            {
                //循环所有表，对每个表进行查询
                await foreach (var tb in GetViewsAsync(database, RestrictionDictionary.Empty))
                {
                    SqlCommand sql = $@"
PRAGMA main.TABLE_INFO('{tb.Name}')";

                    columns.AddRange(await GetViewColumnsAsync(database, tb.Name));
                }
            }

            //如果使用列名进行查询
            if (restrictionValues.TryGetValue(nameof(ViewColumn.Name), out string columnName))
            {
                columns = columns.Where(s => s.Name == columnName).ToList();
            }

            foreach (var column in columns)
            {
                yield return column;
            }
        }

        [SuppressMessage("Security", "CA2100")]
        private async Task<List<Column>> GetColumnsAsync(IDatabase database, string tableName)
        {
            var columns = await ExecuteAndParseMetadataAsync(database, $"PRAGMA main.TABLE_INFO('{tableName}')", null, (wrapper, reader) => new Column
            {
                Catalog = "main",
                TableName = tableName,
                Name = wrapper!.GetString(reader, 1),
                DataType = wrapper.GetString(reader, 2),
                IsNullable = wrapper.GetInt32(reader, 3) == 0,
                Default = wrapper.GetString(reader, 4),
                IsPrimaryKey = wrapper.GetInt32(reader, 5) == 1
            }).ToListAsync();

            if (database.Provider.DbProviderFactory.GetType().Assembly.GetName().Name != "System.Data.SQLite")
            {
                return columns;
            }

            var sql = $"select * from main.[{tableName}]";

            using var command = database.Provider.DbProviderFactory.CreateCommand();
            if (database.Connection.State != ConnectionState.Open)
            {
                database.Connection.Open();
            }

            command.CommandText = sql;
            command.Connection = database.Connection;
            using var reader = command.ExecuteReader(CommandBehavior.SchemaOnly);
            var table = reader.GetSchemaTable();

            foreach (DataRow row in table.Rows)
            {
                var column = columns.FirstOrDefault(s => s.Name == row["ColumnName"].ToString());
                if (column == null)
                {
                    continue;
                }

                column.Autoincrement = (bool)row["IsAutoincrement"];
                column.NumericPrecision = row["NumericPrecision"] == DBNull.Value ? (int?)null : Convert.ToInt32(row["NumericPrecision"]);
                column.NumericScale = row["NumericScale"] == DBNull.Value ? (int?)null : Convert.ToInt32(row["NumericScale"]);
                column.DataType = row["DataTypeName"].ToString().ToLower();
                column.Length = row["ColumnSize"] == DBNull.Value ? (long?)null : Convert.ToInt64(row["ColumnSize"]);
                SetColumnType(column);
            }

            return columns;
        }

        [SuppressMessage("Security", "CA2100")]
        private async Task<List<ViewColumn>> GetViewColumnsAsync(IDatabase database, string tableName)
        {
            var columns = await ExecuteAndParseMetadataAsync(database, $"PRAGMA main.TABLE_INFO('{tableName}')", null, (wrapper, reader) => new ViewColumn
            {
                Catalog = "main",
                ViewName = tableName,
                Name = wrapper!.GetString(reader, 1),
                DataType = wrapper.GetString(reader, 2),
                IsNullable = wrapper.GetInt32(reader, 3) == 1,
                Default = wrapper.GetString(reader, 4),
                IsPrimaryKey = wrapper.GetInt32(reader, 5) == 1
            }).ToListAsync();

            if (database.Provider.DbProviderFactory.GetType().Assembly.GetName().Name != "System.Data.SQLite")
            {
                return columns;
            }

            var sql = $"select * from main.[{tableName}]";

            using var command = database.Provider.DbProviderFactory.CreateCommand();
            if (database.Connection.State != ConnectionState.Open)
            {
                database.Connection.Open();
            }

            command.CommandText = sql;
            command.Connection = database.Connection;
            using var reader = command.ExecuteReader(CommandBehavior.SchemaOnly);
            var table = reader.GetSchemaTable();

            foreach (DataRow row in table.Rows)
            {
                var column = columns.FirstOrDefault(s => s.Name == row["ColumnName"].ToString());
                if (column == null)
                {
                    continue;
                }

                column.Autoincrement = (bool)row["IsAutoincrement"];
                column.NumericPrecision = row["NumericPrecision"] == DBNull.Value ? (int?)null : Convert.ToInt32(row["NumericPrecision"]);
                column.NumericScale = row["NumericScale"] == DBNull.Value ? (int?)null : Convert.ToInt32(row["NumericScale"]);
                column.DataType = row["DataTypeName"].ToString().ToLower();
                column.Length = row["ColumnSize"] == DBNull.Value ? (long?)null : Convert.ToInt64(row["ColumnSize"]);
            }

            return columns;
        }

        /// <summary>
        /// 获取 <see cref="ForeignKey"/> 元数据序列。
        /// </summary>
        /// <param name="database"></param>
        /// <param name="restrictionValues"></param>
        /// <returns></returns>
        protected override async IAsyncEnumerable<ForeignKey> GetForeignKeysAsync(IDatabase database, RestrictionDictionary restrictionValues)
        {
            var parameters = new ParameterCollection();
            var foreignKeys = new List<ForeignKey>();

            //如果指定使用表名查询
            if (restrictionValues.TryGetValue(nameof(ForeignKey.TableName), out string tbName))
            {
                SpecialCommand sql = $@"
PRAGMA main.FOREIGN_KEY_LIST('{tbName}')";

                await foreach (var item in ExecuteAndParseMetadataAsync(database, sql, parameters, (wrapper, reader) => new ForeignKey
                {
                    TableName = tbName,
                    ColumnName = wrapper!.GetString(reader, 3),
                    PKTable = wrapper.GetString(reader, 2),
                    PKColumn = wrapper.GetString(reader, 4)
                }))
                {
                    yield return item;
                }
            }
            else
            {
                //循环所有表，对每个表进行查询
                await foreach (var tb in GetTablesAsync(database, RestrictionDictionary.Empty))
                {
                    SpecialCommand sql = $@"
PRAGMA main.FOREIGN_KEY_LIST('{tb.Name}')";

                    await foreach (var item in ExecuteAndParseMetadataAsync(database, sql, parameters, (wrapper, reader) => new ForeignKey
                    {
                        TableName = tb.Name,
                        ColumnName = wrapper!.GetString(reader, 3),
                        PKTable = wrapper.GetString(reader, 2),
                        PKColumn = wrapper.GetString(reader, 4)
                    }))
                    {
                        yield return item;
                    }
                }
            }
        }
    }
}
