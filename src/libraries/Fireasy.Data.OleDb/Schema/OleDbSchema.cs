using Fireasy.Data.Schema;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;

namespace Fireasy.Data.Schema
{
    /// <summary>
    /// OleDb 驱动的架构信息的获取方法。
    /// </summary>
    public class OleDbSchema : SchemaBase
    {
        private DataTable? _cachedTables = null;
        private DataTable? _cachedViews = null;

        /// <summary>
        /// 初始化数据类型映射。
        /// </summary>
        protected override void InitializeDataTypes()
        {
            AddDataType(nameof(OleDbType.BigInt), DbType.Int64, typeof(long));
            AddDataType(nameof(OleDbType.Binary), DbType.Binary, typeof(byte[]));
            AddDataType(nameof(OleDbType.Boolean), DbType.Boolean, typeof(bool));
            AddDataType(nameof(OleDbType.Char), DbType.String, typeof(string));
            AddDataType(nameof(OleDbType.Currency), DbType.Currency, typeof(decimal));
            AddDataType(nameof(OleDbType.Date), DbType.DateTime, typeof(DateTime));
            AddDataType(nameof(OleDbType.DBDate), DbType.DateTime, typeof(DateTime));
            AddDataType(nameof(OleDbType.DBTime), DbType.DateTime, typeof(DateTime));
            AddDataType(nameof(OleDbType.DBTimeStamp), DbType.DateTime, typeof(DateTime));
            AddDataType(nameof(OleDbType.Decimal), DbType.Decimal, typeof(decimal));
            AddDataType(nameof(OleDbType.Double), DbType.Double, typeof(double));
            AddDataType(nameof(OleDbType.Guid), DbType.Guid, typeof(Guid));
            AddDataType(nameof(OleDbType.Integer), DbType.Int32, typeof(int));
            AddDataType(nameof(OleDbType.LongVarBinary), DbType.Binary, typeof(byte[]));
            AddDataType(nameof(OleDbType.LongVarChar), DbType.String, typeof(string));
            AddDataType(nameof(OleDbType.LongVarWChar), DbType.String, typeof(string));
            AddDataType(nameof(OleDbType.Numeric), DbType.Decimal, typeof(decimal));
            AddDataType(nameof(OleDbType.Single), DbType.Single, typeof(float));
            AddDataType(nameof(OleDbType.SmallInt), DbType.Int16, typeof(short));
            AddDataType(nameof(OleDbType.TinyInt), DbType.Byte, typeof(byte));
            AddDataType(nameof(OleDbType.UnsignedBigInt), DbType.UInt64, typeof(ulong));
            AddDataType(nameof(OleDbType.UnsignedInt), DbType.UInt32, typeof(uint));
            AddDataType(nameof(OleDbType.UnsignedSmallInt), DbType.UInt16, typeof(ushort));
            AddDataType(nameof(OleDbType.UnsignedTinyInt), DbType.Byte, typeof(byte));
            AddDataType(nameof(OleDbType.VarBinary), DbType.Binary, typeof(byte[]));
            AddDataType(nameof(OleDbType.VarChar), DbType.String, typeof(string));
            AddDataType(nameof(OleDbType.VarNumeric), DbType.Decimal, typeof(decimal));
            AddDataType(nameof(OleDbType.VarWChar), DbType.String, typeof(string));
            AddDataType(nameof(OleDbType.Variant), DbType.Object, typeof(object));
            AddDataType(nameof(OleDbType.WChar), DbType.String, typeof(string));
        }

        /// <summary>
        /// 初始化约定查询限制。
        /// </summary>
        protected override void InitializeRestrictions()
        {
            AddRestriction<Table>(s => s.Name, s => s.Type);
            AddRestriction<Column>(s => s.TableName, s => s.Name);
            AddRestriction<View>(s => s.Name);
            AddRestriction<ViewColumn>(s => s.ViewName, s => s.Name);
            AddRestriction<ForeignKey>(s => s.TableName, s => s.Name);
        }

        /// <summary>
        /// 获取是否提供同一限制的集合查询。
        /// </summary>
        public override bool RestrictionMultipleQuerySupport => false;

