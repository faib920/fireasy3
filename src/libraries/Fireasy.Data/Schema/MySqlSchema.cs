// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920?126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Fireasy.Data.Schema
{
    /// <summary>
    /// MySql 数据库架构信息的获取方法。
    /// </summary>
    public sealed class MySqlSchema : SchemaBase
    {
        /// <summary>
        /// 初始化约定查询限制。
        /// </summary>
        protected override void InitializeRestrictions()
        {
            AddRestriction<Database>(s => s.Name);
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
        /// 初始化数据类型映射。
        /// </summary>
        protected override void InitializeDataTypes()
        {
            AddDataType("bigint", DbType.Int64, typeof(long));
            AddDataType("binary", DbType.Binary, typeof(byte[]));
            AddDataType("bit", DbType.Boolean, typeof(UInt64));
            AddDataType("blob", DbType.Binary, typeof(byte[]));
            AddDataType("char", DbType.String, typeof(string));
            AddDataType("date", DbType.Date, typeof(DateTime));
            AddDataType("datetime", DbType.DateTime, typeof(DateTime));
            AddDataType("decimal", DbType.Decimal, typeof(decimal));
            AddDataType("double", DbType.Double, typeof(double));
            AddDataType("enum", DbType.String, typeof(string));
            AddDataType("float", DbType.Single, typeof(float));
            AddDataType("geometry", DbType.Binary, typeof(byte[]));
            AddDataType("geometrycollection", DbType.Binary, typeof(byte[]));
            AddDataType("int", DbType.Int32, typeof(int));
            AddDataType("json", DbType.String, typeof(string));
            AddDataType("linestring", DbType.String, typeof(byte[]));
            AddDataType("longblob", DbType.Binary, typeof(byte[]));
            AddDataType("longtext", DbType.String, typeof(string));
            AddDataType("mediumblob", DbType.Binary, typeof(byte[]));
            AddDataType("mediumint", DbType.Int32, typeof(int));
            AddDataType("mediumtext", DbType.String, typeof(string));
            AddDataType("multilinestring", DbType.Binary, typeof(byte[]));
            AddDataType("multipoint", DbType.Binary, typeof(byte[]));
            AddDataType("multipolygon", DbType.Binary, typeof(byte[]));
            AddDataType("numeric", DbType.Decimal, typeof(decimal));
            AddDataType("point", DbType.Binary, typeof(byte[]));
            AddDataType("polygon", DbType.Binary, typeof(byte[]));
            AddDataType("set", DbType.String, typeof(string));
            AddDataType("smallint", DbType.Int16, typeof(short));
            AddDataType("text", DbType.String, typeof(string));
            AddDataType("time", DbType.DateTimeOffset, typeof(TimeSpan));
            AddDataType("timestamp", DbType.DateTime, typeof(DateTime));
            AddDataType("tinyblob", DbType.Binary, typeof(byte[]));
            AddDataType("tinyint", DbType.SByte, typeof(sbyte));
            AddDataType("tinytext", DbType.String, typeof(string));
            AddDataType("varbinary", DbType.Binary, typeof(byte[]));
            AddDataType("varchar", DbType.String, typeof(string));
        }

        /// <summary>
        /// 获取 <see cref="Database"/> 元数据序列。
        /// </summary>
        /// <param name="database"></param>
        /// <param name="restrictionValues"></param>
        /// <returns></returns>
        protected override IAsyncEnumerable<Database> GetDatabasesAsync(IDatabase database, RestrictionDictionary restrictionValues)
        {
            var sql = "SHOW DATABASES";

            if (restrictionValues.TryGetValue(nameof(Database.Name), out string dbName))
            {
                sql += $" LIKE '{dbName}'";
            }

            return ExecuteAndParseMetadataAsync(database, sql, null, (wrapper, reader) => new Database
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

            SpecialCommand sql = "SELECT HOST, USER FROM MYSQL.USER WHERE (USER = ?NAME OR ?NAME IS NULL)";

            restrictionValues.Parameterize(parameters, "NAME", nameof(User.Name));

            return ExecuteAndParseMetadataAsync(database, sql, parameters, (wrapper, reader) => new User
            {
                Name = wrapper!.GetString(reader, 1)
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
                .Parameterize(parameters, "NAME", nameof(Table.Name))
                .Parameterize(parameters, "TABLETYPE", nameof(Table.Type));

            SpecialCommand sql = $@"
SELECT
  TABLE_CATALOG,
  TABLE_SCHEMA,
  TABLE_NAME,
  TABLE_TYPE,
  TABLE_COMMENT
FROM INFORMATION_SCHEMA.TABLES T
WHERE (T.TABLE_SCHEMA = '{connpar.Database}')
  AND T.TABLE_TYPE <> 'VIEW'{(parameters.HasValue("NAME") ? @"
  AND T.TABLE_NAME IN (?NAME)" : string.Empty)}
  AND ((T.TABLE_TYPE = 'BASE TABLE' AND (@TABLETYPE IS NULL OR @TABLETYPE = 0)) OR (T.TABLE_TYPE = 'SYSTEM TABLE' AND @TABLETYPE = 1))
ORDER BY T.TABLE_CATALOG, T.TABLE_SCHEMA, T.TABLE_NAME";

            return ExecuteAndParseMetadataAsync(database, sql, parameters, (wrapper, reader) => new Table
            {
                Schema = wrapper!.GetString(reader, 1),
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
SELECT T.TABLE_CATALOG,
       T.TABLE_SCHEMA,
       T.TABLE_NAME,
       T.COLUMN_NAME,
       T.DATA_TYPE,
       T.CHARACTER_MAXIMUM_LENGTH,
       T.NUMERIC_PRECISION,
       T.NUMERIC_SCALE,
       T.IS_NULLABLE,
       T.COLUMN_KEY,
       T.COLUMN_DEFAULT,
       T.COLUMN_COMMENT,
       T.EXTRA,
       T.COLUMN_TYPE
FROM INFORMATION_SCHEMA.COLUMNS T
JOIN INFORMATION_SCHEMA.TABLES O
  ON O.TABLE_SCHEMA = T.TABLE_SCHEMA AND O.TABLE_NAME = T.TABLE_NAME
WHERE (T.TABLE_SCHEMA = '{connpar.Database}'){(parameters.HasValue("TABLENAME") ? @"
  AND T.TABLE_NAME IN (?TABLENAME)" : string.Empty)}{(parameters.HasValue("COLUMNNAME") ? @"
  AND T.COLUMN_NAME IN (?COLUMNNAME)" : string.Empty)}
 ORDER BY T.TABLE_CATALOG, T.TABLE_SCHEMA, T.TABLE_NAME, T.ORDINAL_POSITION";

            return ExecuteAndParseMetadataAsync(database, sql, parameters, (wrapper, reader) => SetDataType(SetColumnType(new Column
            {
                Catalog = wrapper!.GetString(reader, 0),
                Schema = wrapper.GetString(reader, 1),
                TableName = wrapper.GetString(reader, 2),
                Name = wrapper.GetString(reader, 3),
                DataType = wrapper.GetString(reader, 4),
                Length = reader.IsDBNull(5) ? (long?)null : wrapper.GetInt64(reader, 5),
                NumericPrecision = reader.IsDBNull(6) ? (int?)null : wrapper.GetInt32(reader, 6),
                NumericScale = reader.IsDBNull(7) ? (int?)null : wrapper.GetInt32(reader, 7),
                IsNullable = wrapper.GetString(reader, 8) == "YES",
                IsPrimaryKey = wrapper.GetString(reader, 9) == "PRI",
                Default = wrapper.GetString(reader, 10),
                Description = wrapper.GetString(reader, 11),
                Autoincrement = !wrapper.IsDbNull(reader, 12) && wrapper.GetString(reader, 12).IndexOf("auto_increment") != -1,
                ColumnType = wrapper.GetString(reader, 13)
            })));
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
SELECT T.TABLE_CATALOG,
  T.TABLE_SCHEMA,
  T.TABLE_NAME
FROM 
  INFORMATION_SCHEMA.VIEWS T
WHERE (T.TABLE_SCHEMA = '{connpar.Database}') AND 
  (T.TABLE_NAME = ?NAME OR ?NAME IS NULL)
 ORDER BY T.TABLE_CATALOG, T.TABLE_SCHEMA, T.TABLE_NAME";

            restrictionValues.Parameterize(parameters, "NAME", nameof(View.Name));

            return ExecuteAndParseMetadataAsync(database, sql, parameters, (wrapper, reader) => new View
            {
                Catalog = wrapper!.GetString(reader, 0),
                Schema = wrapper.GetString(reader, 1),
                Name = wrapper.GetString(reader, 2)
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
SELECT T.TABLE_CATALOG,
       T.TABLE_SCHEMA,
       T.TABLE_NAME,
       T.COLUMN_NAME,
       T.DATA_TYPE,
       T.CHARACTER_MAXIMUM_LENGTH,
       T.NUMERIC_PRECISION,
       T.NUMERIC_SCALE,
       T.IS_NULLABLE,
       T.COLUMN_KEY,
       T.COLUMN_DEFAULT,
       T.COLUMN_COMMENT,
       T.EXTRA
FROM INFORMATION_SCHEMA.COLUMNS T
JOIN INFORMATION_SCHEMA.VIEWS O
  ON O.TABLE_SCHEMA = T.TABLE_SCHEMA AND O.TABLE_NAME = T.TABLE_NAME
WHERE (T.TABLE_SCHEMA = '{connpar.Database}') AND 
  (T.TABLE_NAME = ?TABLENAME OR ?TABLENAME IS NULL) AND 
  (T.COLUMN_NAME = ?COLUMNNAME OR ?COLUMNNAME IS NULL)
 ORDER BY T.TABLE_CATALOG, T.TABLE_SCHEMA, T.TABLE_NAME, T.ORDINAL_POSITION";

            restrictionValues
                .Parameterize(parameters, "TABLENAME", nameof(ViewColumn.ViewName))
                .Parameterize(parameters, "COLUMNNAME", nameof(ViewColumn.Name));

            return ExecuteAndParseMetadataAsync(database, sql, parameters, (wrapper, reader) => SetDataType(new ViewColumn
            {
                Catalog = wrapper!.GetString(reader, 0),
                Schema = wrapper.GetString(reader, 1),
                ViewName = wrapper.GetString(reader, 2),
                Name = wrapper.GetString(reader, 3),
                DataType = wrapper.GetString(reader, 4),
                Length = reader.IsDBNull(5) ? (long?)null : wrapper.GetInt64(reader, 5),
                NumericPrecision = reader.IsDBNull(6) ? (int?)null : wrapper.GetInt32(reader, 6),
                NumericScale = reader.IsDBNull(7) ? (int?)null : wrapper.GetInt32(reader, 7),
                IsNullable = wrapper.GetString(reader, 8) == "YES",
                IsPrimaryKey = wrapper.GetString(reader, 9) == "PRI",
                Default = wrapper.GetString(reader, 10),
                Description = wrapper.GetString(reader, 11),
                Autoincrement = false
            }));
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
    T.CONSTRAINT_CATALOG, 
    T.CONSTRAINT_SCHEMA, 
    T.CONSTRAINT_NAME,
    T.TABLE_NAME, 
    T.COLUMN_NAME,     
    T.REFERENCED_TABLE_NAME, 
    T.REFERENCED_COLUMN_NAME 
FROM  
    INFORMATION_SCHEMA.KEY_COLUMN_USAGE T
WHERE (T.CONSTRAINT_SCHEMA = '{connpar.Database}') AND 
   (T.TABLE_NAME = ?TABLENAME OR ?TABLENAME IS NULL) AND 
   (T.CONSTRAINT_NAME = ?NAME OR ?NAME IS NULL) AND
   REFERENCED_TABLE_NAME IS NOT NULL";

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
        /// 获取 <see cref="Procedure"/> 元数据序列。
        /// </summary>
        /// <param name="database"></param>
        /// <param name="restrictionValues"></param>
        /// <returns></returns>
        protected override IAsyncEnumerable<Procedure> GetProceduresAsync(IDatabase database, RestrictionDictionary restrictionValues)
        {
            var parameters = new ParameterCollection();
            var connpar = GetConnectionParameter(database);

            SpecialCommand sql = $@"
SELECT
  SPECIFIC_NAME,
  ROUTINE_CATALOG,
  ROUTINE_SCHEMA,
  ROUTINE_NAME,
  ROUTINE_TYPE
FROM INFORMATION_SCHEMA.ROUTINES
WHERE (ROUTINE_SCHEMA = '{connpar.Database}')
  AND (ROUTINE_NAME = @NAME OR (@NAME IS NULL))
  AND (ROUTINE_TYPE = @TYPE OR (@TYPE IS NULL))
ORDER BY ROUTINE_CATALOG, ROUTINE_SCHEMA, ROUTINE_NAME";

            restrictionValues
                .Parameterize(parameters, "NAME", nameof(Procedure.Name))
                .Parameterize(parameters, "TYPE", nameof(Procedure.Type));

            return ExecuteAndParseMetadataAsync(database, sql, parameters, (wrapper, reader) => new Procedure
            {
                Catalog = wrapper!.GetString(reader, 0),
                Schema = wrapper.GetString(reader, 1),
                Name = wrapper.GetString(reader, 2),
                Type = wrapper.GetString(reader, 6)
            });
        }

        /// <summary>
        /// 获取 <see cref="ProcedureParameter"/> 元数据序列。
        /// </summary>
        /// <param name="database"></param>
        /// <param name="restrictionValues"></param>
        /// <returns></returns>
        protected override IAsyncEnumerable<ProcedureParameter> GetProcedureParametersAsync(IDatabase database, RestrictionDictionary restrictionValues)
        {
            var parameters = new ParameterCollection();
            var connpar = GetConnectionParameter(database);

            SpecialCommand sql = $@"
SELECT
  SPECIFIC_CATALOG,
  SPECIFIC_SCHEMA,
  SPECIFIC_NAME,
  ORDINAL_POSITION,
  PARAMETER_MODE,
  PARAMETER_NAME,
  DATA_TYPE,
  CHARACTER_MAXIMUM_LENGTH,
  NUMERIC_PRECISION,
  NUMERIC_SCALE
WHERE (SPECIFIC_SCHEMA = '{connpar.Database}')
  AND (SPECIFIC_NAME = @NAME OR (@NAME IS NULL))
  AND (PARAMETER_NAME = @PARAMETER OR (@PARAMETER IS NULL))
ORDER BY SPECIFIC_CATALOG, SPECIFIC_SCHEMA, SPECIFIC_NAME, PARAMETER_NAME";

            restrictionValues
                .Parameterize(parameters, "NAME", nameof(ProcedureParameter.ProcedureName))
                .Parameterize(parameters, "PARAMETER", nameof(ProcedureParameter.Name));

            return ExecuteAndParseMetadataAsync(database, sql, parameters, (wrapper, reader) => new ProcedureParameter
            {
                Catalog = wrapper!.GetString(reader, 0),
                Schema = wrapper.GetString(reader, 1),
                ProcedureName = wrapper.GetString(reader, 2),
                Name = wrapper.GetString(reader, 5),
                Direction = wrapper.GetString(reader, 4) == "IN" ? ParameterDirection.Input : ParameterDirection.Output,
                NumericPrecision = wrapper.GetInt32(reader, 8),
                NumericScale = wrapper.GetInt32(reader, 9),
                DataType = wrapper.GetString(reader, 6),
                Length = wrapper.GetInt64(reader, 7)
            });
        }
    }
}
