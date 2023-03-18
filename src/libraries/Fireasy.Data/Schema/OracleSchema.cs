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
    /// Oracle 数据库架构信息的获取方法。
    /// </summary>
    public sealed class OracleSchema : SchemaBase
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
            AddRestriction<Procedure>(s => s.Name);
            AddRestriction<ProcedureParameter>(s => s.ProcedureName);
            AddRestriction<Index>(s => s.TableName, s => s.Name);
            AddRestriction<IndexColumn>(s => s.TableName, s => s.IndexName, s => s.ColumnName);
            AddRestriction<ForeignKey>(s => s.TableName, s => s.Name);
        }

        /// <summary>
        /// 初始化数据类型映射。
        /// </summary>
        protected override void InitializeDataTypes()
        {
            AddDataType("bfile", DbType.Binary, typeof(byte[]));
            AddDataType("blob", DbType.Binary, typeof(byte[]));
            AddDataType("binary_float", DbType.Single, typeof(float));
            AddDataType("binary_double", DbType.Double, typeof(double));
            AddDataType("char", DbType.String, typeof(string));
            AddDataType("clob", DbType.String, typeof(string));
            AddDataType("date", DbType.Date, typeof(DateTime));
            AddDataType("float", DbType.Decimal, typeof(decimal));
            AddDataType(@"interval day\([\d+]\) to second\([\d+]\)", DbType.Time, typeof(TimeSpan), true);
            AddDataType(@"interval year\([\d+]\) to month", DbType.Int64, typeof(long), true);
            AddDataType("long", DbType.String, typeof(string));
            AddDataType("long raw", DbType.Binary, typeof(byte[]));
            AddDataType("nchar", DbType.String, typeof(string));
            AddDataType("nclob", DbType.String, typeof(string));
            AddDataType("number", DbType.Decimal, typeof(decimal));
            AddDataType("nvarchar2", DbType.String, typeof(string));
            AddDataType("raw", DbType.Binary, typeof(byte[]));
            AddDataType("rowid", DbType.String, typeof(string));
            AddDataType(@"timestamp\([\d+]\)", DbType.DateTime, typeof(DateTime), true);
            AddDataType(@"timestamp\([\d+]\) with local time zone", DbType.DateTime, typeof(DateTime), true);
            AddDataType(@"timestamp\([\d+]\) with time zone", DbType.DateTime, typeof(DateTime), true);
            AddDataType("varchar2", DbType.String, typeof(string));
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
SELECT NAME, CREATED FROM V$DATABASE T
WHERE (T.NAME = :NAME OR (:NAME IS NULL))";

            restrictionValues
                .Parameterize(parameters, "NAME", nameof(Database.Name));

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
SELECT USERNAME FROM USER_USERS T
WHERE (T.USERNAME = :USERNAME OR (:USERNAME IS NULL))";

            restrictionValues
                .Parameterize(parameters, "USERNAME", nameof(User.Name));

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
                .Parameterize(parameters, "TABLENAME", nameof(Table.Name))
                .Parameterize(parameters, "TABLETYPE", nameof(Table.Type));

            SpecialCommand sql = $@"
SELECT * FROM (
    SELECT T.OWNER,
       T.TABLE_NAME,
       DECODE(T.OWNER,
              'SYS',
              'SYSTEM',
              'SYSTEM',
              'SYSTEM',
              'SYSMAN',
              'SYSTEM',
              'CTXSYS',
              'SYSTEM',
              'MDSYS',
              'SYSTEM',
              'OLAPSYS',
              'SYSTEM',
              'ORDSYS',
              'SYSTEM',
              'OUTLN',
              'SYSTEM',
              'WKSYS',
              'SYSTEM',
              'WMSYS',
              'SYSTEM',
              'XDB',
              'SYSTEM',
              'ORDPLUGINS',
              'SYSTEM',
              'USER') AS TYPE,
       C.COMMENTS
  FROM ALL_TABLES T
  JOIN ALL_TAB_COMMENTS C
    ON T.OWNER = C.OWNER
   AND T.TABLE_NAME = C.TABLE_NAME
) T
 WHERE (T.OWNER = '{connpar.UserId!.ToUpper()}'){(parameters.HasValue("TABLENAME") ? @"
  AND T.TABLE_NAME IN (:TABLENAME)" : string.Empty)}
  AND ((T.TYPE = 'USER' AND (:TABLETYPE IS NULL OR :TABLETYPE = 0)) OR (T.TYPE = 'SYSTEM' AND :TABLETYPE = 1))
ORDER BY OWNER, TABLE_NAME";

            return ExecuteAndParseMetadataAsync(database, sql, parameters, (wrapper, reader) => new Table
            {
                Schema = wrapper!.GetString(reader, 0),
                Name = wrapper.GetString(reader, 1),
                Type = wrapper.GetString(reader, 2) == "USER" ? TableType.BaseTable : TableType.SystemTable,
                Description = wrapper.GetString(reader, 3)
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
SELECT T.OWNER,
       T.TABLE_NAME,
       T.COLUMN_NAME,
       T.DATA_TYPE AS DATATYPE,
       T.DATA_LENGTH AS LENGTH,
       T.DATA_PRECISION AS PRECISION,
       T.DATA_SCALE AS SCALE,
       T.NULLABLE AS NULLABLE,
       (CASE
         WHEN P.OWNER IS NULL THEN
          'N'
         ELSE
          'Y'
       END) PK,
       D.DATA_DEFAULT,
       C.COMMENTS
  FROM ALL_TAB_COLUMNS T
  JOIN ALL_TABLES V
    ON T.OWNER = V.OWNER
   AND T.TABLE_NAME = V.TABLE_NAME
  LEFT JOIN ALL_COL_COMMENTS C
    ON T.OWNER = C.OWNER
   AND T.TABLE_NAME = C.TABLE_NAME
   AND T.COLUMN_NAME = C.COLUMN_NAME
  LEFT JOIN ALL_TAB_COLUMNS D
    ON T.OWNER = D.OWNER
   AND T.TABLE_NAME = D.TABLE_NAME
   AND T.COLUMN_NAME = D.COLUMN_NAME
  LEFT JOIN (SELECT AU.OWNER, AU.TABLE_NAME, CU.COLUMN_NAME
               FROM ALL_CONS_COLUMNS CU, ALL_CONSTRAINTS AU
              WHERE CU.OWNER = AU.OWNER
                AND CU.CONSTRAINT_NAME = AU.CONSTRAINT_NAME
                AND AU.CONSTRAINT_TYPE = 'P') P
    ON T.OWNER = P.OWNER
   AND T.TABLE_NAME =P.TABLE_NAME
   AND T.COLUMN_NAME = P.COLUMN_NAME
 WHERE (T.OWNER = '{connpar.UserId!.ToUpper()}'){(parameters.HasValue("TABLENAME") ? @"
  AND T.TABLE_NAME IN (:TABLENAME)" : string.Empty)}{(parameters.HasValue("COLUMNNAME") ? @"
  AND T.COLUMN_NAME IN (:COLUMNNAME)" : string.Empty)}
 ORDER BY T.OWNER, T.TABLE_NAME, T.COLUMN_ID";

            return ExecuteAndParseMetadataAsync(database, sql, parameters, (wrapper, reader) => SetDataType(SetColumnType(new Column
            {
                Schema = wrapper!.GetString(reader, 0),
                TableName = wrapper.GetString(reader, 1),
                Name = wrapper.GetString(reader, 2),
                DataType = wrapper.GetString(reader, 3),
                Length = reader.IsDBNull(4) ? (long?)null : wrapper.GetInt32(reader, 4),
                NumericPrecision = reader.IsDBNull(5) ? (int?)null : wrapper.GetInt32(reader, 5),
                NumericScale = reader.IsDBNull(6) ? (int?)null : wrapper.GetInt32(reader, 6),
                IsNullable = wrapper.GetString(reader, 7) == "Y",
                IsPrimaryKey = wrapper.GetString(reader, 8) == "Y",
                Default = wrapper.GetString(reader, 9),
                Description = wrapper.GetString(reader, 10),
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

            restrictionValues
                .Parameterize(parameters, "TABLENAME", nameof(ForeignKey.TableName))
                .Parameterize(parameters, "CONSTRAINTNAME", nameof(ForeignKey.Name));

            SpecialCommand sql = $@"
SELECT PKCON.CONSTRAINT_NAME AS PRIMARY_KEY_CONSTRAINT_NAME,
       PKCON.OWNER AS PRIMARY_KEY_OWNER,
       PKCON.TABLE_NAME AS PRIMARY_KEY_TABLE_NAME,
       FKCON.OWNER AS FOREIGN_KEY_OWNER,
       FKCON.CONSTRAINT_NAME AS FOREIGN_KEY_CONSTRAINT_NAME,
       FKCON.TABLE_NAME AS FOREIGN_KEY_TABLE_NAME,
       FKCON.SEARCH_CONDITION,
       FKCON.R_OWNER,
       FKCON.R_CONSTRAINT_NAME,
       FKCON.DELETE_RULE,
       FKCON.STATUS,
       (SELECT cu.COLUMN_NAME
          FROM ALL_CONS_COLUMNS CU, ALL_CONSTRAINTS AU
         WHERE CU.OWNER = AU.OWNER
           AND CU.CONSTRAINT_NAME = AU.CONSTRAINT_NAME
           AND AU.CONSTRAINT_TYPE = 'P'
           and au.constraint_name = FKCON.r_constraint_name
           and FKCON.owner = au.OWNER
           and rownum = 1) PRIMARY_KEY_COLUMN_NAME,
       (SELECT cu.COLUMN_NAME
          FROM ALL_CONS_COLUMNS CU, ALL_CONSTRAINTS AU
         WHERE CU.OWNER = AU.OWNER
           AND CU.CONSTRAINT_NAME = AU.CONSTRAINT_NAME
           AND AU.CONSTRAINT_TYPE = 'R'
           and au.constraint_name = FKCON.CONSTRAINT_NAME
           and PKCON.owner = au.OWNER
           and rownum = 1) FOREIGN_KEY_COLUMN_NAME
  FROM ALL_CONSTRAINTS FKCON, ALL_CONSTRAINTS PKCON
 WHERE PKCON.OWNER = FKCON.R_OWNER
   AND PKCON.CONSTRAINT_NAME = FKCON.R_CONSTRAINT_NAME
   AND FKCON.CONSTRAINT_TYPE = 'R'
   and (FKCON.OWNER = '{connpar.UserId!.ToUpper()}'){(parameters.HasValue("TABLENAME") ? @"
  AND FKCON.TABLE_NAME IN (:TABLENAME)" : string.Empty)}{(parameters.HasValue("CONSTRAINTNAME") ? @"
  AND FKCON.CONSTRAINT_NAME IN (:CONSTRAINTNAME)" : string.Empty)}
";

            return ExecuteAndParseMetadataAsync(database, sql, parameters, (wrapper, reader) => new ForeignKey
            {
                Schema = reader["FOREIGN_KEY_OWNER"].ToString(),
                Name = reader["FOREIGN_KEY_CONSTRAINT_NAME"].ToString(),
                TableName = reader["FOREIGN_KEY_TABLE_NAME"].ToString().Replace("\"", ""),
                ColumnName = reader["FOREIGN_KEY_COLUMN_NAME"].ToString(),
                PKTable = reader["PRIMARY_KEY_TABLE_NAME"].ToString(),
                PKColumn = reader["PRIMARY_KEY_COLUMN_NAME"].ToString()
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

            restrictionValues
                .Parameterize(parameters, "VIEWNAME", nameof(Table.Name));

            SpecialCommand sql = $@"
SELECT * FROM (
    SELECT T.OWNER,
       T.VIEW_NAME,
       C.COMMENTS
  FROM ALL_VIEWS T
  JOIN ALL_TAB_COMMENTS C
    ON T.OWNER = C.OWNER
   AND T.VIEW_NAME = C.TABLE_NAME
) T
 WHERE (T.OWNER = '{connpar.UserId!.ToUpper()}'){(parameters.HasValue("TABLENAME") ? @"
  AND T.VIEW_NAME IN (:TABLENAME)" : string.Empty)}
ORDER BY OWNER, VIEW_NAME";

            return ExecuteAndParseMetadataAsync(database, sql, parameters, (wrapper, reader) => new View
            {
                Schema = wrapper!.GetString(reader, 0),
                Name = wrapper.GetString(reader, 1),
                Description = wrapper.GetString(reader, 2)
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
SELECT T.OWNER,
       T.TABLE_NAME,
       T.COLUMN_NAME,
       T.DATA_TYPE AS DATATYPE,
       T.DATA_LENGTH AS LENGTH,
       T.DATA_PRECISION AS PRECISION,
       T.DATA_SCALE AS SCALE,
       T.NULLABLE AS NULLABLE,
       C.COMMENTS
  FROM ALL_TAB_COLUMNS T
  JOIN ALL_VIEWS V
    ON T.OWNER = V.OWNER
   AND T.TABLE_NAME = V.VIEW_NAME
  LEFT JOIN ALL_COL_COMMENTS C
    ON T.OWNER = C.OWNER
   AND T.TABLE_NAME = C.TABLE_NAME
   AND T.COLUMN_NAME = C.COLUMN_NAME
  LEFT JOIN ALL_TAB_COLUMNS D
    ON T.OWNER = D.OWNER
   AND T.TABLE_NAME = D.TABLE_NAME
   AND T.COLUMN_NAME = D.COLUMN_NAME
 WHERE (T.OWNER = '{connpar.UserId!.ToUpper()}'){(parameters.HasValue("VIEWNAME") ? @"
  AND T.TABLE_NAME IN (:VIEWNAME)" : string.Empty)}{(parameters.HasValue("COLUMNNAME") ? @"
  AND T.COLUMN_NAME IN (:COLUMNNAME)" : string.Empty)}
 ORDER BY T.OWNER, T.TABLE_NAME, T.COLUMN_ID";

            return ExecuteAndParseMetadataAsync(database, sql, parameters, (wrapper, reader) => SetDataType(new ViewColumn
            {
                Schema = wrapper!.GetString(reader, 0),
                ViewName = wrapper.GetString(reader, 1),
                Name = wrapper.GetString(reader, 2),
                DataType = wrapper.GetString(reader, 3),
                Length = reader.IsDBNull(4) ? (long?)null : wrapper.GetInt32(reader, 4),
                NumericPrecision = reader.IsDBNull(5) ? (int?)null : wrapper.GetInt32(reader, 5),
                NumericScale = reader.IsDBNull(6) ? (int?)null : wrapper.GetInt32(reader, 6),
                IsNullable = wrapper.GetString(reader, 7) == "Y",
                Description = wrapper.GetString(reader, 8),
            }));
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
SELECT T.OWNER,
       T.INDEX_NAME, 
       T.TABLE_NAME,
       T.UNIQUENESS
 FROM ALL_INDEXES T
 WHERE (T.OWNER = '{connpar.UserId!.ToUpper()}'){(parameters.HasValue("TABLENAME") ? @"
  AND T.TABLE_NAME IN (:TABLENAME)" : string.Empty)}{(parameters.HasValue("INDEXNAME") ? @"
  AND T.INDEX_NAME IN (:INDEXNAME)" : string.Empty)}
 ORDER BY T.OWNER, T.TABLE_NAME";

            return ExecuteAndParseMetadataAsync(database, sql, parameters, (wrapper, reader) => new Index
            {
                Schema = wrapper!.GetString(reader, 0),
                Name = wrapper.GetString(reader, 1),
                TableName = wrapper.GetString(reader, 2),
                IsUnique = wrapper.GetString(reader, 3) == "UNIQUE"
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
SELECT T.INDEX_OWNER,
       T.INDEX_NAME, 
       T.TABLE_NAME,
       T.COLUMN_NAME
 FROM ALL_IND_COLUMNS T
 WHERE (T.INDEX_OWNER = '{connpar.UserId!.ToUpper()}'){(parameters.HasValue("TABLENAME") ? @"
  AND T.TABLE_NAME IN (:TABLENAME)" : string.Empty)}{(parameters.HasValue("INDEXNAME") ? @"
  AND T.INDEX_NAME IN (:INDEXNAME)" : string.Empty)}{(parameters.HasValue("COLUMNNAME") ? @"
  AND T.COLUMN_NAME IN (:COLUMNNAME)" : string.Empty)}
 ORDER BY T.INDEX_OWNER, T.TABLE_NAME";

            return ExecuteAndParseMetadataAsync(database, sql, parameters, (wrapper, reader) => new IndexColumn
            {
                Schema = wrapper!.GetString(reader, 0),
                TableName = wrapper.GetString(reader, 1),
                IndexName = wrapper.GetString(reader, 2),
                ColumnName = wrapper.GetString(reader, 3)
            });
        }
    }
}
