﻿// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Fireasy.Data.Provider;
using System.Text;

namespace Fireasy.Data
{
    /// <summary>
    /// 扩展的数据库连接字符串。
    /// </summary>
    /// <remarks>连接字符串中可使用特殊的名称来标记数据库文件的有效地址，这些标识包含|datadirectory|或来自 <see cref="Environment.SpecialFolder"/> 枚举项，如|System|、|ApplicationData|，同时，也可以使用../路径命令符。</remarks>
    public class ConnectionString
    {
        private string _connectionString;

        /// <summary>
        /// 将连接字符串转换为 <see cref="ConnectionString"/> 对象。
        /// </summary>
        /// <param name="connectionString">表示连接数据库的字符串。</param>
        /// <returns></returns>
        public static implicit operator ConnectionString(string connectionString)
        {
            return new ConnectionString(connectionString);
        }

        /// <summary>
        /// 将 <see cref="ConnectionString"/> 转换为字符串。
        /// </summary>
        /// <param name="connectionString">扩展数据库连接字符串对象。</param>
        /// <returns></returns>
        public static explicit operator string(ConnectionString connectionString)
        {
            return connectionString != null ? connectionString.ToString() : string.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connStr1"></param>
        /// <param name="connStr2"></param>
        /// <returns></returns>
        public static bool operator ==(ConnectionString? connStr1, ConnectionString? connStr2)
        {
            if (Equals(connStr1, null) && Equals(connStr2, null))
            {
                return true;
            }

            if ((Equals(connStr1, null) && !Equals(connStr2, null)) || (!Equals(connStr1, null) && Equals(connStr2, null)))
            {
                return false;
            }

            return connStr1?._connectionString == connStr2?._connectionString;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connStr1"></param>
        /// <param name="connStr2"></param>
        /// <returns></returns>
        public static bool operator !=(ConnectionString? connStr1, ConnectionString? connStr2)
        {
            if (Equals(connStr1, null) && Equals(connStr2, null))
            {
                return false;
            }

            if ((Equals(connStr1, null) && !Equals(connStr2, null)) || (!Equals(connStr1, null) && Equals(connStr2, null)))
            {
                return true;
            }

            return connStr1?._connectionString != connStr2?._connectionString;
        }

        /// <summary>
        /// 使用连接字符串初始化 <see cref="ConnectionString"/> 类的新实例。
        /// </summary>
        /// <param name="connectionString">数据库连接字符串。</param>
        /// <exception cref="ArgumentNullException">connectionString 为 null。</exception>
        public ConnectionString(string connectionString)
        {
            Guard.ArgumentNull(connectionString, nameof(connectionString));

            Properties = new ConnectionProperties(this);

            _connectionString = ParseConnectionString(connectionString.AsSpan());
        }

        /// <summary>
        /// 获取数据库提供者类型名称，该名称可以是 <see cref="ProviderType"/> 枚举或实现 <see cref="IProvider"/> 接口的类的名称。
        /// </summary>
        public string ProviderType { get; private set; }

        /// <summary>
        /// 获取数据库类型名称，该类型实现 <see cref="IDatabase"/> 接口。
        /// </summary>
        public string DatabaseType { get; private set; }

        /// <summary>
        /// 获取提供命令执行的调试跟踪支持。
        /// </summary>
        public bool IsTracking { get; private set; }

        /// <summary>
        /// 获取连接字符串的属性字典。
        /// </summary>
        public ConnectionProperties Properties { get; private set; }

        /// <summary>
        /// 输出处理过的可用于数据库连接的字符串。
        /// </summary>
        /// <returns>数据库连接字符串。</returns>
        public override string ToString()
        {
            return _connectionString;
        }

        /// <summary>
        /// 更新连接字符串。
        /// </summary>
        /// <returns></returns>
        internal void Update()
        {
            var builder = new StringBuilder();

            foreach (var name in Properties.Names)
            {
                if (Properties.IsCustomized(name))
                {
                    continue;
                }

                var value = Properties[name];
                builder.AppendFormat("{0}={1};", name, value);
            }

            _connectionString = builder.ToString();
        }

        /// <summary>
        /// 解析数据库连接字符串，去除字符串内的 version、|datadirectory| 等参数。
        /// </summary>
        /// <param name="constr">配置的源连接字符串。</param>
        /// <returns>可用于进行数据库连接的字符串。</returns>
        private string ParseConnectionString(ReadOnlySpan<char> constr)
        {
            var builder = new StringBuilder();
            var index = 0;
            var name = ReadOnlySpan<char>.Empty;
            var last = false;
            var quote = false;

            var value = ReadOnlySpan<char>.Empty;

            for (int i = 0, n = constr.Length; i < n; i++)
            {
                if (constr[i] == '"' || constr[i] == '\'')
                {
                    quote = !quote;
                }
                else if (constr[i] == '=' && !quote)
                {
                    name = constr.Slice(index, i - index).Trim();
                    index = i + 1;
                }
                else if (constr[i] == ';' && !quote)
                {
                    value = constr.Slice(index, i - index);

                    var isCustomized = ParseParameter(name, value);
                    if (!isCustomized)
                    {
                        value = ResolveFullPath(value).AsSpan();
                        builder.AppendFormat("{0}={1};", name.ToString(), value.ToString());
                    }

                    Properties.Add(name.ToString(), value.ToString(), isCustomized);

                    index = i + 1;
                    last = true;
                }
                else
                {
                    last = false;
                }
            }

            if (!last)
            {
                value = constr.Slice(index);
                var isCustomized = false;

                if (name.IsEmpty)
                {
                    builder.Append(value.ToString());
                }
                else if (!(isCustomized = ParseParameter(name, value)))
                {
                    value = ResolveFullPath(value).AsSpan();
                    builder.AppendFormat("{0}={1};", name.ToString(), value.ToString());
                }

                Properties.Add(name.ToString(), value.ToString(), isCustomized);
            }

            return builder.ToString();
        }

        /// <summary>
        /// 解析参数。
        /// </summary>
        /// <param name="name">参数的名称。</param>
        /// <param name="value">值。</param>
        /// <returns>如果解析出特有的参数，则为 true。</returns>
        private bool ParseParameter(ReadOnlySpan<char> name, ReadOnlySpan<char> value)
        {
            if (name.Equals("version".AsSpan(), StringComparison.OrdinalIgnoreCase))
            {
                //Version = Convert.ToDecimal(value);
            }
            else if (name.Equals("provider type".AsSpan(), StringComparison.OrdinalIgnoreCase))
            {
                ProviderType = value.ToString();
            }
            else if (name.Equals("database type".AsSpan(), StringComparison.OrdinalIgnoreCase))
            {
                DatabaseType = value.ToString();
            }
            else if (name.Equals("tracking".AsSpan(), StringComparison.OrdinalIgnoreCase))
            {
                IsTracking = Convert.ToBoolean(value.ToString());
            }
            else
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 解析字符串中的带目录的参数值。
        /// </summary>
        /// <param name="urlExpression">数据库文件名表达式。</param>
        /// <example>如 |datadirectory|..\data\db1.mdf、|system|db1.mdb 等等。</example>
        private string ResolveFullPath(ReadOnlySpan<char> urlExpression)
        {
            int dirIndex;
            if ((dirIndex = urlExpression.LastIndexOf('|')) != -1)
            {
                var directory = string.Empty;
                var file = urlExpression.Slice(dirIndex + 1);
                if (urlExpression.IndexOf("|datadirectory|".AsSpan(), StringComparison.OrdinalIgnoreCase) != -1)
                {
#if !NETFRAMEWORK
                    var data = AppContext.GetData("DataDirectory");
                    directory = data == null ? AppContext.BaseDirectory : data.ToString();
#else
                    directory = AppContext.BaseDirectory;
#endif
                }
                else if (urlExpression.IndexOf("|appdir|".AsSpan(), StringComparison.OrdinalIgnoreCase) != -1)
                {
                    directory = AppContext.BaseDirectory;
                }
                else
                {
                    var folderName = urlExpression.Slice(1, dirIndex - 1);
                    if (Enum.TryParse(folderName.ToString(), out Environment.SpecialFolder folder))
                    {
                        directory = Environment.GetFolderPath(folder);
                    }
                }

                if (!string.IsNullOrWhiteSpace(directory))
                {
                    if (!directory.EndsWith("\\"))
                    {
                        directory += "\\";
                    }

                    var uri = new Uri(new Uri(directory.ToString()), file.ToString());
                    if (uri.IsFile)
                    {
                        return uri.LocalPath.Replace("file:", string.Empty);
                    }
                    else
                    {
                        return uri.OriginalString;
                    }
                }
            }

            return urlExpression.ToString();
        }
    }
}