        /// <summary>
        /// 获取 <see cref="Table"/> 元数据序列。
        /// </summary>
        /// <param name="database"></param>
        /// <param name="restrictionValues"></param>
        /// <returns></returns>
        protected override async IAsyncEnumerable<Table> GetTablesAsync(IDatabase database, RestrictionDictionary restrictionValues)
        {
            using var connection = database.CreateConnection() as OleDbConnection;
            var tableType = restrictionValues.GetValue(nameof(Table.Type)) == "1" ? "SYSTEM TABLE" : "TABLE";
            var restrictions = new[] { null, null, restrictionValues.GetValue(nameof(Table.Name)), tableType };
            await connection!.TryOpenAsync();
            var dtTables = connection!.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, restrictions);
            foreach (DataRow row in dtTables!.Rows)
            {
                yield return new Table
                {
                    Catalog = row["TABLE_CATALOG"].ToString(),
                    Schema = row["TABLE_SCHEMA"].ToString(),
                    Name = row["TABLE_NAME"].ToString(),
                    Type = row["TABLE_TYPE"].ToString() == "TABLE" ? TableType.BaseTable : TableType.SystemTable,
                    Description = ""
                };
            }
        }

        /// <summary>
        /// 获取 <see cref="Column"/> 元数据序列。
        /// </summary>
        /// <param name="database"></param>
        /// <param name="restrictionValues"></param>
        /// <returns></returns>
        protected override async IAsyncEnumerable<Column> GetColumnsAsync(IDatabase database, RestrictionDictionary restrictionValues)
        {
            using var connection = database.CreateConnection() as OleDbConnection;
            var restrictions = new[] { null, null, restrictionValues.GetValue(nameof(Column.TableName)), restrictionValues.GetValue(nameof(Column.Name)) };
            await connection!.TryOpenAsync();
            var dtPrimary = connection!.GetOleDbSchemaTable(OleDbSchemaGuid.Primary_Keys, new[] { null, null, restrictionValues.GetValue(nameof(Column.TableName)) });
            var dtTables = !restrictionValues.ContainsKey(nameof(Column.TableName)) ? 
                _cachedTables ??= connection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new[] { null, null, null, "TABLE" }) : null;
            var dtColumns = connection.GetOleDbSchemaTable(OleDbSchemaGuid.Columns, restrictions);
            foreach (DataRow row in dtColumns!.Rows)
            {
                if (dtTables != null && dtTables.Select($"TABLE_NAME = '{row["TABLE_NAME"]}'").Length == 0)
                {
                    continue;
                }

                yield return SetColumnType(SetDataType(new Column
                {
                    Catalog = row["TABLE_CATALOG"].ToString(),
                    Schema = row["TABLE_SCHEMA"].ToString(),
                    TableName = row["TABLE_NAME"].ToString(),
                    Name = row["COLUMN_NAME"].ToString(),
                    Default = row["COLUMN_DEFAULT"].ToString(),
                    DataType = Enum.Parse(typeof(OleDbType), row["DATA_TYPE"].ToString()).ToString(),
                    NumericPrecision = row["NUMERIC_PRECISION"] == DBNull.Value ? (int?)null : Convert.ToInt32(row["NUMERIC_PRECISION"]),
                    NumericScale = row["NUMERIC_SCALE"] == DBNull.Value ? (int?)null : Convert.ToInt32(row["NUMERIC_SCALE"]),
                    IsNullable = row["IS_NULLABLE"] != DBNull.Value && Convert.ToBoolean(row["IS_NULLABLE"]),
                    Length = row["CHARACTER_MAXIMUM_LENGTH"] == DBNull.Value ? (long?)null : Convert.ToInt64(row["CHARACTER_MAXIMUM_LENGTH"]),
                    Position = Convert.ToInt32(row["ORDINAL_POSITION"]),
                    IsPrimaryKey = dtPrimary.Select($"TABLE_NAME = '{row["TABLE_NAME"].ToString().Replace("\"", "").Replace("'", "")}' AND COLUMN_NAME='{row["COLUMN_NAME"].ToString()}'").Length > 0
                }));
            }
        }

        /// <summary>
        /// 获取 <see cref="View"/> 元数据序列。
        /// </summary>
        /// <param name="database"></param>
        /// <param name="restrictionValues"></param>
        /// <returns></returns>
        protected override async IAsyncEnumerable<View> GetViewsAsync(IDatabase database, RestrictionDictionary restrictionValues)
        {
            using var connection = database.CreateConnection() as OleDbConnection;
            var restrictions = new[] { null, null, restrictionValues.GetValue(nameof(View.Name)), "VIEW" };
            await connection!.TryOpenAsync();
            var dtTables = connection!.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, restrictions);
            foreach (DataRow row in dtTables!.Rows)
            {
                yield return new View
                {
                    Catalog = row["TABLE_CATALOG"].ToString(),
                    Schema = row["TABLE_SCHEMA"].ToString(),
                    Name = row["TABLE_NAME"].ToString(),
                    Description = ""
                };
            }
        }

        /// <summary>
        /// 获取 <see cref="ViewColumn"/> 元数据序列。
        /// </summary>
        /// <param name="database"></param>
        /// <param name="restrictionValues"></param>
        /// <returns></returns>
        protected override async IAsyncEnumerable<ViewColumn> GetViewColumnsAsync(IDatabase database, RestrictionDictionary restrictionValues)
        {
            using var connection = database.CreateConnection() as OleDbConnection;
            var restrictions = new[] { null, null, restrictionValues.GetValue(nameof(ViewColumn.ViewName)), restrictionValues.GetValue(nameof(ViewColumn.Name)) };
            await connection!.TryOpenAsync();
            var dtViews = !restrictionValues.ContainsKey(nameof(ViewColumn.ViewName)) ? 
                _cachedViews ??= connection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new[] { null, null, null, "VIEW" }) : null;
            var dtColumns = connection.GetOleDbSchemaTable(OleDbSchemaGuid.Columns, restrictions);
            foreach (DataRow row in dtColumns!.Rows)
            {
                if (dtViews != null && dtViews.Select($"TABLE_NAME = '{row["TABLE_NAME"]}'").Length == 0)
                {
                    continue;
                }

                yield return SetDataType(new ViewColumn
                {
                    Catalog = row["TABLE_CATALOG"].ToString(),
                    Schema = row["TABLE_SCHEMA"].ToString(),
                    ViewName = row["TABLE_NAME"].ToString(),
                    Name = row["COLUMN_NAME"].ToString(),
                    Default = row["COLUMN_DEFAULT"].ToString(),
                    DataType = Enum.Parse(typeof(OleDbType), row["DATA_TYPE"].ToString()).ToString(),
                    NumericPrecision = row["NUMERIC_PRECISION"] == DBNull.Value ? (int?)null : Convert.ToInt32(row["NUMERIC_PRECISION"]),
                    NumericScale = row["NUMERIC_SCALE"] == DBNull.Value ? (int?)null : Convert.ToInt32(row["NUMERIC_SCALE"]),
                    IsNullable = row["IS_NULLABLE"] != DBNull.Value && Convert.ToBoolean(row["IS_NULLABLE"]),
                    Length = row["CHARACTER_MAXIMUM_LENGTH"] == DBNull.Value ? (long?)null : Convert.ToInt64(row["CHARACTER_MAXIMUM_LENGTH"]),
                    Position = Convert.ToInt32(row["ORDINAL_POSITION"])
                });
            }
        }

        /// <summary>
        /// 获取 <see cref="ForeignKey"/> 元数据序列。
        /// </summary>
        /// <param name="database"></param>
        /// <param name="restrictionValues"></param>
        /// <returns></returns>
        protected override async IAsyncEnumerable<ForeignKey> GetForeignKeysAsync(IDatabase database, RestrictionDictionary restrictionValues)
        {
            using var connection = database.CreateConnection() as OleDbConnection;
            var restrictions = new[] { null, null, restrictionValues.GetValue(nameof(ForeignKey.TableName)), restrictionValues.GetValue(nameof(ForeignKey.Name)) };
            await connection!.TryOpenAsync();
            var dtFKeys = connection!.GetOleDbSchemaTable(OleDbSchemaGuid.Foreign_Keys, restrictions);
            foreach (DataRow row in dtFKeys!.Rows)
            {
                yield return new ForeignKey
                {
                    Catalog = row["FK_TABLE_CATALOG"].ToString(),
                    Schema = row["FK_TABLE_SCHEMA"].ToString(),
                    Name = row["FK_NAME"].ToString(),
                    PKTable = row["PK_TABLE_NAME"].ToString(),
                    PKColumn = row["PK_COLUMN_NAME"].ToString(),
                    TableName = row["FK_TABLE_NAME"].ToString(),
                    ColumnName = row["FK_COLUMN_NAME"].ToString()
                };
            }
        }
    }
}
