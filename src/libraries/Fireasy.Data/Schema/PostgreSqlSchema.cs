// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using System.Net;
using System.Net.NetworkInformation;

namespace Fireasy.Data.Schema
{
    /// <summary>
    /// PostgreSql 数据库架构信息的获取方法。
    /// </summary>
    public class PostgreSqlSchema : SchemaBase
    {
        /// <summary>
        /// 初始化约定查询限制。
        /// </summary>
        protected override void InitializeRestrictions()
        {
            AddRestriction<Database>(s => s.Name);
            AddRestriction<Table>(s => s.Name, s => s.Type);
            AddRestriction<Column>(s => s.TableName, s => s.Name);
            AddRestriction<ForeignKey>(s => s.TableName, s => s.Name);
            AddRestriction<View>(s => s.Name);
            AddRestriction<ViewColumn>(s => s.ViewName, s => s.Name);
        }

        /// <summary>
        /// 初始化数据类型映射。
        /// </summary>
        protected override void InitializeDataTypes()
        {
            AddDataType("bigint", DbType.Int64, typeof(long));
            AddDataType("bigserial", DbType.Int64, typeof(long));
            AddDataType("bit", DbType.Boolean, typeof(bool));
            AddDataType("bit varying", DbType.Binary, typeof(byte[]));
            AddDataType("bool", DbType.Boolean, typeof(bool));
            AddDataType("boolean", DbType.Boolean, typeof(bool));
            AddDataType("box", DbType.String, typeof(object)); //NpgsqlTypes.NpgsqlBox
            AddDataType("bpchar", DbType.String, typeof(string));
            AddDataType("bytea", DbType.Binary, typeof(byte[]));
            AddDataType("char", DbType.String, typeof(string));
            AddDataType("character", DbType.String, typeof(string));
            AddDataType("character varying", DbType.String, typeof(string));
            AddDataType("cidr", DbType.String, typeof(ValueTuple<IPAddress, int>));
            AddDataType("circle", DbType.String, typeof(object)); //NpgsqlTypes.NpgsqlCircle
            AddDataType("date", DbType.Date, typeof(DateTime));
            AddDataType("decimal", DbType.Decimal, typeof(decimal));
            AddDataType("double precision", DbType.Double, typeof(double));
            AddDataType("float4", DbType.Single, typeof(float));
            AddDataType("float8", DbType.Double, typeof(double));
            AddDataType("inet", DbType.String, typeof(IPAddress));
            AddDataType("int", DbType.Int32, typeof(int));
            AddDataType("int2", DbType.Int16, typeof(short));
            AddDataType("int4", DbType.Int32, typeof(int));
            AddDataType("int8", DbType.Int64, typeof(long));
            AddDataType("integer", DbType.Int32, typeof(int));
            AddDataType("interval", DbType.Time, typeof(TimeSpan));
            AddDataType("interval year", DbType.Time, typeof(TimeSpan));
            AddDataType("interval year to month", DbType.Time, typeof(TimeSpan));
            AddDataType("interval month", DbType.Time, typeof(TimeSpan));
            AddDataType("interval day", DbType.Time, typeof(TimeSpan));
            AddDataType("interval day to hour", DbType.Time, typeof(TimeSpan));
            AddDataType("interval day to minute", DbType.Time, typeof(TimeSpan));
            AddDataType("interval day to second", DbType.Time, typeof(TimeSpan));
            AddDataType("interval hour", DbType.Time, typeof(TimeSpan));
            AddDataType("interval hour to minute", DbType.Time, typeof(TimeSpan));
            AddDataType("interval hour to second", DbType.Time, typeof(TimeSpan));
            AddDataType("interval minute", DbType.Time, typeof(TimeSpan));
            AddDataType("interval minute to second", DbType.Time, typeof(TimeSpan));
            AddDataType("interval second", DbType.Time, typeof(TimeSpan));
            AddDataType("json", DbType.String, typeof(string));
            AddDataType("jsonb", DbType.String, typeof(string));
            AddDataType("line", DbType.String, typeof(object)); //NpgsqlTypes.NpgsqlLine
            AddDataType("lseg", DbType.String, typeof(object)); //NpgsqlTypes.NpgsqlLSeg
            AddDataType("macaddr", DbType.String, typeof(PhysicalAddress));
            AddDataType("macaddr8", DbType.String, typeof(PhysicalAddress));
            AddDataType("money", DbType.Currency, typeof(decimal));
            AddDataType("numeric", DbType.Decimal, typeof(decimal));
            AddDataType("path", DbType.String, typeof(object)); //NpgsqlTypes.NpgsqlPath
            AddDataType("pg_lsn", DbType.String, typeof(object)); //NpgsqlTypes.NpgsqlLogSequenceNumber
            AddDataType("pg_snapshot", DbType.String, typeof(string));
            AddDataType("point", DbType.String, typeof(object)); //NpgsqlTypes.NpgsqlPoint
            AddDataType("polygon", DbType.String, typeof(object)); //NpgsqlTypes.NpgsqlPolygon
            AddDataType("real", DbType.Double, typeof(double));
            AddDataType("smallint", DbType.Int16, typeof(short));
            AddDataType("smallserial", DbType.Int16, typeof(short));
            AddDataType("serial", DbType.Int32, typeof(int));
            AddDataType("serial2", DbType.Int16, typeof(short));
            AddDataType("serial4", DbType.Int32, typeof(int));
            AddDataType("serial8", DbType.Int64, typeof(long));
            AddDataType("text", DbType.String, typeof(string));
            AddDataType("time", DbType.Time, typeof(TimeSpan));
            AddDataType("timetz", DbType.DateTimeOffset, typeof(DateTimeOffset));
            AddDataType("timestamp", DbType.DateTime, typeof(DateTime));
            AddDataType("timestamptz", DbType.DateTimeOffset, typeof(DateTimeOffset));
            AddDataType("tsquery", DbType.String, typeof(object)); //NpgsqlTypes.NpgsqlTsQuery
            AddDataType("tsvector", DbType.String, typeof(object)); //NpgsqlTypes.NpgsqlTsVector
            AddDataType("txid_snapshot", DbType.String, typeof(string));
            AddDataType("uuid", DbType.Guid, typeof(Guid));
            AddDataType("varbit", DbType.Binary, typeof(byte[]));
            AddDataType("varchar", DbType.String, typeof(string));
            AddDataType("xml", DbType.Xml, typeof(string));
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

            SpecialCommand sql = @"
SELECT DATNAME FROM PG_DATABASE WHERE (DATNAME = @NAME OR (@NAME IS NULL))";

            restrictionValues.Parameterize(parameters, "NAME", nameof(Database.Name));

            return ExecuteAndParseMetadataAsync(database, sql, parameters, (wrapper, reader) => new Database
            {
                Name = wrapper!.GetString(reader, 0)
            });
        }

