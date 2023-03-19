// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Fireasy.Configuration;
using System;

namespace Fireasy.Composition.Configuration
{
    /// <summary>
    /// 表示 MEF 的导入配置节。
    /// </summary>
    [ConfigurationSectionStorage("fireasy:imports")]
    public sealed class ImportConfigurationSection : ConfigurationSection<ImportConfigurationSetting>
    {
        /// <summary>
        /// 使用配置节点对当前配置进行初始化。
        /// </summary>
        /// <param name="context">对应的配置节点。</param>
        public override void Bind(BindingContext context)
        {
            InternalBind(context,
                "settings",
                initializer: c => new ImportConfigurationSetting
                {
                    Assembly = c.Configuration.GetSection("assembly").Value,
                    ContractType = Type.GetType(c.Configuration.GetSection("contractType").Value ?? string.Empty, false, true),
                    ImportType = Type.GetType(c.Configuration.GetSection("importType").Value ?? string.Empty, false, true)
                });
        }
    }
}
