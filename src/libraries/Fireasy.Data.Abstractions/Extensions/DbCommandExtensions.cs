// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Fireasy.Common;
using Fireasy.Data.Provider;
using Fireasy.Data.Syntax;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace Fireasy.Data.Extensions
{
    public static class DbCommandExtensions
    {
        /// <summary>
        /// 输出 <see cref="DbCommand"/> 的命令文本及参数。
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public static string Output(this IDbCommand command)
        {
            var sb = new StringBuilder();
            if (command.Parameters.Count > 0)
            {
                sb.AppendFormat("'{0}'", command.CommandText);
                foreach (DbParameter par in command.Parameters)
                {
                    //字符或日期型，加'
                    if (par.Value is string || par.Value is DateTime || par.Value is char)
                    {
                        sb.AppendFormat(",{0}='{1}'", par.ParameterName, TrimStringValue(par.Value));
                    }

                    //字节数组，转换为字符串
                    else if (par.Value is byte[])
                    {
                        sb.AppendFormat(",{0}='{1}'", par.ParameterName, TrimByteValue(par.Value as byte[]));
                    }
                    else
                    {
                        sb.AppendFormat(",{0}={1}", par.ParameterName, par.Value);
                    }
                }
            }
            else
            {
                sb.Append(command.CommandText);
            }

            return sb.ToString();
        }

        /// <summary>
        /// 清理IDbCommand中的参数。
        /// </summary>
        /// <param name="command"></param>
        public static void ClearParameters(this IDbCommand command)
        {
            if (command.Parameters.Count != 0)
            {
                command.Parameters.Clear();
            }
        }

        /// <summary>
        /// 同步IDbCommand中的输出型参数。
        /// </summary>
        /// <param name="command"></param>
        /// <param name="parameters"></param>
        public static void SyncParameters(this IDbCommand command, IEnumerable<Parameter>? parameters)
        {
            if (command.Parameters.Count == 0 ||
                parameters == null)
            {
                return;
            }

            foreach (var paramter in parameters)
            {
                if (!paramter.IsOutput())
                {
                    continue;
                }

                var par = command.Parameters[paramter.ParameterName];
                if (par != null)
                {
                    paramter.Value = ((DbParameter)par).Value;
                }
            }
        }

        /// <summary>
        /// 将参数添加到DbCommand。
        /// </summary>
        /// <param name="command"></param>
        /// <param name="provider"></param>
        /// <param name="parameters"></param>
        public static void PrepareParameters(this DbCommand command, IProvider provider, IEnumerable<Parameter> parameters)
        {
            var syntax = provider.GetService<ISyntaxProvider>();

            foreach (var parameter in parameters)
            {
                if (command.CommandType == CommandType.Text && parameter.Value is IEnumerable && !(parameter.Value is string) && !(parameter.Value is byte[]))
                {
                    var parmeterName = parameter.ParameterName[0] == syntax.ParameterPrefix[0] ? parameter.ParameterName : syntax.ParameterPrefix + parameter.ParameterName;

                    var sb = new StringBuilder();
                    var index = 0;
                    foreach (var value in parameter.Value as IEnumerable)
                    {
                        if (sb.Length > 0)
                        {
                            sb.Append(", ");
                        }

                        var name = string.Concat(parmeterName, "_auto_", index++);
                        var newPar = new Parameter(name, value);
                        sb.Append(name);
                        command.Parameters.Add(provider.PrepareParameter(newPar.ToDbParameter(provider)));
                    }

                    command.CommandText = command.CommandText.Replace(parmeterName, sb.ToString());
                }
                else
                {
                    command.Parameters.Add(provider.PrepareParameter(parameter.ToDbParameter(provider)));
                }
            }
        }

        private static string? TrimStringValue(object value)
        {
            if (value == null)
            {
                return null;
            }

            var str = value.ToString();

            if (str.Length > 128)
            {
                return str.Substring(0, 128) + $"...<<{str.Length}>>";
            }

            return str;
        }

        private static string? TrimByteValue(byte[] bytes)
        {
            if (bytes == null)
            {
                return null;
            }

            if (bytes.Length > 128)
            {
                return Encoding.UTF8.GetString(bytes.Take(128).ToArray()) + $"...<<{bytes.Length}>>";
            }

            return Encoding.UTF8.GetString(bytes);
        }
    }
}
