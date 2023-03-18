// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Data.Common;
using System.Text.RegularExpressions;

namespace Fireasy.Data.Schema
{
    /// <summary>
    /// 数据库数据类型信息。
    /// </summary>
    public class DataType : ISchemaMetadata
    {
        /// <summary>
        /// 获取数据类型名称。
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 获取数据类型对应的 <see cref="TValue:System.Data.DbType"/>。
        /// </summary>
        public DbType DbType { get; set; }

        /// <summary>
        /// 获取数据类型对应的 <see cref="TValue:System.ModuleType"/>。
        /// </summary>
        public Type SystemType { get; set; }

        /// <summary>
        /// 获取或设置是否正则表达式匹配。
        /// </summary>
        public bool RegexMatch { get; set; }

        /// <summary>
        /// 判断字段类型是否匹配。
        /// </summary>
        /// <param name="dbType"></param>
        /// <returns></returns>
        public bool IsMatch(string dbType)
        {
            return RegexMatch ? Regex.IsMatch(dbType, Name, RegexOptions.IgnoreCase) :
                string.Equals(dbType, Name, StringComparison.OrdinalIgnoreCase);
        }
    }
}
