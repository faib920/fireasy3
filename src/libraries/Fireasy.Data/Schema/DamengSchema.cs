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
    /// Dameng 数据库架构信息的获取方法。
    /// </summary>
    public sealed class DamengSchema : SchemaBase
    {
        public DamengSchema()
        {
            AddRestriction<Database>(s => s.Name);
            AddRestriction<Table>(s => s.Name);
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
        /// 添加数据类型映射。
        /// </summary>
        protected override void AddDataTypeMappers()
        {
            AddDataType("bit", DbType.Boolean, typeof(bool));
            AddDataType("int", DbType.Int32, typeof(int));
            AddDataType("integer", DbType.Int32, typeof(int));
            AddDataType("smallint", DbType.Int16, typeof(short));
            AddDataType("bigint", DbType.Int64, typeof(long));
            AddDataType("byte", DbType.SByte, typeof(sbyte));
            AddDataType("tinyint", DbType.SByte, typeof(sbyte));
            AddDataType("float", DbType.Double, typeof(double));
            AddDataType("double", DbType.Double, typeof(double));
            AddDataType("double precision", DbType.Double, typeof(double));
            AddDataType("number", DbType.Decimal, typeof(decimal));
            AddDataType("numeric", DbType.Decimal, typeof(decimal));
            AddDataType("decimal", DbType.Decimal, typeof(decimal));
            AddDataType("dec", DbType.Decimal, typeof(decimal));
            AddDataType("real", DbType.Single, typeof(float));
            AddDataType("bfile", DbType.String, typeof(string));
            AddDataType("blob", DbType.Binary, typeof(byte[]));
            AddDataType("binary", DbType.Binary, typeof(byte[]));
            AddDataType("varbinary", DbType.Binary, typeof(byte[]));
            AddDataType("longvarbinary", DbType.Binary, typeof(byte[]));
            AddDataType("image", DbType.Binary, typeof(byte[]));
            AddDataType("char", DbType.String, typeof(string));
            AddDataType("character", DbType.String, typeof(string));
            AddDataType("longvarchar", DbType.String, typeof(string));
            AddDataType("varchar", DbType.String, typeof(string));
            AddDataType("varchar2", DbType.String, typeof(string));
            AddDataType("text", DbType.String, typeof(string));
            AddDataType("clob", DbType.String, typeof(string));
            AddDataType("timestamp with time zone", DbType.Object, typeof(object));
            AddDataType("timestamp with local time zone", DbType.DateTime, typeof(DateTime));
            AddDataType("timestamp", DbType.DateTime, typeof(DateTime));
            AddDataType("date", DbType.DateTime, typeof(DateTime));
            AddDataType("time", DbType.DateTime, typeof(DateTime));
            AddDataType("time with time zone", DbType.Object, typeof(object));
            AddDataType("datetime", DbType.DateTime, typeof(DateTime));
            AddDataType("datetime with time zone", DbType.Object, typeof(object));
            AddDataType("interval year", DbType.Object, typeof(object));
            AddDataType("interval year to month", DbType.Object, typeof(object));
            AddDataType("interval month", DbType.Object, typeof(object));
            AddDataType("interval day", DbType.Object, typeof(object));
            AddDataType("interval day to hour", DbType.Object, typeof(object));
            AddDataType("interval day to minute", DbType.Object, typeof(object));
            AddDataType("interval day to second", DbType.Object, typeof(object));
            AddDataType("interval hour", DbType.Object, typeof(object));
            AddDataType("interval hour to minute", DbType.Object, typeof(object));
            AddDataType("interval hour to second", DbType.Object, typeof(object));
            AddDataType("interval minute", DbType.Object, typeof(object));
            AddDataType("interval minute to second", DbType.Object, typeof(object));
            AddDataType("interval second", DbType.Object, typeof(object));
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

            SpecialCommand sql = $@"
SELECT T.OWNER,
       T.TABLE_NAME,
       C.COMMENTS 
FROM DBA_TABLES T
JOIN USER_TAB_COMMENTS C ON C.TABLE_NAME = T.TABLE_NAME
WHERE (T.OWNER = '{connpar.UserId!.ToUpper()}') AND 
  (T.TABLE_NAME = :TABLENAME OR (:TABLENAME IS NULL))
  ORDER BY T.OWNER, T.TABLE_NAME";

            restrictionValues
                .Parameterize(parameters, "TABLENAME", nameof(Table.Name));

            return ExecuteAndParseMetadataAsync(database, sql, parameters, (wrapper, reader) => new Table
            {
                Schema = wrapper!.GetString(reader, 0),
                Name = wrapper.GetString(reader, 1),
                Description = wrapper.GetString(reader, 2)
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

            SpecialCommand sql = $@"
SELECT 
    T.OWNER,
    T.TABLE_NAME,
    T.COLUMN_NAME,
    T.DATA_TYPE,
    T.DATA_LENGTH,
    T.DATA_PRECISION,
    T.DATA_SCALE,
    T.NULLABLE,
    (CASE WHEN B.COLUMN_NAME IS NULL THEN 'N' ELSE 'Y' END) ISPK,
    (CASE WHEN D.INFO2 = 1 THEN 'Y' ELSE 'N' END) ISINC,
    C.COMMENTS
FROM DBA_TAB_COLUMNS T
JOIN DBA_COL_COMMENTS C ON T.TABLE_NAME = C.TABLE_NAME AND T.COLUMN_NAME = C.COLUMN_NAME
LEFT JOIN USER_IND_COLUMNS B ON T.TABLE_NAME = T.TABLE_NAME AND B.COLUMN_NAME = T.COLUMN_NAME
LEFT JOIN SYSOBJECTS O ON O.NAME = T.TABLE_NAME AND O.SUBTYPE$='UTAB'
LEFT JOIN SYSCOLUMNS D ON D.NAME = T.COLUMN_NAME AND D.ID = O.ID AND D.INFO2 & 1 = 1
 WHERE (T.OWNER = '{connpar.UserId!.ToUpper()}') AND 
   (T.TABLE_NAME = :TABLENAME OR :TABLENAME IS NULL) AND 
   (T.COLUMN_NAME = :COLUMNNAME OR :COLUMNNAME IS NULL)
 ORDER BY T.OWNER, T.TABLE_NAME, T.COLUMN_ID";

            restrictionValues
                .Parameterize(parameters, "TABLENAME", nameof(Column.TableName))
                .Parameterize(parameters, "COLUMNNAME", nameof(Column.Name));

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
                Autoincrement = wrapper.GetString(reader, 9) == "Y",
                //Default = wrapper.GetString(reader, 9),
                Description = wrapper.GetString(reader, 10),
            })));
        }
    }
}
