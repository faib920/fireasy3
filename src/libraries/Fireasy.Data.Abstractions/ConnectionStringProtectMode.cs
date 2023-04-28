// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Fireasy.Data
{
    /// <summary>
    /// 连接字符串瓮中保护模式。
    /// </summary>
    [Flags]
    public enum ConnectionStringProtectMode
    {
        /// <summary>
        /// 保护服务器参数。
        /// </summary>
        Server = 1,
        /// <summary>
        /// 保护用户名参数。
        /// </summary>
        UserId = 3,
        /// <summary>
        /// 保护密码参数。
        /// </summary>
        Password = 4,
        /// <summary>
        /// 完整保护，保护整个连接串。
        /// </summary>
        Full = 32
    }
}
