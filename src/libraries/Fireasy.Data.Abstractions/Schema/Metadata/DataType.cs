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
    }
}
