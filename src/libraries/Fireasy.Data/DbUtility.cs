// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Fireasy.Data.Syntax;
using System;
using System.Text.RegularExpressions;

namespace Fireasy.Data
{
    /// <summary>
    /// 实用类。
    /// </summary>
    public static class DbUtility
    {
        /// <summary>
        /// 使用界定符格式化表名称或列名称。
        /// </summary>
        /// <param name="syntax"></param>
        /// <param name="name"></param>
        /// <param name="forceAttach">是否强制添加。</param>
        /// <returns></returns>
        public static string FormatByDelimiter(ISyntaxProvider syntax, string name, bool forceAttach = false)
        {
            if (syntax == null)
            {
                return name;
            }

            if (name.Length > 1 &&
                (name[0] == syntax.Delimiter[0][0] || name[name.Length - 1] == syntax.Delimiter[1][0]))
            {
                return name;
            }

            return string.Format("{0}{1}{2}", syntax.Delimiter[0], name, syntax.Delimiter[1]);
        }

        /// <summary>
        /// 在命令文本中查找 Order By 子句。
        /// </summary>
        /// <param name="commandText"></param>
        /// <returns></returns>
        internal static string FindOrderBy(string commandText)
        {
            var regx = new Regex(@"\border\s*by", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            var match = regx.Match(commandText);
            if (match.Groups.Count > 0 && match.Groups[0].Success)
            {
                var index = match.Groups[match.Groups.Count - 1].Index;
                var start = index;
                var len = commandText.Length;
                var count = 0;
                var finded = false;
                while (index < len - 1)
                {
                    if (commandText[index] == '(')
                    {
                        count++;
                        finded = true;
                    }
                    else if (commandText[index] == ')')
                    {
                        count--;
                        finded = true;
                    }
                    if (finded && count == -1)
                    {
                        break;
                    }

                    index++;
                }

                return commandText.Substring(start, index - start + 1);
            }

            return string.Empty;
        }

        /// <summary>
        /// 在命令文本中查找 Order By 子句。
        /// </summary>
        /// <param name="commandText"></param>
        /// <returns></returns>
        internal static string CullingOrderBy(string commandText)
        {
            var regx = new Regex(@"order\s*by", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            Match match;
            while ((match = regx.Match(commandText)).Success)
            {
                var index = match.Groups[match.Groups.Count - 1].Index;
                var start = index;
                var len = commandText.Length;
                var count = 0;
                var finded = false;
                while (index < len)
                {
                    if (commandText[index] == '(')
                    {
                        count++;
                        finded = true;
                    }
                    else if (commandText[index] == ')')
                    {
                        count--;
                        finded = true;
                    }
                    if (finded && count == -1)
                    {
                        break;
                    }

                    index++;
                }

                commandText = commandText.Replace(commandText.Substring(start, index - start), "");
            }

            return commandText;
        }
    }
}
