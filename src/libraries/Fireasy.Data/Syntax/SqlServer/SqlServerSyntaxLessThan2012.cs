// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Fireasy.Data.Extensions;
using System.Text.RegularExpressions;

namespace Fireasy.Data.Syntax.SqlServer
{
    /// <summary>
    /// SqlServer 2012 以下版本函数语法解析。
    /// </summary>
    public class SqlServerSyntaxLessThan2012 : SqlServerSyntax
    {
        /// <summary>
        /// 对命令文本进行分段处理，使之能够返回小范围内的数据。
        /// </summary>
        /// <param name="commandText">命令文本。</param>
        /// <param name="segment">数据分段对象。</param>
        /// <returns>处理后的分段命令文本。</returns>
        /// <exception cref="SegmentNotSupportedException">当前数据库或版本不支持分段时，引发该异常。</exception>
        public override string Segment(string commandText, IDataSegment segment)
        {
            var orderBy = DbUtility.FindOrderBy(commandText);
            var regAlias = new Regex(@"(\w+)?\.");

            //如果有排序
            if (!string.IsNullOrEmpty(orderBy))
            {
                //去除子句中的Order并移到OVER后
                return @$"
                    SELECT TValue.* FROM 
                    (
                        SELECT TValue.*, ROW_NUMBER() OVER ({regAlias.Replace(orderBy, string.Empty)}) AS ROW_NUM 
                        FROM ({commandText.Replace(orderBy, string.Empty).Trim()}) TValue
                    ) TValue 
                    WHERE {segment.Condition("ROW_NUM")}";
            }
            else
            {
                return @$"
                    SELECT TValue.* FROM 
                    (
                        SELECT TValue.*, ROW_NUMBER() OVER (ORDER BY (SELECT 1)) AS ROW_NUM 
                        FROM ({commandText}) TValue
                    ) TValue 
                    WHERE {segment.Condition("ROW_NUM")}";
            }
        }
    }
}
