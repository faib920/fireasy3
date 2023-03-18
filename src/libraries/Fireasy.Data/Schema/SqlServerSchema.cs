// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Fireasy.Data.Schema
{
    /// <summary>
    /// SqlServer 数据库架构信息的获取方法。
    /// </summary>
    public sealed class SqlServerSchema : SchemaBase
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
            AddDataType("bit", DbType.Boolean, typeof(bool));
            AddDataType("char", DbType.String, typeof(string));
            AddDataType("date", DbType.Date, typeof(DateTime));
            AddDataType("datetime", DbType.DateTime, typeof(DateTime));
            AddDataType("datetime2", DbType.DateTime2, typeof(DateTime));
            AddDataType("datetimeoffset", DbType.DateTimeOffset, typeof(DateTimeOffset));
            AddDataType("decimal", DbType.Decimal, typeof(decimal));
            AddDataType("float", DbType.Double, typeof(double));
            AddDataType("image", DbType.Binary, typeof(byte[]));
            AddDataType("int", DbType.Int32, typeof(int));
            AddDataType("money", DbType.Decimal, typeof(decimal));
            AddDataType("nchar", DbType.String, typeof(string));
            AddDataType("ntext", DbType.String, typeof(string));
            AddDataType("numeric", DbType.Decimal, typeof(decimal));
            AddDataType("nvarchar", DbType.String, typeof(string));
            AddDataType("real", DbType.Single, typeof(float));
            AddDataType("smalldatetime", DbType.DateTime, typeof(DateTime));
            AddDataType("smallint", DbType.Int16, typeof(short));
            AddDataType("smallmoney", DbType.Decimal, typeof(decimal));
            AddDataType("sql_variant", DbType.Object, typeof(object));
            AddDataType("text", DbType.String, typeof(string));
            AddDataType("time", DbType.Time, typeof(TimeSpan));
            AddDataType("timestamp", DbType.Binary, typeof(byte[]));
            AddDataType("tinyint", DbType.Byte, typeof(byte));
            AddDataType("uniqueidentifier", DbType.Guid, typeof(Guid));
            AddDataType("varbinary", DbType.Binary, typeof(byte[]));
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
SELECT NAME AS DATABASE_NAME, CRDATE AS CREATE_DATE FROM MASTER..SYSDATABASES WHERE (NAME = @NAME OR (@NAME IS NULL))";

            restrictionValues.Parameterize(parameters, "NAME", nameof(Database.Name));

            return ExecuteAndParseMetadataAsync(database, sql, parameters, (wrapper, reader) => new Database
            {
                Name = wrapper!.GetString(reader, 0),
                CreateDate = wrapper.GetDateTime(reader, 1)
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

            SpecialCommand sql = @"
SELECT UID, NAME AS USER_NAME, CREATEDATE, UPDATEDATE
FROM SYSUSERS
WHERE (NAME = @NAME OR (@NAME IS NULL))";

            restrictionValues.Parameterize(parameters, "NAME", nameof(User.Name));

            return ExecuteAndParseMetadataAsync(database, sql, parameters, (wrapper, reader) => new User
            {
                Name = wrapper!.GetString(reader, 1),
                CreateDate = wrapper.GetDateTime(reader, 2)
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
SELECT T.TABLE_CATALOG, 
  T.TABLE_SCHEMA, 
  T.TABLE_NAME, 
  T.TABLE_TYPE,
  (SELECT VALUE FROM ::FN_LISTEXTENDEDPROPERTY('MS_Description','user',T.TABLE_SCHEMA,'table',T.TABLE_NAME,NULL,NULL)) COMMENTS
FROM 
  INFORMATION_SCHEMA.TABLES T
WHERE TABLE_TYPE <> 'view'{(parameters.HasValue("NAME") ? @"
  AND T.TABLE_NAME IN (@NAME)" : string.Empty)}
  AND ((T.TABLE_TYPE = 'BASE TABLE' AND (@TABLETYPE IS NULL OR @TABLETYPE = 0)) OR (T.TABLE_TYPE = 'SYSTEM TABLE' AND @TABLETYPE = 1))
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
SELECT T.TABLE_CATALOG,
       T.TABLE_SCHEMA,
       T.TABLE_NAME,
       T.COLUMN_NAME,
       T.DATA_TYPE AS DATATYPE,
       T.CHARACTER_MAXIMUM_LENGTH AS LENGTH,
       T.NUMERIC_PRECISION AS PRECISION,
       T.NUMERIC_SCALE AS SCALE,
       T.IS_NULLABLE AS NULLABLE,
       (SELECT COUNT(1) FROM SYSCOLUMNS A
            JOIN SYSINDEXKEYS B ON A.ID=B.ID AND A.COLID=B.COLID AND A.ID=OBJECT_ID(T.TABLE_NAME)
            JOIN SYSINDEXES C ON A.ID=C.ID AND B.INDID=C.INDID JOIN SYSOBJECTS D ON C.NAME=D.NAME AND D.XTYPE= 'PK' WHERE A.NAME = T.COLUMN_NAME) COLUMN_IS_PK,
       T.COLUMN_DEFAULT,
       (SELECT VALUE FROM ::FN_LISTEXTENDEDPROPERTY('MS_Description','user',T.TABLE_SCHEMA,'table',T.TABLE_NAME,'column',T.COLUMN_NAME)) COMMENTS,
       (SELECT C.COLSTAT FROM SYSCOLUMNS C
            LEFT JOIN SYSOBJECTS O ON C.ID = O.ID WHERE O.XTYPE='U' AND O.NAME = T.TABLE_NAME AND C.NAME = T.COLUMN_NAME) AUTOINC
  FROM INFORMATION_SCHEMA.COLUMNS T
  JOIN INFORMATION_SCHEMA.TABLES O
    ON O.TABLE_CATALOG = T.TABLE_CATALOG AND O.TABLE_SCHEMA = T.TABLE_SCHEMA AND T.TABLE_NAME = O.TABLE_NAME
WHERE O.TABLE_TYPE <> 'view'{(parameters.HasValue("TABLENAME") ? @"
  AND T.TABLE_NAME IN (@TABLENAME)" : string.Empty)}{(parameters.HasValue("COLUMNNAME") ? @"
  AND T.COLUMN_NAME IN (@COLUMNNAME)" : string.Empty)}
 ORDER BY T.TABLE_CATALOG, T.TABLE_SCHEMA, T.TABLE_NAME, T.ORDINAL_POSITION";

            return ExecuteAndParseMetadataAsync(database, sql, parameters, (wrapper, reader) => SetDataType(SetColumnType(new Column
            {
                Catalog = wrapper!.GetString(reader, 0),
                Schema = wrapper.GetString(reader, 1),
                TableName = wrapper.GetString(reader, 2),
                Name = wrapper.GetString(reader, 3),
                DataType = wrapper.GetString(reader, 4),
                Length = reader.IsDBNull(5) ? (long?)null : wrapper.GetInt32(reader, 5),
                NumericPrecision = reader.IsDBNull(6) ? (int?)null : wrapper.GetByte(reader, 6),
                NumericScale = reader.IsDBNull(7) ? (int?)null : wrapper.GetInt32(reader, 7),
                IsNullable = wrapper.GetString(reader, 8) == "YES",
                IsPrimaryKey = wrapper.GetInt32(reader, 9) > 0,
                Default = wrapper.GetString(reader, 10),
                Description = wrapper.GetString(reader, 11),
                Autoincrement = wrapper.GetInt16(reader, 12) == 1
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

            restrictionValues.Parameterize(parameters, "VIEWNAME", nameof(View.Name));

            SpecialCommand sql = $@"
SELECT T.TABLE_CATALOG,
  T.TABLE_SCHEMA,
  T.TABLE_NAME, 
  (SELECT VALUE FROM ::FN_LISTEXTENDEDPROPERTY('MS_Description','user',t.TABLE_SCHEMA,'view',T.TABLE_NAME,NULL,NULL)) COMMENTS
FROM 
  INFORMATION_SCHEMA.TABLES T
WHERE TABLE_TYPE = 'view'{(parameters.HasValue("VIEWNAME") ? @"
  AND T.TABLE_NAME IN (@VIEWNAME)" : string.Empty)}
 ORDER BY T.TABLE_CATALOG, T.TABLE_SCHEMA, T.TABLE_NAME";

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

            restrictionValues
                .Parameterize(parameters, "VIEWNAME", nameof(ViewColumn.ViewName))
                .Parameterize(parameters, "COLUMNNAME", nameof(ViewColumn.Name));

            SpecialCommand sql = $@"
SELECT T.TABLE_CATALOG,
       T.TABLE_SCHEMA,
       T.TABLE_NAME,
       T.COLUMN_NAME,
       T.DATA_TYPE AS DATATYPE,
       T.CHARACTER_MAXIMUM_LENGTH AS LENGTH,
       T.NUMERIC_PRECISION AS PRECISION,
       T.NUMERIC_SCALE AS SCALE,
       T.IS_NULLABLE AS NULLABLE,
       (SELECT COUNT(1) FROM SYSCOLUMNS A
            JOIN SYSINDEXKEYS B ON A.ID=B.ID AND A.COLID=B.COLID AND A.ID=OBJECT_ID(T.TABLE_NAME)
            JOIN SYSINDEXES C ON A.ID=C.ID AND B.INDID=C.INDID JOIN SYSOBJECTS D ON C.NAME=D.NAME AND D.XTYPE= 'PK' WHERE A.NAME = T.COLUMN_NAME) COLUMN_IS_PK,
       T.COLUMN_DEFAULT,
       (SELECT VALUE FROM ::FN_LISTEXTENDEDPROPERTY('MS_Description','user',T.TABLE_SCHEMA,'table',T.TABLE_NAME,'column',T.COLUMN_NAME)) COMMENTS,
       0 AUTOINC
  FROM INFORMATION_SCHEMA.COLUMNS T
  JOIN INFORMATION_SCHEMA.TABLES O
    ON O.TABLE_CATALOG = T.TABLE_CATALOG AND O.TABLE_SCHEMA = T.TABLE_SCHEMA AND T.TABLE_NAME = O.TABLE_NAME
WHERE O.TABLE_TYPE = 'view'{(parameters.HasValue("VIEWNAME") ? @"
  AND T.TABLE_NAME IN (@VIEWNAME)" : string.Empty)}{(parameters.HasValue("COLUMNNAME") ? @"
  AND T.COLUMN_NAME IN (@COLUMNNAME)" : string.Empty)}
 ORDER BY T.TABLE_CATALOG, T.TABLE_SCHEMA, T.TABLE_NAME, T.ORDINAL_POSITION";

            return ExecuteAndParseMetadataAsync(database, sql, parameters, (wrapper, reader) => new ViewColumn
            {
                Catalog = wrapper!.GetString(reader, 0),
                Schema = wrapper.GetString(reader, 1),
                ViewName = wrapper.GetString(reader, 2),
                Name = wrapper.GetString(reader, 3),
                DataType = wrapper.GetString(reader, 4),
                Length = reader.IsDBNull(5) ? (long?)null : wrapper.GetInt32(reader, 5),
                NumericPrecision = reader.IsDBNull(6) ? (int?)null : wrapper.GetByte(reader, 6),
                NumericScale = reader.IsDBNull(7) ? (int?)null : wrapper.GetInt32(reader, 7),
                IsNullable = wrapper.GetString(reader, 8) == "YES",
                IsPrimaryKey = wrapper.GetInt32(reader, 9) > 0,
                Default = wrapper.GetString(reader, 10),
                Description = wrapper.GetString(reader, 11),
                Autoincrement = wrapper.GetInt16(reader, 12) == 1
            });
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

            restrictionValues
                .Parameterize(parameters, "TABLENAME", nameof(ForeignKey.TableName))
                .Parameterize(parameters, "NAME", nameof(ForeignKey.Name));

            SpecialCommand sql = $@"
SELECT
  TC.CONSTRAINT_CATALOG, 
  TC.CONSTRAINT_SCHEMA, 
  TC.CONSTRAINT_NAME, 
  TC.TABLE_NAME,
  C.COLUMN_NAME,
  FKCU.TABLE_NAME REFERENCED_TABLE_NAME, 
  FKCU.COLUMN_NAME AS REFERENCED_COLUMN_NAME
FROM [INFORMATION_SCHEMA].[COLUMNS] C
LEFT JOIN [INFORMATION_SCHEMA].[KEY_COLUMN_USAGE] KCU
  ON KCU.TABLE_SCHEMA = C.TABLE_SCHEMA
  AND KCU.TABLE_NAME = C.TABLE_NAME
  AND KCU.COLUMN_NAME = C.COLUMN_NAME
LEFT JOIN [INFORMATION_SCHEMA].[TABLE_CONSTRAINTS] TC
  ON TC.CONSTRAINT_SCHEMA = KCU.CONSTRAINT_SCHEMA
AND TC.CONSTRAINT_NAME = KCU.CONSTRAINT_NAME
LEFT JOIN [INFORMATION_SCHEMA].[REFERENTIAL_CONSTRAINTS] FC
  ON KCU.CONSTRAINT_SCHEMA = FC.CONSTRAINT_SCHEMA
AND KCU.CONSTRAINT_NAME = FC.CONSTRAINT_NAME
LEFT JOIN [INFORMATION_SCHEMA].[KEY_COLUMN_USAGE] FKCU
  ON FKCU.CONSTRAINT_SCHEMA = FC.UNIQUE_CONSTRAINT_SCHEMA
  AND FKCU.CONSTRAINT_NAME = FC.UNIQUE_CONSTRAINT_NAME
WHERE TC.CONSTRAINT_TYPE = 'FOREIGN KEY'{(parameters.HasValue("TABLENAME") ? @"
  AND TC.TABLE_NAME IN (@TABLENAME)" : string.Empty)}{(parameters.HasValue("NAME") ? @"
  AND TC.CONSTRAINT_NAME IN (@NAME)" : string.Empty)}";

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
        /// 获取 <see cref="Index"/> 元数据序列。
        /// </summary>
        /// <param name="database"></param>
        /// <param name="restrictionValues"></param>
        /// <returns></returns>
        protected override IAsyncEnumerable<Index> GetIndexsAsync(IDatabase database, RestrictionDictionary restrictionValues)
        {
            var parameters = new ParameterCollection();
            var connpar = GetConnectionParameter(database);

            restrictionValues
                .Parameterize(parameters, "TABLENAME", nameof(Index.TableName))
                .Parameterize(parameters, "INDEXNAME", nameof(Index.Name));

            SpecialCommand sql = $@"
SELECT DISTINCT
  DB_NAME() AS CONSTRAINT_CATALOG,
  CONSTRAINT_SCHEMA = USER_NAME(O.UID),
  CONSTRAINT_NAME = X.NAME,
  TABLE_CATALOG  = DB_NAME(),
  TABLE_SCHEMA = USER_NAME(O.UID),
  TABLE_NAME = O.NAME,
  INDEX_NAME = X.NAME
FROM SYSOBJECTS O, SYSINDEXES X, SYSINDEXKEYS XK
WHERE O.TYPE IN ('U') AND X.ID = O.ID  AND O.ID = XK.ID AND X.INDID = XK.INDID AND XK.KEYNO <= X.KEYCNT
  {(parameters.HasValue("TABLENAME") ? @"
  AND O.NAME IN (@TABLENAME)" : string.Empty)}{(parameters.HasValue("INDEXNAME") ? @"
  AND X.NAME IN (@INDEXNAME)" : string.Empty)}
ORDER BY TABLE_NAME, INDEX_NAME";

            return ExecuteAndParseMetadataAsync(database, sql, parameters, (wrapper, reader) => new Index
            {
                Catalog = wrapper!.GetString(reader, 0),
                Schema = wrapper.GetString(reader, 1),
                TableName = wrapper.GetString(reader, 5),
                Name = wrapper.GetString(reader, 6)
            });
        }

        /// <summary>
        /// 获取 <see cref="IndexColumn"/> 元数据序列。
        /// </summary>
        /// <param name="database"></param>
        /// <param name="restrictionValues"></param>
        /// <returns></returns>
        protected override IAsyncEnumerable<IndexColumn> GetIndexColumnsAsync(IDatabase database, RestrictionDictionary restrictionValues)
        {
            var parameters = new ParameterCollection();
            var connpar = GetConnectionParameter(database);
            
            restrictionValues
                .Parameterize(parameters, "TABLENAME", nameof(IndexColumn.TableName))
                .Parameterize(parameters, "INDEXNAME", nameof(IndexColumn.IndexName))
                .Parameterize(parameters, "COLUMNNAME", nameof(IndexColumn.ColumnName));

            SpecialCommand sql = $@"
SELECT DISTINCT
  DB_NAME() AS CONSTRAINT_CATALOG,
  CONSTRAINT_SCHEMA = USER_NAME(O.UID),
  CONSTRAINT_NAME = X.NAME,
  TABLE_CATALOG  = DB_NAME(),
  TABLE_SCHEMA = USER_NAME(O.UID),
  TABLE_NAME = O.NAME,
  COLUMN_NAME = C.NAME,
  ORDINAL_POSITION = CONVERT(INT, XK.KEYNO),
  KEYTYPE = C.XTYPE, 
  INDEX_NAME = X.NAME
FROM SYSOBJECTS O, SYSINDEXES X, SYSCOLUMNS C, SYSINDEXKEYS XK
WHERE O.TYPE IN ('U') AND X.ID = O.ID  AND O.ID = C.ID AND O.ID = XK.ID AND X.INDID = XK.INDID AND C.COLID = XK.COLID AND XK.KEYNO <= X.KEYCNT AND PERMISSIONS(O.ID, C.NAME) <> 0 
  {(parameters.HasValue("TABLENAME") ? @"
  AND O.NAME IN (@TABLENAME)" : string.Empty)}{(parameters.HasValue("INDEXNAME") ? @"
  AND X.NAME IN (@INDEXNAME)" : string.Empty)}{(parameters.HasValue("COLUMNNAME") ? @"
  AND C.NAME IN (@COLUMNNAME)" : string.Empty)}
ORDER BY TABLE_NAME, INDEX_NAME";

            return ExecuteAndParseMetadataAsync(database, sql, parameters, (wrapper, reader) => new IndexColumn
            {
                Catalog = wrapper!.GetString(reader, 0),
                Schema = wrapper.GetString(reader, 1),
                TableName = wrapper.GetString(reader, 5),
                IndexName = wrapper.GetString(reader, 2),
                ColumnName = wrapper.GetString(reader, 6)
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

            restrictionValues
                .Parameterize(parameters, "NAME", nameof(Procedure.Name))
                .Parameterize(parameters, "TYPE", nameof(Procedure.Type));

            SpecialCommand sql = $@"
SELECT
  SPECIFIC_CATALOG,
  SPECIFIC_SCHEMA,
  SPECIFIC_NAME,
  ROUTINE_CATALOG,
  ROUTINE_SCHEMA,
  ROUTINE_NAME,
  ROUTINE_TYPE,
  CREATED,
  LAST_ALTERED
FROM INFORMATION_SCHEMA.ROUTINES
WHERE 1 = 1{(parameters.HasValue("NAME") ? @"
  AND SPECIFIC_NAME IN (@NAME)" : string.Empty)}
  AND ((ROUTINE_TYPE = 'PROCEDURE' AND (@TYPE IS NULL OR @TYPE = 0)) OR (ROUTINE_TYPE = 'FUNCTION' AND @TYPE = 1))
ORDER BY SPECIFIC_CATALOG, SPECIFIC_SCHEMA, SPECIFIC_NAME";

            return ExecuteAndParseMetadataAsync(database, sql, parameters, (wrapper, reader) => new Procedure
            {
                Catalog = wrapper!.GetString(reader, 0),
                Schema = wrapper.GetString(reader, 1),
                Name = wrapper.GetString(reader, 2),
                Type = wrapper.GetString(reader, 6) == "PROCEDURE" ? ProcedureType.Procedure : ProcedureType.Function
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

            restrictionValues
                .Parameterize(parameters, "NAME", nameof(ProcedureParameter.ProcedureName))
                .Parameterize(parameters, "PARAMETER", nameof(ProcedureParameter.Name));

            SpecialCommand sql = $@"
SELECT
  SPECIFIC_CATALOG,
  SPECIFIC_SCHEMA,
  SPECIFIC_NAME,
  ORDINAL_POSITION,
  PARAMETER_MODE,
  IS_RESULT,
  AS_LOCATOR,
  PARAMETER_NAME,
  CASE WHEN DATA_TYPE IS NULL THEN USER_DEFINED_TYPE_NAME WHEN DATA_TYPE = 'TABLE TYPE' THEN USER_DEFINED_TYPE_NAME ELSE DATA_TYPE END AS DATA_TYPE,
  CHARACTER_MAXIMUM_LENGTH,
  CHARACTER_OCTET_LENGTH,
  COLLATION_CATALOG,
  COLLATION_SCHEMA,
  COLLATION_NAME,
  CHARACTER_SET_CATALOG,
  CHARACTER_SET_SCHEMA,
  CHARACTER_SET_NAME,
  NUMERIC_PRECISION,
  NUMERIC_PRECISION_RADIX,
  NUMERIC_SCALE,
  DATETIME_PRECISION,
  INTERVAL_TYPE,
  INTERVAL_PRECISION
FROM INFORMATION_SCHEMA.PARAMETERS
WHERE 1 = 1{(parameters.HasValue("NAME") ? @"
  AND SPECIFIC_NAME IN (@NAME)" : string.Empty)}{(parameters.HasValue("PARAMETER") ? @"
  AND PARAMETER_NAME IN (@PARAMETER)" : string.Empty)}
ORDER BY SPECIFIC_CATALOG, SPECIFIC_SCHEMA, SPECIFIC_NAME, PARAMETER_NAME";

            return ExecuteAndParseMetadataAsync(database, sql, parameters, (wrapper, reader) => new ProcedureParameter
            {
                Catalog = wrapper!.GetString(reader, 0),
                Schema = wrapper.GetString(reader, 1),
                ProcedureName = wrapper.GetString(reader, 2),
                Name = wrapper.GetString(reader, 7),
                Direction = wrapper.GetString(reader, 5) == "YES" ? ParameterDirection.ReturnValue : (wrapper.GetString(reader, 4) == "IN" ? ParameterDirection.Input : ParameterDirection.Output),
                NumericPrecision = wrapper.GetInt32(reader, 17),
                NumericScale = wrapper.GetInt32(reader, 19),
                DataType = wrapper.GetString(reader, 8),
                Length = wrapper.GetInt64(reader, 9)
            });
        }
    }
}
