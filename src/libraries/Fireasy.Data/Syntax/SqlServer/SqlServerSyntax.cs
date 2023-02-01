// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using System;
using System.Data;
using System.Text;

namespace Fireasy.Data.Syntax
{
    /// <summary>
    /// SqlServer 函数语法解析。
    /// </summary>
    public class SqlServerSyntax : ISyntaxProvider
    {
        /// <summary>
        /// 获取字符串函数相关的语法。
        /// </summary>
        public virtual IStringSyntax String => new SqlServerStringSyntax();

        /// <summary>
        /// 获取日期函数相关的语法。
        /// </summary>
        public virtual IDateTimeSyntax DateTime => new SqlServerDateTimeSyntax();

        /// <summary>
        /// 获取数学函数相关的语法。
        /// </summary>
        public virtual IMathSyntax Math => new SqlServerMathSyntax();

        /// <summary>
        /// 获取最近创建的自动编号的查询文本。
        /// </summary>
        public virtual string IdentityValue => "SELECT @@IDENTITY";

        /// <summary>
        /// 获取自增长列的关键词。
        /// </summary>
        public string IdentityColumn => "IDENTITY(1, 1)";

        /// <summary>
        /// 获取受影响的行数的查询文本。
        /// </summary>
        public string RowsAffected => "SELECT @@ROWCOUNT";

        /// <summary>
        /// 获取新的 Guid。
        /// </summary>
        /// <returns></returns>
        public string NewGuid => "SELECT NEWID()";

        /// <summary>
        /// 获取伪查询的表名称。
        /// </summary>
        public string FakeSelect => string.Empty;

        /// <summary>
        /// 获取存储参数的前缀。
        /// </summary>
        public virtual string ParameterPrefix => "@";

        /// <summary>
        /// 获取定界符。
        /// </summary>
        public string[] Delimiter => new[] { "[", "]" };

        /// <summary>
        /// 获取语句结束符。
        /// </summary>
        public string StatementTerminator => ";" + Environment.NewLine;

        /// <summary>
        /// 获取是否允许在聚合中使用 DISTINCT 关键字。
        /// </summary>
        public bool SupportDistinctInAggregates => true;

        /// <summary>
        /// 获取是否允许在没有 FORM 的语句中使用子查询。
        /// </summary>
        public bool SupportSubqueryInSelectWithoutFrom => true;

        /// <summary>
        /// 获取是否支持同时返回自增值。
        /// </summary>
        public bool SupportReturnIdentityValue => true;

        /// <summary>
        /// 对命令文本进行分段处理，使之能够返回小范围内的数据。
        /// </summary>
        /// <param name="context">命令上下文对象。</param>
        /// <exception cref="SegmentNotSupportedException">当前数据库或版本不支持分段时，引发该异常。</exception>
        public virtual bool Segment(CommandContext context)
        {
            context.SetCommand(Segment(context.Command.CommandText, context.Segment));

            return true;
        }

        /// <summary>
        /// 对命令文本进行分段处理，使之能够返回小范围内的数据。
        /// </summary>
        /// <param name="commandText">命令文本。</param>
        /// <param name="segment">数据分段对象。</param>
        /// <returns>处理后的分段命令文本。</returns>
        /// <exception cref="SegmentNotSupportedException">当前数据库或版本不支持分段时，引发该异常。</exception>
        public virtual string Segment(string commandText, IDataSegment segment)
        {
            var orderBy = DbUtility.FindOrderBy(commandText);

            return @$"{commandText}
{(string.IsNullOrEmpty(orderBy) ? "ORDER BY 1" : string.Empty)}{$" OFFSET {(segment.Start ?? 1) - 1} ROW"} FETCH NEXT {(segment.Length != 0 ? segment.Length : 1000)} ROWS ONLY";
        }

        /// <summary>
        /// 转换源表达式的数据类型。
        /// </summary>
        /// <param name="sourceExp">要转换的源表达式。</param>
        /// <param name="dbType">要转换的类型。</param>
        /// <returns></returns>
        public virtual string Convert(object sourceExp, DbType dbType)
        {
            switch (dbType)
            {
                case DbType.AnsiString:
                    return $"CAST({sourceExp} AS VARCHAR)";
                case DbType.AnsiStringFixedLength:
                    return $"CAST({sourceExp} AS CHAR)";
                case DbType.Binary:
                    return $"CAST({sourceExp} AS VARBINARY)";
                case DbType.Boolean:
                    return $"CAST({sourceExp} AS BIT)";
                case DbType.Byte:
                    return $"CAST({sourceExp} AS TINYINT)";
                case DbType.Currency:
                    return $"CAST({sourceExp} AS MONEY)";
                case DbType.Date:
                    return $"CAST({sourceExp} AS DATETIME)";
                case DbType.DateTime:
                    return $"CAST({sourceExp} AS DATETIME)";
                case DbType.DateTime2:
                    return $"CAST({sourceExp} AS DATETIME2)";
                case DbType.DateTimeOffset:
                    return $"CAST({sourceExp} AS DATETIMEOFFSET)";
                case DbType.Decimal:
                    return $"CAST({sourceExp} AS DECIMAL(12,6))";
                case DbType.Double:
                    return $"CAST({sourceExp} AS DOUBLE PRECISION)";
                case DbType.Guid:
                    return $"CAST({sourceExp} AS UNIQUEIDENTIFIER)";
                case DbType.Int16:
                    return $"CAST({sourceExp} AS SMALLINT)";
                case DbType.Int32:
                    return $"CAST({sourceExp} AS INT)";
                case DbType.Int64:
                    return $"CAST({sourceExp} AS BIGINT)";
                case DbType.Single:
                    return $"CAST({sourceExp} AS REAL)";
                case DbType.String:
                    return $"CAST({sourceExp} AS NVARCHAR)";
                case DbType.StringFixedLength:
                    return $"CAST({sourceExp} AS NCHAR)";
                case DbType.Time:
                    return $"CAST({sourceExp} AS DATETIME)";
                case DbType.Xml:
                    return $"CAST({sourceExp} AS XML)";
            }

            throw new SyntaxParseErrorException($"{nameof(Convert)} 无法转换 {dbType} 类型");
        }

