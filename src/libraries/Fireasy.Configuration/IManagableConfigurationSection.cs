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
    /// 表示受托管的配置节。
    /// </summary>
    public interface IManagableConfigurationSection
    {
        /// <summary>
        /// 获取实例创建工厂。
        /// </summary>
        IManagedFactory Factory { get; }
    }
}