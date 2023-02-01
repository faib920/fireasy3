// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;

namespace Fireasy.Configuration
{
    /// <summary>
    /// 表示多个配置项的配置节。
    /// </summary>
    public interface IMultipleConfigurationSection
    {
        /// <summary>
        /// 获取所有配置项。
        /// </summary>
        /// <returns></returns>
        IEnumerable<IConfigurationSettingItem?> GetSettings();

        /// <summary>
        /// 获取指定名称的配置项。
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        IConfigurationSettingItem? GetSetting(string name);

        /// <summary>
        /// 返回配置的节点数。
        /// </summary>
        int Count { get; }
    }
}

