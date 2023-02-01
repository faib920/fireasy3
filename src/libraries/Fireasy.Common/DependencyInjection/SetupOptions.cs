// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Fireasy.Common
{
    /// <summary>
    /// 配置选项。无法继承此类。
    /// </summary>
    public sealed class SetupOptions
    {
        /// <summary>
        /// 获取服务发现选项。
        /// </summary>
        public DiscoverOptions DiscoverOptions { get; } = new();
    }
}