        /// <summary>
        /// 获取 <see cref="User"/> 元数据序列。
        /// </summary>
        /// <param name="database"></param>
        /// <param name="restrictionValues"></param>
        /// <returns></returns>
        protected override IAsyncEnumerable<User> GetUsersAsync(IDatabase database, RestrictionDictionary restrictionValues)
        {
            var parameters = new ParameterCollection();

            SpecialCommand sql = "SELECT ROLNAME FROM PG_ROLES WHERE (ROLNAME = @NAME OR @NAME IS NULL)";

            restrictionValues.Parameterize(parameters, "NAME", nameof(User.Name));

            return ExecuteAndParseMetadataAsync(database, sql, parameters, (wrapper, reader) => new User
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
            var connpar = GetConnectionParameter(database);

            restrictionValues
                .Parameterize(parameters, "NAME", nameof(Table.Name));

            SpecialCommand sql = $@"
SELECT
  T.TABLE_CATALOG,
  T.TABLE_SCHEMA,
  T.TABLE_NAME,
  T.TABLE_TYPE,
  C.TABLE_COMMENT
FROM INFORMATION_SCHEMA.TABLES T
JOIN (
  SELECT RELNAME AS TABLE_NAME, CAST(OBJ_DESCRIPTION(RELFILENODE, 'pg_class') AS VARCHAR) AS TABLE_COMMENT FROM PG_CLASS C 
  WHERE RELKIND = 'r'
) C ON T.TABLE_NAME = C.TABLE_NAME
WHERE (T.TABLE_CATALOG = '{connpar.Database}' AND T.TABLE_SCHEMA = ANY (CURRENT_SCHEMAS(false))){(parameters.HasValue("NAME") ? @"
  AND T.TABLE_NAME IN (:NAME)" : string.Empty)}
 ORDER BY T.TABLE_CATALOG, T.TABLE_SCHEMA, T.TABLE_NAME";

            return ExecuteAndParseMetadataAsync(database, sql, parameters, (wrapper, reader) => new Table
            {
                Catalog = wrapper!.GetString(reader, 0),
                Schema = wrapper.GetString(reader, 1),
                Name = wrapper.GetString(reader, 2),
                Type = wrapper.GetString(reader, 3) == "BASE TABLE" ? TableType.BaseTable : TableType.SystemTable,
                Description = wrapper.GetString(reader, 4)
            });
        }

        /// <summary>
        /// 获取 <see cref="Column"/> 元数据序列。
        /// </summary>
        /// <param name="database"></param>
        /// <param name="restrictionValues"></param>
        /// <returns></returns>
        protected override IAsyncEnumerable<Column> GetColumnsAsync(IDatabase database, RestrictionDictionary restrictionValues)
        {
            var parameters = new ParameterCollection();
            var connpar = GetConnectionParameter(database);

            restrictionValues
                .Parameterize(parameters, "TABLENAME", nameof(Column.TableName))
                .Parameterize(parameters, "COLUMNNAME", nameof(Column.Name));

            SpecialCommand sql = $@"
SELECT
    COL.TABLE_CATALOG,
    COL.TABLE_SCHEMA,
    COL.TABLE_NAME,
    COL.ORDINAL_POSITION,
    COL.COLUMN_NAME,
    COL.UDT_NAME,
    COL.CHARACTER_MAXIMUM_LENGTH,
    COL.NUMERIC_PRECISION,
    COL.NUMERIC_SCALE,
    COL.IS_NULLABLE,
    COL.COLUMN_DEFAULT ,
    DES.DESCRIPTION,
	(CASE WHEN COL.ORDINAL_POSITION = CON.CONKEY[1] THEN 'YES' ELSE 'NO' END) IS_KEY
FROM
    INFORMATION_SCHEMA.COLUMNS COL
JOIN (
  SELECT RELNAME AS TABLE_NAME, CAST(OBJ_DESCRIPTION(RELFILENODE, 'pg_class') AS VARCHAR) AS TABLE_COMMENT FROM PG_CLASS C 
  WHERE RELKIND = 'r'
  ) C
    ON col.TABLE_NAME = C.TABLE_NAME
LEFT JOIN PG_CLASS PCL
    ON PCL.RELNAME = COL.TABLE_NAME
LEFT JOIN PG_DESCRIPTION DES
    ON PCL.OID = DES.OBJOID AND COL.ORDINAL_POSITION = DES.OBJSUBID
LEFT JOIN PG_CONSTRAINT CON
	ON CON.CONRELID = PCL.OID AND CON.CONTYPE = 'p'
WHERE (COL.TABLE_CATALOG = '{connpar.Database}' AND COL.TABLE_SCHEMA = ANY (CURRENT_SCHEMAS(false))){(parameters.HasValue("TABLENAME") ? @"
  AND COL.TABLE_NAME IN (@TABLENAME)" : string.Empty)}{(parameters.HasValue("COLUMNNAME") ? @"
  AND COL.COLUMN_NAME IN (@COLUMNNAME)" : string.Empty)}
ORDER BY
  COL.TABLE_CATALOG, COL.TABLE_SCHEMA, COL.TABLE_NAME, COL.ORDINAL_POSITION";

            return ExecuteAndParseMetadataAsync(database, sql, parameters, (wrapper, reader) => SetDataType(SetColumnType(new Column
            {
                Catalog = wrapper!.GetString(reader, 0),
                Schema = wrapper.GetString(reader, 1),
                TableName = wrapper.GetString(reader, 2),
                Name = wrapper.GetString(reader, 4),
                DataType = wrapper.GetString(reader, 5),
                Length = reader.IsDBNull(6) ? (long?)null : wrapper.GetInt64(reader, 6),
                NumericPrecision = reader.IsDBNull(7) ? (int?)null : wrapper.GetInt32(reader, 7),
                NumericScale = reader.IsDBNull(8) ? (int?)null : wrapper.GetInt32(reader, 8),
                IsNullable = wrapper.GetString(reader, 9) == "YES",
                IsPrimaryKey = wrapper.GetString(reader, 12) == "YES",
                Default = wrapper.GetString(reader, 10),
                Description = wrapper.GetString(reader, 11),
            })));
        }

        /// <summary>
        /// 获取 <see cref="ForeignKey"/> 元数据序列。
        /// </summary>
        /// <param name="database"></param>
        /// <param name="restrictionValues"></param>
        /// <returns></returns>
        protected override IAsyncEnumerable<ForeignKey> GetForeignKeysAsync(IDatabase database, RestrictionDictionary restrictionValues)
        {
            var parameters = new ParameterCollection();
            var connpar = GetConnectionParameter(database);

            SpecialCommand sql = $@"
SELECT
  TC.CONSTRAINT_CATALOG,
  TC.CONSTRAINT_SCHEMA,
  TC.CONSTRAINT_NAME,
  TC.TABLE_NAME,
  KCU.COLUMN_NAME,
  CCU.TABLE_NAME AS FOREIGN_TABLE_NAME,
  CCU.COLUMN_NAME AS FOREIGN_COLUMN_NAME,
  TC.IS_DEFERRABLE,
  TC.INITIALLY_DEFERRED
FROM
  INFORMATION_SCHEMA.TABLE_CONSTRAINTS AS TC
JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE AS KCU ON TC.CONSTRAINT_NAME = KCU.CONSTRAINT_NAME
JOIN INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE AS CCU ON CCU.CONSTRAINT_NAME = TC.CONSTRAINT_NAME
WHERE (TC.CONSTRAINT_CATALOG = '{connpar.Database}' AND TC.TABLE_SCHEMA = ANY (CURRENT_SCHEMAS(false))) AND
  CONSTRAINT_TYPE = 'FOREIGN KEY' AND
  (TC.TABLE_NAME = @TABLENAME OR @TABLENAME IS NULL) AND 
  (TC.CONSTRAINT_NAME = @NAME OR @NAME IS NULL)";

            restrictionValues
                .Parameterize(parameters, "TABLENAME", nameof(ForeignKey.TableName))
                .Parameterize(parameters, "NAME", nameof(ForeignKey.Name));

            return ExecuteAndParseMetadataAsync(database, sql, parameters, (wrapper, reader) => new ForeignKey
            {
                Catalog = wrapper!.GetString(reader, 0),
                Schema = wrapper.GetString(reader, 1),
                Name = wrapper.GetString(reader, 2),
                TableName = wrapper.GetString(reader, 3),
                ColumnName = wrapper.GetString(reader, 4),
                PKTable = wrapper.GetString(reader, 5),
                PKColumn = wrapper.GetString(reader, 6),
            });
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
            var connpar = GetConnectionParameter(database);

            SpecialCommand sql = $@"
SELECT
  T.TABLE_CATALOG,
  T.TABLE_SCHEMA,
  T.TABLE_NAME,
  C.TABLE_COMMENT
FROM INFORMATION_SCHEMA.VIEWS T
JOIN (
  SELECT RELNAME AS TABLE_NAME, CAST(OBJ_DESCRIPTION(RELFILENODE, 'pg_class') AS VARCHAR) AS TABLE_COMMENT FROM PG_CLASS C 
  WHERE RELKIND = 'v'
  ) C
    ON t.TABLE_NAME = C.TABLE_NAME
WHERE (TABLE_CATALOG = '{connpar.Database}' AND TABLE_SCHEMA = ANY (CURRENT_SCHEMAS(false)))
  AND (T.TABLE_NAME = :NAME OR :NAME IS NULL)
 ORDER BY T.TABLE_CATALOG, T.TABLE_SCHEMA, T.TABLE_NAME";

            restrictionValues
                .Parameterize(parameters, "NAME", nameof(View.Name));

            return ExecuteAndParseMetadataAsync(database, sql, parameters, (wrapper, reader) => new View
            {
                Catalog = wrapper!.GetString(reader, 0),
                Schema = wrapper.GetString(reader, 1),
                Name = wrapper.GetString(reader, 2),
                Description = wrapper.GetString(reader, 3)
            });
        }

        /// <summary>
        /// 获取 <see cref="ViewColumn"/> 元数据序列。
        /// </summary>
        /// <param name="database"></param>
        /// <param name="restrictionValues"></param>
        /// <returns></returns>
        protected override IAsyncEnumerable<ViewColumn> GetViewColumnsAsync(IDatabase database, RestrictionDictionary restrictionValues)
        {
            var parameters = new ParameterCollection();
            var connpar = GetConnectionParameter(database);

            SpecialCommand sql = $@"
SELECT
    COL.TABLE_CATALOG,
    COL.TABLE_SCHEMA,
    COL.TABLE_NAME,
    COL.ORDINAL_POSITION,
    COL.COLUMN_NAME,
    COL.UDT_NAME,
    COL.CHARACTER_MAXIMUM_LENGTH,
    COL.NUMERIC_PRECISION,
    COL.NUMERIC_SCALE
FROM
    INFORMATION_SCHEMA.COLUMNS COL
JOIN (
  SELECT RELNAME AS TABLE_NAME, CAST(OBJ_DESCRIPTION(RELFILENODE, 'pg_class') AS VARCHAR) AS TABLE_COMMENT FROM PG_CLASS C 
  WHERE RELKIND = 'v' AND RELNAME NOT LIKE 'pg_%' AND RELNAME NOT LIKE 'sql_%'
  ) C
    ON col.TABLE_NAME = C.TABLE_NAME
WHERE (COL.TABLE_CATALOG = '{connpar.Database}' AND COL.TABLE_SCHEMA = ANY (CURRENT_SCHEMAS(false)))
  AND (COL.TABLE_NAME = :TABLENAME OR :TABLENAME IS NULL)
  AND (COL.COLUMN_NAME = :COLUMNNAME OR :COLUMNNAME IS NULL)
ORDER BY
  COL.TABLE_CATALOG, COL.TABLE_SCHEMA, COL.TABLE_NAME, COL.ORDINAL_POSITION";

            restrictionValues
                .Parameterize(parameters, "TABLENAME", nameof(ViewColumn.ViewName))
                .Parameterize(parameters, "COLUMNNAME", nameof(ViewColumn.Name));

            return ExecuteAndParseMetadataAsync(database, sql, parameters, (wrapper, reader) => SetDataType(new ViewColumn
            {
                Catalog = wrapper!.GetString(reader, 0),
                Schema = wrapper.GetString(reader, 1),
                ViewName = wrapper.GetString(reader, 2),
                Name = wrapper.GetString(reader, 4),
                DataType = wrapper.GetString(reader, 5),
                Length = reader.IsDBNull(6) ? (long?)null : wrapper.GetInt64(reader, 6),
                NumericPrecision = reader.IsDBNull(7) ? (int?)null : wrapper.GetInt32(reader, 7),
                NumericScale = reader.IsDBNull(8) ? (int?)null : wrapper.GetInt32(reader, 8)
            }));
        }
    }
}
