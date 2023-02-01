// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Fireasy.Configuration;
using Microsoft.Extensions.Configuration;
using System;
using System.Xml;

namespace Fireasy.Data.Configuration
{
    /// <summary>
    /// 一个提供数据库字符串配置的类，使用配置文件中的字符串进行配置。
    /// </summary>
    [Serializable]
    public sealed class StringInstanceSetting : DefaultInstanceConfigurationSetting
    {
        /// <summary>
        /// 返回字符串的引用类型。
        /// </summary>
        public StringReferenceType ReferenceType { get; set; }

        internal class SettingParseHandler : IConfigurationSettingParseHandler
        {
            public IConfigurationSettingItem Parse(BindingContext context)
            {
                var storeType = context.Configuration.GetSection("storeType").Value;
                var providerType = context.Configuration.GetSection("providerType").Value;
                var databaseType = context.Configuration.GetSection("databaseType").Value;
                var clusters = context.Configuration.GetSection("clusters");
                var key = context.Configuration.GetSection("key").Value;

                if (clusters.Exists())
                {
                    return Parse(storeType, providerType, databaseType, key, clusters);
                }

                var connectionString = context.Configuration.GetSection("connectionString").Value;

                return Parse(storeType, providerType, databaseType, key, connectionString);
            }

            private IConfigurationSettingItem Parse(string storeType, string providerType, string databaseType, string key, string connectionString)
            {
                var setting = new StringInstanceSetting
                {
                    ProviderType = providerType,
                };

                switch (string.Concat(string.Empty, storeType).ToLower())
                {
                    case "appsettings":
                        break;
                    case "connectionstrings":
                        break;
                    default:
                        setting.ReferenceType = StringReferenceType.String;
                        setting.ConnectionString = ConnectionStringHelper.GetConnectionString(connectionString);
                        break;
                }
                return setting;
            }

            private IConfigurationSettingItem Parse(string storeType, string providerType, string databaseType, string key, XmlNode clusters)
            {
                var setting = new StringInstanceSetting
                {
                    ProviderType = providerType,
                };

                var master = clusters.SelectSingleNode("master");
                var slaves = clusters.SelectSingleNode("slaves");

                if (master != null)
                {
                    var connstr = master.Attributes["connectionString"]?.Value;
                    var cluster = new ClusteredConnectionSetting
                    {
                        ConnectionString = ConnectionStringHelper.GetConnectionString(connstr),
                        Mode = DistributedMode.Master
                    };
                    setting.Clusters.Add(cluster);
                }

                if (slaves != null)
                {
                    foreach (XmlNode node in slaves.ChildNodes)
                    {
                        var connstr = node.Attributes["connectionString"].Value;
                        var cluster = new ClusteredConnectionSetting
                        {
                            ConnectionString = ConnectionStringHelper.GetConnectionString(connstr),
                            Mode = DistributedMode.Slave,
                            Weight = Convert.ToInt32(node.Attributes["weight"].Value)
                        };
                        setting.Clusters.Add(cluster);
                    }
                }

                return setting;
            }

            private IConfigurationSettingItem Parse(string storeType, string providerType, string databaseType, string key, IConfiguration clusters)
            {
                var setting = new StringInstanceSetting
                {
                    ProviderType = providerType,
                };

                var master = clusters.GetSection("master");
                var slaves = clusters.GetSection("slaves");

                if (master.Exists())
                {
                    var connstr = master["connectionString"];
                    var cluster = new ClusteredConnectionSetting
                        {
                            ConnectionString = ConnectionStringHelper.GetConnectionString(connstr),
                            Mode = DistributedMode.Master
                        };
                    setting.Clusters.Add(cluster);
                }

                if (slaves.Exists())
                {
                    foreach (var child in slaves.GetChildren())
                    {
                        var connstr = child["connectionString"];
                        var cluster = new ClusteredConnectionSetting
                            {
                                ConnectionString = ConnectionStringHelper.GetConnectionString(connstr),
                                Mode = DistributedMode.Slave,
                                Weight = Convert.ToInt32(child["weight"])
                            };
                        setting.Clusters.Add(cluster);
                    }
                }

                return setting;
            }
        }
    }

    /// <summary>
    /// 字符串引用类别。
    /// </summary>
    public enum StringReferenceType
    {
        /// <summary>
        /// 直接给定的字符串。
        /// </summary>
        String,
        /// <summary>
        /// 使用appSettings配置节。
        /// </summary>
        AppSettings,
        /// <summary>
        /// 使用connectionStrings配置节。
        /// </summary>
        ConnectionStrings
    }
}
