﻿// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using System.Text;

namespace Fireasy.Data.Syntax
{
    /// <summary>
    /// MySql函数语法解析。
    /// </summary>
    public class MySqlSyntax : ISyntaxProvider
    {
        /// <summary>
        /// 获取字符串函数相关的语法。
        /// </summary>
        public virtual IStringSyntax String => new MySqlStringSyntax();

        /// <summary>
        /// 获取日期函数相关的语法。
        /// </summary>
        public virtual IDateTimeSyntax DateTime => new MySqlDateTimeSyntax();

        /// <summary>
        /// 获取数学函数相关的语法。
        /// </summary>
        public virtual IMathSyntax Math => new MySqlMathSyntax();

        /// <summary>
        /// 获取最近创建的自动编号的查询文本。
        /// </summary>
        public virtual string IdentityValue => "SELECT LAST_INSERT_ID()";

        /// <summary>
        /// 获取自增长列的关键词。
        /// </summary>
        public virtual string IdentityColumn => "AUTO_INCREMENT";

        /// <summary>
        /// 获取受影响的行数的查询文本。
        /// </summary>
        public string RowsAffected => "SELECT ROW_COUNT()";

        /// <summary>
        /// 获取新的 Guid。
        /// </summary>
        /// <returns></returns>
        public string NewGuid => "SELECT UUID()";

        /// <summary>
        /// 获取伪查询的表名称。
        /// </summary>
        public string FakeTable => " FROM DUAL";

        /// <summary>
        /// 获取存储参数的前缀。
        /// </summary>
        public virtual string ParameterPrefix => "?";

        /// <summary>
        /// 获取定界符。
        /// </summary>
        public virtual string[] Delimiter => new[] { "`", "`" };

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
            context.Command.CommandText = Segment(context.Command.CommandText, context.Segment);
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
            return @$"{commandText}
LIMIT {(segment.Length != 0 ? segment.Length : 1000)}{(segment.Start != null ? $" OFFSET {segment.Start - 1}" : string.Empty)}";
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
                case DbType.String:
                case DbType.AnsiString:
                case DbType.Guid:
                case DbType.StringFixedLength:
                case DbType.AnsiStringFixedLength:
                    return $"CAST({sourceExp} AS CHAR)";
                case DbType.Binary:
                    return $"CAST({sourceExp} AS BINARY)";
                case DbType.Currency:
                case DbType.Decimal:
                case DbType.Double:
                case DbType.Single:
                    return $"CAST({sourceExp} AS DECIMAL(12,6))";
                case DbType.Boolean:
                case DbType.SByte:
                case DbType.Int16:
                case DbType.Int32:
                case DbType.Int64:
                    return $"CAST({sourceExp} AS SIGNED)";
                case DbType.Byte:
                case DbType.UInt16:
                case DbType.UInt32:
                case DbType.UInt64:
                    return $"CAST({sourceExp} AS UNSIGNED)";
                case DbType.Date:
                    return $"CAST({sourceExp} AS DATE)";
                case DbType.DateTime:
                    return $"CAST({sourceExp} AS DATETIME)";
                case DbType.Time:
                    return $"CAST({sourceExp} AS TIME)";
                case DbType.DateTimeOffset:
                    return $"CAST({sourceExp} AS TIMESTAMP)";
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
        public virtual string Column(DbType dbType, int? length = null, int? precision = null, int? scale = null)
        {
            switch (dbType)
            {
                case DbType.String:
                case DbType.AnsiString:
                    if (length == null || length <= 255)
                    {
                        return $"VARCHAR({length ?? 255})";
                    }
                    if (length > 255 && length <= 65535)
                    {
                        return "TEXT";
                    }
                    if (length > 65535 && length <= 16777215)
                    {
                        return "MEDIUMTEXT";
                    }
                    return "LONGTEXT";
                case DbType.StringFixedLength:
                case DbType.AnsiStringFixedLength:
                    if (length == null || length <= 255)
                    {
                        return $"CHAR({length ?? 255})";
                    }
                    if (length > 255 && length <= 65535)
                    {
                        return "TEXT";
                    }
                    if (length > 65535 && length <= 16777215)
                    {
                        return "MEDIUMTEXT";
                    }
                    return "LONGTEXT";
                case DbType.Guid:
                    return "VARCHAR(40)";
                case DbType.Binary:
                    if (length == null || length <= 255)
                    {
                        return "TINYBLOB";
                    }
                    if (length > 255 && length <= 65535)
                    {
                        return "BLOB";
                    }
                    if (length > 65535 && length <= 16777215)
                    {
                        return "MEDIUMBLOB";
                    }
                    return "LONGBLOB";
                case DbType.Currency:
                case DbType.Decimal:
                    return $"DECIMAL({precision ?? 19}, {scale ?? 6})";
                case DbType.Double:
                    return "DOUBLE";
                case DbType.Single:
                    return "FLOAT";
                case DbType.Boolean:
                    return "TINYINT(1)";
                case DbType.Byte:
                case DbType.SByte:
                    return "TINYINT";
                case DbType.Int16:
                    return "SMALLINT";
                case DbType.Int32:
                    return "INT";
                case DbType.Int64:
                    return "BIGINT";
                case DbType.UInt16:
                    return "SMALLINT";
                case DbType.UInt32:
                    return "MEDIUMINT";
                case DbType.UInt64:
                    return "BIT";
                case DbType.Date:
                    return "DATE";
                case DbType.DateTime:
                    return "DATETIME";
                case DbType.Time:
                    return "TIME";
                case DbType.DateTimeOffset:
                    return "TIMESTAMP";
                case DbType.Xml:
                    return "TEXT";
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
            return $"select count(1) from information_schema.tables where Table_Name='{tableName}'";
        }

        /// <summary>
        /// 给表名或字段名添加界定符。
        /// </summary>
        /// <param name="name"></param>
        public virtual string Delimit(string name)
        {
            return DbUtility.FormatByDelimiter(this, name);
        }

        /// <summary>
        /// 将表名或字段名转换特定的大小写。
        /// </summary>
        /// <param name="name"></param>
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
