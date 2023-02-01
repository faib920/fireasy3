// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Fireasy.Common.Extensions;
using Fireasy.Common.Serialization;
using Fireasy.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;

namespace Fireasy.Data.Configuration
{
    /// <summary>
    /// 一个提供数据库字符串配置的类，使用Json文件进行配置。
    /// </summary>
    [Serializable]
    public sealed class JsonInstanceSetting : DefaultInstanceConfigurationSetting
    {
        /// <summary>
        /// 返回Xml文件名称。
        /// </summary>
        public string FileName { get; set; }

        internal class SettingParseHandler : IConfigurationSettingParseHandler
        {
            public IConfigurationSettingItem Parse(BindingContext context)
            {
                var fileName = context.Configuration.GetSection("fileName").Value;
                return Parse(context, fileName);
            }

            private IConfigurationSettingItem Parse(BindingContext context, string fileName)
            {
                if (!File.Exists(fileName))
                {
                    throw new FileNotFoundException($"无法找到文件{fileName}。", fileName);
                }

                var content = File.ReadAllText(fileName);
                var jsonSerializer = context.ServiceProvider.GetRequiredService<IJsonSerializer>();
                var dict = (IDictionary<string, object>)jsonSerializer.Deserialize<object>(content);
                var setting = new JsonInstanceSetting
                {
                    FileName = fileName,
                    ProviderType = dict.TryGetValue("providerType", () => string.Empty)?.ToString()
                };

                if (dict.TryGetValue("connectionString", out object connectionString))
                {
                    setting.ConnectionString = ConnectionStringHelper.GetConnectionString((string)connectionString);
                }

                return setting;
            }
        }
    }
}
