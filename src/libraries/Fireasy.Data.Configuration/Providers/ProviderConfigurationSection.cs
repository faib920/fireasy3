// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Fireasy.Common;
using Fireasy.Configuration;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace Fireasy.Data.Configuration.Providers
{
    /// <summary>
    /// 表示数据库提供者的配置节。
    /// </summary>
    [ConfigurationSectionStorage("fireasy:dataProviders")]
    public sealed class ProviderConfigurationSection : ConfigurationSection<ProviderConfigurationSetting>
    {
        /// <summary>
        /// 使用配置节点对当前配置进行初始化。
        /// </summary>
        /// <param name="context">对应的配置节点。</param>
        public override void Bind(BindingContext context)
        {
            InternalBind(context, initializer: ParseProviderSetting);
        }

        private ProviderConfigurationSetting ParseProviderSetting(BindingContext context)
        {
            return new ProviderConfigurationSetting()
                {
                    Name = ((Microsoft.Extensions.Configuration.IConfigurationSection)context.Configuration).Key,
                    Type = Type.GetType(GetEllipticalTypeName(context.Configuration.GetSection("type")?.Value), false, true)
                };
        }

        private string GetEllipticalTypeName(string? typeName)
        {
            if (!string.IsNullOrEmpty(typeName) && typeName?.Contains(",") == false)
            {
                typeName = string.Format("Fireasy.Data.Provider.{0}, Fireasy.Data", typeName);
            }

            Guard.ArgumentNull(typeName, nameof(typeName));

            return typeName!;
        }
    }
}
