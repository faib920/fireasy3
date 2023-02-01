// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Microsoft.Extensions.Configuration;

namespace Fireasy.Configuration
{
    /// <summary>
    /// 提供对配置项的解析方法。
    /// </summary>
    public interface IConfigurationSettingParseHandler
    {
        /// <summary>
        /// 将节点信息解析为配置项。
        /// </summary>
        /// <param name="context">配置属性。</param>
        /// <returns></returns>
        IConfigurationSettingItem Parse(BindingContext context);
    }
}