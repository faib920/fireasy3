// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Fireasy.Configuration;
using System;

namespace Fireasy.Data.Configuration.Providers
{
    /// <summary>
    /// 提供者的配置。
    /// </summary>
    public class ProviderConfigurationSetting : IConfigurationSettingItem
    {
        /// <summary>
        /// 获取或设置配置的名称。
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 获取或设置提供者的类型。
        /// </summary>
        public Type? Type { get; set; }
    }
}
