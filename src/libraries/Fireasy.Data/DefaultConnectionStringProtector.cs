// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Fireasy.Data
{
    /// <summary>
    /// 缺省的连接字符串保护器。
    /// </summary>
    public class DefaultConnectionStringProtector : IConnectionStringProtector
    {
        private const string START_FLAG = "/*";
        private const string END_FLAG = "*/";
        private readonly byte[] _key = new byte[] { 53, 211, 34, 65, 171, 43, 21, 134 };
        private readonly byte[] _iv = new byte[] { 12, 64, 134, 43, 58, 154, 200, 48 };

        /// <summary>
        /// 加密连接字符串。
        /// </summary>
        /// <param name="connectionString">连接字符串。</param>
        /// <param name="mode">保护的模式。</param>
        /// <returns></returns>
        public virtual ConnectionString Encrypt(ConnectionString connectionString, ConnectionStringProtectMode mode)
        {
            if (mode == ConnectionStringProtectMode.Full)
            {
                return Encrypt((string)connectionString);
            }

            var hasChanged = false;

            if (mode.HasFlag(ConnectionStringProtectMode.Server))
            {
                var server = connectionString.Properties.TryGetValue(ConnectionParameterKeys.Server);
                if (!string.IsNullOrEmpty(server))
                {
                    server = Encrypt(server!);
                    connectionString.Properties.TrySetValue(server, ConnectionParameterKeys.Server);
                    hasChanged = true;
                }
            }
            if (mode.HasFlag(ConnectionStringProtectMode.Password))
            {
                var password = connectionString.Properties.TryGetValue(ConnectionParameterKeys.Password);
                if (!string.IsNullOrEmpty(password))
                {
                    password = Encrypt(password!);
                    connectionString.Properties.TrySetValue(password, ConnectionParameterKeys.Password);
                    hasChanged = true;
                }
            }

            if (hasChanged)
            {
                connectionString.Properties.Update();
            }

            return connectionString;
        }

        /// <summary>
        /// 解密连接字符串。
        /// </summary>
        /// <param name="connectionString">连接字符串。</param>
        /// <returns></returns>
        public virtual ConnectionString Decrypt(ConnectionString connectionString)
        {
            var connStr = (string)connectionString;
            if (connStr!.StartsWith(START_FLAG) == true)
            {
                return Decrypt(connStr);
            }

            var hasChanged = false;
            foreach (var name in connectionString.Properties.Names)
            {
                var value = connectionString.Properties[name];
                if (value?.StartsWith(START_FLAG) == true)
                {
                    value = Decrypt(value);
                    connectionString.Properties.TrySetValue(value, name);
                    hasChanged = true;
                }
            }

            if (hasChanged)
            {
                connectionString.Properties.Update();
            }

            return connectionString;
        }

        /// <summary>
        /// 检查是否被保护。
        /// </summary>
        /// <param name="param">参数或字符串。</param>
        /// <returns></returns>
        public virtual bool IsProtected(string param)
        {
            return param?.StartsWith(START_FLAG) == true && param?.EndsWith(END_FLAG) == true;
        }

        private string Encrypt(string param)
        {
            if (IsProtected(param))
            {
                return param;
            }

            byte[] curIv = _iv;
            byte[] temp = new byte[4];
            new Random().NextBytes(temp);
            curIv[1] = temp[0];
            curIv[3] = temp[1];
            curIv[5] = temp[2];
            curIv[7] = temp[3];

            var des = new DESCryptoServiceProvider { Key = _key, IV = curIv };

            using var ct = des.CreateEncryptor();
            var buffer = Encoding.UTF8.GetBytes(param);

            using var ms = new MemoryStream();
            using var cs = new CryptoStream(ms, ct, CryptoStreamMode.Write);
            cs.Write(buffer, 0, buffer.Length);
            cs.FlushFinalBlock();

            buffer = new byte[ms.Length + 4];
            Array.Copy(ms.ToArray(), buffer, ms.Length);
            buffer[buffer.Length - 4] = curIv[1];
            buffer[buffer.Length - 3] = curIv[3];
            buffer[buffer.Length - 2] = curIv[5];
            buffer[buffer.Length - 1] = curIv[7];

            return $"{START_FLAG}{Convert.ToBase64String(buffer).Replace('=', '*')}{END_FLAG}";
        }

        private string Decrypt(string param)
        {
            if (!IsProtected(param))
            {
                return param;
            }

            var buffer = Convert.FromBase64String(param.Substring(START_FLAG.Length, param.Length - START_FLAG.Length - END_FLAG.Length).Replace('*', '='));

            var curIv = _iv;
            curIv[1] = buffer[buffer.Length - 4];
            curIv[3] = buffer[buffer.Length - 3];
            curIv[5] = buffer[buffer.Length - 2];
            curIv[7] = buffer[buffer.Length - 1];

            var des = new DESCryptoServiceProvider { Key = _key, IV = curIv };

            using var ct = des.CreateDecryptor();

            using var ms = new MemoryStream();
            using var cs = new CryptoStream(ms, ct, CryptoStreamMode.Write);
            cs.Write(buffer, 0, buffer.Length - 4);
            cs.FlushFinalBlock();
            return Encoding.UTF8.GetString(ms.ToArray());
        }
    }
}