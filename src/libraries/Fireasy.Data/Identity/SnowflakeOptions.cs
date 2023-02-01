// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Fireasy.Data.Identity
{
    /// <summary>
    /// 雪花算法相关的参数。
    /// </summary>
    public class SnowflakeOptions
    {
        /// <summary>
        /// 获取或设置机器码。
        /// </summary>
        public virtual long WorkerId { get; set; }
    }
}