        /// <summary>
        /// 根据数据类型生成相应的列。
        /// </summary>
        /// <param name="dbType">数据类型。</param>
        /// <param name="length">数据长度。</param>
        /// <param name="precision">数值的精度。</param>
        /// <param name="scale">数值的小数位。</param>
        /// <returns></returns>
        public string Column(DbType dbType, int? length, int? precision, int? scale = new int?())
        {
            switch (dbType)
            {
                case DbType.AnsiString:
                    if (length == null)
                    {
                        return "VARCHAR(255)";
                    }
                    if (length > 8000)
                    {
                        return "NTEXT";
                    }
                    return $"VARCHAR({length})";
                case DbType.AnsiStringFixedLength:
                    return length == null ? "CHAR(255)" : $"CHAR({length})";
                case DbType.Binary:
                    if (length == null)
                    {
                        return "VARBINARY(8000)";
                    }
                    if (length > 8000)
                    {
                        return "IMAGE";
                    }
                    return $"VARBINARY({length})";
                case DbType.Boolean:
                    return "BIT";
                case DbType.Byte:
                    return "TINYINT";
                case DbType.Currency:
                    return "MONEY";
                case DbType.Date:
                    return "DATETIME";
                case DbType.DateTime:
                    return "DATETIME";
                case DbType.DateTime2:
                    return "DATETIME2";
                case DbType.DateTimeOffset:
                    return "DATETIMEOFFSET";
                case DbType.Decimal:
                    if (precision == null && scale == null)
                    {
                        return "DECIMAL(19, 5)";
                    }
                    if (precision == null)
                    {
                        return $"DECIMAL(19, {scale})";
                    }
                    if (scale == null)
                    {
                        return $"DECIMAL({precision}, 5)";
                    }
                    return $"DECIMAL({precision}, {scale})";
                case DbType.Double:
                    return "DOUBLE PRECISION";
                case DbType.Guid:
                    return "UNIQUEIDENTIFIER";
                case DbType.Int16:
                    return "SMALLINT";
                case DbType.Int32:
                    return "INT";
                case DbType.Int64:
                    return "BIGINT";
                case DbType.SByte:
                    return "TINYINT";
                case DbType.Single:
                    return "REAL";
                case DbType.String:
                    if (length == null)
                    {
                        return "VARCHAR(255)";
                    }
                    if (length > 8000)
                    {
                        return "NTEXT";
                    }
                    return $"VARCHAR({length})";
                case DbType.StringFixedLength:
                    if (length == null)
                    {
                        return "NCHAR(255)";
                    }
                    return $"NCHAR({length})";
                case DbType.Time:
                    return "DATETIME";
                case DbType.Xml:
                    return "XML";
            }

            throw new SyntaxParseErrorException($"{nameof(Column)} 无法构造 {dbType} 类型");
        }

        /// <summary>
        /// 如果源表达式为 null，则依次判断给定的一组参数，直至某参数非 null 时返回。
        /// </summary>
        /// <param name="sourceExp">要转换的源表达式。</param>
        /// <param name="argExps">参与判断的一组参数。</param>
        /// <returns></returns>
        public virtual string Coalesce(object sourceExp, params object[] argExps)
        {
            if (argExps == null || argExps.Length == 0)
            {
                return sourceExp.ToString();
            }

            var sb = new StringBuilder();
            sb.AppendFormat("COALESCE({0}", sourceExp);
            foreach (var par in argExps)
            {
                sb.AppendFormat(", {0}", par);
            }

            sb.Append(")");

            return sb.ToString();
        }

        /// <summary>
        /// 格式化参数名称。
        /// </summary>
        /// <param name="parameterName"></param>
        /// <returns></returns>
        public virtual string FormatParameter(string parameterName)
        {
            return string.Concat(ParameterPrefix, parameterName);
        }

        /// <summary>
        /// 获取判断表是否存在的语句。
        /// </summary>
        /// <param name="tableName">要判断的表的名称。</param>
        /// <returns></returns>
        public string ExistsTable(string tableName)
        {
            return $"SELECT COUNT(1) FROM SYS.OBJECTS WHERE NAME = '{tableName}' AND TYPE = 'U'";
        }

        /// <summary>
        /// 给表名或字段名添加界定符。
        /// </summary>
        /// <param name="name">表名或字段名。</param>
        public virtual string Delimit(string name)
        {
            return DbUtility.FormatByDelimiter(this, name);
        }

        /// <summary>
        /// 将表名或字段名转换特定的大小写。
        /// </summary>
        /// <param name="name">表名或字段名。</param>
        /// <returns></returns>
        public virtual string ToggleCase(string name)
        {
            return name;
        }

        /// <summary>
        /// 修正 <see cref="DbType"/> 值。
        /// </summary>
        /// <param name="dbType"></param>
        /// <returns></returns>
        public DbType CorrectDbType(DbType dbType)
        {
            return dbType;
        }
    }
}
