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

namespace Fireasy.Data.Configuration
{
    /// <summary>
    /// 数据库实例配置类。对应的配置节为 fireasy:dataInstances。
    /// </summary>
    [ConfigurationSectionStorage("fireasy:dataInstances")]
    public sealed class InstanceConfigurationSection : DefaultInstaneConfigurationSection<IInstanceConfigurationSetting>
    {
        /// <summary>
        /// 使用配置节点对当前配置进行初始化。
        /// </summary>
        /// <param name="context">对应的配置节点。</param>
        public override void Bind(BindingContext context)
        {
            InternalBind(context,
                "settings",
                null,
                CreateDataInstanceSetting);

            DefaultInstanceName = context.Configuration.GetSection("default").Value;

            base.Bind(context);
        }

        /// <summary>
        /// 根据实例名创建相应的配置实例。
        /// </summary>
        /// <param name="context">Section节点。</param>
        /// <returns>返回相应类型的配置实例。</returns>
        private static IInstanceConfigurationSetting CreateDataInstanceSetting(BindingContext context)
        {
            var typeName = context.Configuration.GetSection("handlerType").Value;
            var handerType = string.IsNullOrEmpty(typeName) ? null : Type.GetType(typeName, false, true);
            var handler = handerType != null ? Activator.CreateInstance<IConfigurationSettingParseHandler>() :
                InstanceParseHandleFactory.GetParseHandler(context.Configuration.GetSection("storeType").Value);

            if (handler != null)
            {
                return handler.Parse(context) as IInstanceConfigurationSetting;
            }

            return null;
        }
    }
}
