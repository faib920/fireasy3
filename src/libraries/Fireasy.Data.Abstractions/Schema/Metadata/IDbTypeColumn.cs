// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Fireasy.Data.Schema
{
    public interface IDbTypeColumn
    {
        /// <summary>
        /// 获取数据类型。
        /// </summary>
        string DataType { get; set; }

        /// <summary>
        /// 获取或设置对应的 <see cref="DbType"/> 类型。
        /// </summary>
        DbType? DbType { get; set; }

        /// <summary>
        /// 获取或设置运行库类型。
        /// </summary>
        Type? ClrType { get; set; }
    }
}
