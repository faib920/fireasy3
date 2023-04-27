// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Fireasy.Common.Extensions;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Fireasy.Data
{
    /// <summary>
    /// 缺省的字符串加密器。
    /// </summary>
    public class DefaultConnectionStringEncryptor : IConnectionStringEncryptor
    {
        private const string FLAG = "0x";
        private readonly byte[] _key = new byte[] { 53, 211, 34, 65, 171, 43, 21, 134 };
        private readonly byte[] _iv = new byte[] { 12, 64, 134, 43, 58, 154, 200, 48 };

        ConnectionString IConnectionStringEncryptor.Encrypt(ConnectionString connectionString, CSEncryptPart part = CSEncryptPart.Password)
        {
            if (part == CSEncryptPart.Full)
            {
                return Encrypt((string)connectionString);
            }

            var hasChanged = false;

            if (part.HasFlag(CSEncryptPart.Server))
            {
                var server = connectionString.Properties.TryGetValue("data source", "server", "host");
                if (!string.IsNullOrEmpty(server))
                {
                    server = Encrypt(server!);
                    connectionString.Properties.TrySetValue(server, "data source", "server", "host");
                    hasChanged = true;
                }
            }
            if (part.HasFlag(CSEncryptPart.Password))
            {
                var password = connectionString.Properties.TryGetValue("password", "pwd");
                if (!string.IsNullOrEmpty(password))
                {
                    password = Encrypt(password!);
                    connectionString.Properties.TrySetValue(password, "password", "pwd");
                    hasChanged = true;
                }
            }

            if (hasChanged)
            {
                connectionString.Properties.Update();
            }

            return connectionString;
        }

        ConnectionString IConnectionStringEncryptor.Decrypt(ConnectionString connectionString)
        {
            var connStr = (string)connectionString;
            if (connStr!.StartsWith(FLAG) == true)
            {
                return Decrypt(connStr);
            }

            var hasChanged = false;
            foreach (var name in connectionString.Properties.Names)
            {
                var value = connectionString.Properties[name];
                if (value?.StartsWith(FLAG) == true)
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

        private string Encrypt(string password)
        {
            if (password.StartsWith(FLAG))
            {
                return password;
            }

            var des = new DESCryptoServiceProvider { Key = _key, IV = _iv };

            using var ct = des.CreateEncryptor();
            var byt = Encoding.UTF8.GetBytes(password);

            using var ms = new MemoryStream();
            using var cs = new CryptoStream(ms, ct, CryptoStreamMode.Write);
            cs.Write(byt, 0, byt.Length);
            cs.FlushFinalBlock();
            return $"{FLAG}{ms.ToArray().ToHex(true)}";
        }

        private string Decrypt(string password)
        {
            if (!password.StartsWith(FLAG))
            {
                return password;
            }

            var des = new DESCryptoServiceProvider { Key = _key, IV = _iv };

            using var ct = des.CreateDecryptor();
            var byt = password.Substring(FLAG.Length).FromHex();

            using var ms = new MemoryStream();
            using var cs = new CryptoStream(ms, ct, CryptoStreamMode.Write);
            cs.Write(byt, 0, byt.Length);
            cs.FlushFinalBlock();
            return Encoding.UTF8.GetString(ms.ToArray());
        }
    }
}