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
    /// 连接字符串加密部位。
    /// </summary>
    [Flags]
    public enum CSEncryptPart
    {
        /// <summary>
        /// 服务器参数。
        /// </summary>
        Server = 1,
        /// <summary>
        /// 用户名参数。
        /// </summary>
        UserId = 3,
        /// <summary>
        /// 密码参数。
        /// </summary>
        Password = 4,
        /// <summary>
        /// 完整加密。
        /// </summary>
        Full = 32
    }
}
