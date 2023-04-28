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
    /// 连接字符串保护器。
    /// </summary>
    public interface IConnectionStringProtector
    {
        /// <summary>
        /// 加密连接字符串。
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="part">加密部位。</param>
        /// <returns></returns>
        ConnectionString Encrypt(ConnectionString connectionString, CSEncryptPart part = CSEncryptPart.Password);

        /// <summary>
        /// 解密连接字符串。
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        ConnectionString Decrypt(ConnectionString connectionString);
    }
}
