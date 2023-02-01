// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Fireasy.Configuration;
using System;
using System.IO;
using System.Xml;
using System.Xml.XPath;

namespace Fireasy.Data.Configuration
{
    /// <summary>
    /// 一个提供数据库字符串配置的类，使用XML文件进行配置。
    /// </summary>
    [Serializable]
    public sealed class XmlInstanceSetting : DefaultInstanceConfigurationSetting
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
                var xpath = context.Configuration.GetSection("xmlPath").Value;
                return Parse(fileName, xpath);
            }

            private IConfigurationSettingItem Parse(string fileName, string xpath)
            {
                if (!File.Exists(fileName))
                {
                    throw new FileNotFoundException($"无法找到文件{fileName}。", fileName);
                }

                var setting = new XmlInstanceSetting
                {
                    FileName = fileName
                };

                var xmlDoc = new XmlDocument();
                xmlDoc.Load(fileName);
                XmlNode connNode = null;

                if (!string.IsNullOrEmpty(xpath))
                {
                    xmlDoc.SelectSingleNode(xpath);
                    if (connNode == null)
                    {
                        throw new XPathException(xpath);
                    }
                }
                else
                {
                    connNode = xmlDoc.DocumentElement;
                }

                setting.ProviderType = connNode.SelectSingleNode("providerType")?.InnerText ?? connNode.Attributes["providerType"]?.Value;

                setting.ConnectionString = ConnectionStringHelper.GetConnectionString(connNode.SelectSingleNode("connectionString")?.InnerText ?? connNode["connectionString"]?.Value);

                return setting;
            }
        }
    }
}
