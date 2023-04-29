// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;

namespace Fireasy.Data.Schema
{
    /// <summary>
    /// OleDb 驱动的架构信息的获取方法。
    /// </summary>
    public class OdbcSchema : SchemaBase
    {
        private DataTable? _cachedTables = null;
        private DataTable? _cachedViews = null;

        /// <summary>
        /// 初始化数据类型映射。
        /// </summary>
        protected override void InitializeDataTypes()
        {
            AddDataType(nameof(OdbcType.BigInt), DbType.Int64, typeof(long));
            AddDataType(nameof(OdbcType.Binary), DbType.Binary, typeof(byte[]));
            AddDataType(nameof(OdbcType.Bit), DbType.Boolean, typeof(bool));
            AddDataType(nameof(OdbcType.Char), DbType.String, typeof(string));
            AddDataType(nameof(OdbcType.Date), DbType.DateTime, typeof(DateTime));
            AddDataType(nameof(OdbcType.Decimal), DbType.Decimal, typeof(decimal));
            AddDataType(nameof(OdbcType.Double), DbType.Double, typeof(double));
            AddDataType(nameof(OdbcType.Image), DbType.Binary, typeof(byte[]));
            AddDataType(nameof(OdbcType.Int), DbType.Int32, typeof(int));
            AddDataType(nameof(OdbcType.Numeric), DbType.Decimal, typeof(decimal));
            AddDataType(nameof(OdbcType.NChar), DbType.String, typeof(string));
            AddDataType(nameof(OdbcType.NText), DbType.String, typeof(string));
            AddDataType(nameof(OdbcType.NVarChar), DbType.String, typeof(string));
            AddDataType(nameof(OdbcType.Real), DbType.Single, typeof(float));
            AddDataType(nameof(OdbcType.SmallDateTime), DbType.DateTime, typeof(DateTime));
            AddDataType(nameof(OdbcType.SmallInt), DbType.Int16, typeof(short));
            AddDataType(nameof(OdbcType.Text), DbType.String, typeof(string));
            AddDataType(nameof(OdbcType.Time), DbType.DateTime, typeof(DateTime));
            AddDataType(nameof(OdbcType.Timestamp), DbType.Binary, typeof(byte[]));
            AddDataType(nameof(OdbcType.TinyInt), DbType.Byte, typeof(byte));
            AddDataType(nameof(OdbcType.UniqueIdentifier), DbType.Guid, typeof(Guid));
            AddDataType(nameof(OdbcType.VarBinary), DbType.Binary, typeof(byte[]));
            AddDataType(nameof(OdbcType.VarChar), DbType.String, typeof(string));
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
            using var connection = database.CreateConnection() as OdbcConnection;
            var restrictions = new [] { null, null, restrictionValues.GetValue(nameof(Table.Name)) };
            await connection!.TryOpenAsync();
            var dtTables = connection!.GetSchema("Tables", restrictions);
            foreach (DataRow row in dtTables!.Rows)
            {
                yield return new Table
                {
                    Catalog = row["TABLE_CAT"].ToString(),
                    Schema = row["TABLE_SCHEM"].ToString(),
                    Name = row["TABLE_NAME"].ToString(),
                    Type = row["TABLE_TYPE"].ToString() == "TABLE" ? TableType.BaseTable : TableType.SystemTable,
                    Description = row["REMARKS"].ToString()
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
            using var connection = database.CreateConnection() as OdbcConnection;
            var restrictions = new[] { null, null, restrictionValues.GetValue(nameof(Column.TableName)), restrictionValues.GetValue(nameof(Column.Name)) };
            await connection!.TryOpenAsync();
            var dtTables = !restrictionValues.ContainsKey(nameof(Column.TableName)) ?
                _cachedTables ??= connection.GetSchema("Tables", new string[] { null, null, null }) : null;
            var dtColumns = connection.GetSchema("Columns", restrictions);
            foreach (DataRow row in dtColumns!.Rows)
            {
                if (dtTables != null && dtTables.Select($"TABLE_NAME = '{row["TABLE_NAME"]}'").Length == 0)
                {
                    continue;
                }

                yield return SetColumnType(SetDataType(new Column
                {
                    Catalog = row["TABLE_CAT"].ToString(),
                    Schema = row["TABLE_SCHEM"].ToString(),
                    TableName = row["TABLE_NAME"].ToString(),
                    Name = row["COLUMN_NAME"].ToString(),
                    Default = row["COLUMN_DEF"].ToString(),
                    DataType = Enum.Parse(typeof(OdbcType), row["DATA_TYPE"].ToString()).ToString(),
                    NumericPrecision = row["COLUMN_SIZE"] == DBNull.Value ? (int?)null : Convert.ToInt32(row["COLUMN_SIZE"]),
                    NumericScale = row["DECIMAL_DIGITS"] == DBNull.Value ? (int?)null : Convert.ToInt32(row["DECIMAL_DIGITS"]),
                    IsNullable = row["NULLABLE"] != DBNull.Value && Convert.ToBoolean(row["NULLABLE"]),
                    Length = row["COLUMN_SIZE"] == DBNull.Value ? (long?)null : Convert.ToInt64(row["COLUMN_SIZE"]),
                    Position = Convert.ToInt32(row["ORDINAL_POSITION"])
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
            using var connection = database.CreateConnection() as OdbcConnection;
            var restrictions = new[] { null, null, restrictionValues.GetValue(nameof(View.Name)) };
            await connection!.TryOpenAsync();
            var dtView = connection!.GetSchema("Views", restrictions);
            foreach (DataRow row in dtView!.Rows)
            {
                yield return new View
                {
                    Catalog = row["TABLE_CAT"].ToString(),
                    Schema = row["TABLE_SCHEM"].ToString(),
                    Name = row["TABLE_NAME"].ToString(),
                    Description = row["REMARKS"].ToString()
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
            using var connection = database.CreateConnection() as OdbcConnection;
            var restrictions = new[] { null, null, restrictionValues.GetValue(nameof(ViewColumn.ViewName)), restrictionValues.GetValue(nameof(ViewColumn.Name)) };
            await connection!.TryOpenAsync();
            var dtTables = !restrictionValues.ContainsKey(nameof(ViewColumn.ViewName)) ?
                _cachedTables ??= connection.GetSchema("Views", new string[] { null, null, null }) : null;
            var dtColumns = connection.GetSchema("Columns", restrictions);
            foreach (DataRow row in dtColumns!.Rows)
            {
                if (dtTables != null && dtTables.Select($"TABLE_NAME = '{row["TABLE_NAME"]}'").Length == 0)
                {
                    continue;
                }

                yield return SetDataType(new ViewColumn
                {
                    Catalog = row["TABLE_CAT"].ToString(),
                    Schema = row["TABLE_SCHEM"].ToString(),
                    ViewName = row["TABLE_NAME"].ToString(),
                    Name = row["COLUMN_NAME"].ToString(),
                    Default = row["COLUMN_DEF"].ToString(),
                    DataType = Enum.Parse(typeof(OdbcType), row["DATA_TYPE"].ToString()).ToString(),
                    NumericPrecision = row["COLUMN_SIZE"] == DBNull.Value ? (int?)null : Convert.ToInt32(row["COLUMN_SIZE"]),
                    NumericScale = row["DECIMAL_DIGITS"] == DBNull.Value ? (int?)null : Convert.ToInt32(row["DECIMAL_DIGITS"]),
                    IsNullable = row["NULLABLE"] != DBNull.Value && Convert.ToBoolean(row["NULLABLE"]),
                    Length = row["COLUMN_SIZE"] == DBNull.Value ? (long?)null : Convert.ToInt64(row["COLUMN_SIZE"]),
                    Position = Convert.ToInt32(row["ORDINAL_POSITION"])
                });
            }
        }

    }
}
