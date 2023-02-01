// <copyright company="Faib Studio"
//      email="faib920@126.com"
//      qq="55570729"
//      date="2011-2-16">
//   (c) Copyright Faib Studio 2011. All rights reserved.
// </copyright>
// ---------------------------------------------------------------

using Fireasy.Common.Serialization;
using Fireasy.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;

namespace Fireasy.Data.Configuration
{
    /// <summary>
    /// 一个提供数据库字符串配置的类，使用二进制文件进行配置。
    /// </summary>
    [Serializable]
    public sealed class BinaryInstanceSetting : DefaultInstanceConfigurationSetting
    {
        /// <summary>
        /// 返回二进制文件名称。
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
                var setting = new BinaryInstanceSetting();

                if (!File.Exists(fileName))
                {
                    throw new FileNotFoundException($"无法找到文件{setting.FileName}。", setting.FileName);
                }

                setting.FileName = fileName;

                FileStream stream = null;
                try
                {
                    //读取文件
                    stream = new FileStream(setting.FileName, FileMode.Open, FileAccess.Read);
                    var bytes = new byte[stream.Length];
                    stream.Read(bytes, 0, bytes.Length);
                    stream.Close();
                    stream = null;

                    //反序列化
                    var binarySerializer = context.ServiceProvider.GetService<IBinarySerializer>();
                    var store = binarySerializer?.Deserialize<BinaryConnectionStore>(bytes);
                    if (store != null)
                    {
                        setting.ProviderType = store.ProviderType;
                        setting.ConnectionString = ConnectionStringHelper.GetConnectionString(store.ConnectionString);
                    }
                }
                catch
                {
                    throw new InvalidOperationException("数据解析失败。");
                }
                finally
                {
                    if (stream != null)
                    {
                        stream.Dispose();
                    }
                }

                return setting;
            }
        }
    }
}
