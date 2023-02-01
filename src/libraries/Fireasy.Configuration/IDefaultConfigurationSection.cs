// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
namespace Fireasy.Configuration
{
    /// <summary>
    /// 表示具有默认实例的配置节。
    /// </summary>
    public interface IDefaultConfigurationSection
    {
        /// <summary>
        /// 获取或设置默认实例名称。
        /// </summary>
        string DefaultInstanceName { get; set; }

        /// <summary>
        /// 获取默认配置实例。
        /// </summary>
        /// <returns></returns>
        IConfigurationSettingItem GetDefault();
    }
}
