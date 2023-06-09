﻿// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Fireasy.Data.Syntax
{
    /// <summary>
    /// Oracle字符串函数语法解析。
    /// </summary>
    public class OracleStringSyntax : IStringSyntax
    {
        /// <summary>
        /// 取源表达式的子串。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <param name="startExp">子串的起始字符位置。</param>
        /// <param name="lenExp">子串中的字符数。</param>
        /// <returns></returns>
        public virtual string Substring(object sourceExp, object startExp, object? lenExp = null)
        {
            return lenExp == null ?
                $"SUBSTR({sourceExp}, {startExp})" :
                $"SUBSTR({sourceExp}, {startExp}, {lenExp})";
        }

        /// <summary>
        /// 计算源表达式的长度。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <returns></returns>
        public virtual string Length(object sourceExp)
        {
            return $"LENGTH({sourceExp})";
        }

        /// <summary>
        /// 判断子串在源表达式中的位置。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <param name="searchExp">要搜寻的字符串。</param>
        /// <param name="startExp">搜索起始位置。</param>
        /// <param name="countExp">要检查的字符位置数</param>
        /// <returns></returns>
        public virtual string IndexOf(object sourceExp, object searchExp, object? startExp = null, object? countExp = null)
        {
            if (startExp != null)
            {
                return countExp == null ?
                    $"INSTR({sourceExp}, {searchExp}, {startExp})" :
                    $"INSTR({sourceExp}, {searchExp}, {startExp}, {countExp})";
            }

            return $"INSTR({sourceExp}, {searchExp})";
        }

        /// <summary>
        /// 将源表达式转换为小写格式。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <returns></returns>
        public virtual string ToLower(object sourceExp)
        {
            return $"LOWER({sourceExp})";
        }

        /// <summary>
        /// 将源表达式转换为大写格式。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <returns></returns>
        public virtual string ToUpper(object sourceExp)
        {
            return $"UPPER({sourceExp})";
        }

        /// <summary>
        /// 截掉源表达式的左边所有空格。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <returns></returns>
        public virtual string TrimStart(object sourceExp)
        {
            return $"LTRIM({sourceExp})";
        }

        /// <summary>
        /// 截掉源表达式的右边所有空格。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <returns></returns>
        public virtual string TrimEnd(object sourceExp)
        {
            return $"RTRIM({sourceExp})";
        }

        /// <summary>
        /// 截掉源表达式的两边所有空格。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <returns></returns>
        public virtual string Trim(object sourceExp)
        {
            return $"TRIM({sourceExp})";
        }

        /// <summary>
        /// 在源的左边加入字符。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <param name="count">空格的个数。</param>
        /// <param name="padding"></param>
        /// <returns></returns>
        public virtual string PadLeft(object sourceExp, object count, object? padding = null)
        {
            return $"LPAD({sourceExp}, {count}, {padding ?? "' '"})";
        }

        /// <summary>
        /// 在源的右边加入字符。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <param name="count">空格的个数。</param>
        /// <param name="padding"></param>
        /// <returns></returns>
        public virtual string PadRight(object sourceExp, object count, object? padding = null)
        {
            return $"RPAD({sourceExp}, {count}, {padding ?? "' '"})";
        }

        /// <summary>
        /// 将源表达式中的部份字符替换为新的字符。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <param name="searchExp">要搜寻的字符串。</param>
        /// <param name="replaceExp">要替换的字符串。</param>
        /// <returns></returns>
        public virtual string Replace(object sourceExp, object searchExp, object replaceExp)
        {
            return $"REPLACE({sourceExp}, {searchExp}, {replaceExp})";
        }

        /// <summary>
        /// 将一组字符串连接为新的字符串。
        /// </summary>
        /// <param name="strExps">要连接的字符串数组。</param>
        /// <returns></returns>
        public virtual string Concat(params object[] strExps)
        {
            return $"({string.Join(" || ", strExps)})";
        }

        /// <summary>
        /// 反转源表达式。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <returns></returns>
        public virtual string Reverse(object sourceExp)
        {
            return $"REVERSE({sourceExp})";
        }

        /// <summary>
        /// 将分组后的某字段连接为新的字符串。
        /// </summary>
        /// <param name="sourceExp"></param>
        /// <param name="separator">分隔符。</param>
        /// <param name="orderby">排序。</param>
        /// <returns></returns>
        public virtual string GroupConcat(object sourceExp, object separator, object? orderby)
        {
            if (orderby != null)
            {
                throw new SyntaxParseErrorException("不支持指定排序字段。");
            }

            return $"TO_CHAR(DISTINCT LISTAGG({sourceExp}, {separator ?? "','"}))";
        }

        /// <summary>
        /// 正则表达式匹配。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <param name="regexExp">正则表达式。</param>
        /// <returns></returns>
        public virtual string IsMatch(object sourceExp, object regexExp)
        {
            return $"DECODE(REGEXP_SUBSTR({sourceExp}, {regexExp}), NULL, 0, 1)";
        }
    }
}
