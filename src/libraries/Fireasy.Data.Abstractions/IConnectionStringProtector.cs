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
        /// <param name="connectionString">连接字符串。</param>
        /// <param name="mode">保护的模式。</param>
        /// <returns></returns>
        ConnectionString Encrypt(ConnectionString connectionString, ConnectionStringProtectMode mode = ConnectionStringProtectMode.Password);

        /// <summary>
        /// 解密连接字符串。
        /// </summary>
        /// <param name="connectionString">连接字符串。</param>
        /// <returns></returns>
        ConnectionString Decrypt(ConnectionString connectionString);

        /// <summary>
        /// 检查是否被保护。
        /// </summary>
        /// <param name="param">参数或字符串。</param>
        /// <returns></returns>
        bool IsProtected(string param);
    }
}
