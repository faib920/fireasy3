// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Fireasy.Common;
using Fireasy.Configuration;
using Fireasy.Data.Converter;
using System;

namespace Fireasy.Data.Configuration.Converters
{
    /// <summary>
    /// 表示数据转换器的配置节。对应的配置节为 fireasy:dataConverters。
    /// </summary>
    [ConfigurationSectionStorage("fireasy:dataConverters")]
    public sealed class ConverterConfigurationSection : ConfigurationSection<ConverterConfigurationSetting>
    {
        /// <summary>
        /// 使用配置节点对当前配置进行初始化。
        /// </summary>
        /// <param name="context">对应的配置节点。</param>
        public override void Bind(BindingContext context)
        {
            InternalBind(context, initializer: c =>
                {
                    var sourceTypeName = c.Configuration.GetSection("sourceType").Value;
                    var converterTypeName = c.Configuration.GetSection("converterType").Value;

                    Guard.ArgumentNull(converterTypeName, nameof(converterTypeName));

                    return GetSetting(sourceTypeName, converterTypeName);
                });
        }

        private static ConverterConfigurationSetting GetSetting(string? sourceTypeName, string? converterTypeName)
        {
            Guard.ArgumentNull(sourceTypeName, nameof(sourceTypeName));
            Guard.ArgumentNull(converterTypeName, nameof(converterTypeName));

            Type? sourceType = null;
            var converterType = Type.GetType(converterTypeName, false, true);

            if (converterType == null || !typeof(IValueConverter).IsAssignableFrom(converterType))
            {
                throw new Exception($"converterType 未配置或该类型未实现 {typeof(IValueConverter).FullName} 接口。");
            }

            if (!string.IsNullOrEmpty(sourceTypeName))
            {
                sourceType = Type.GetType(sourceTypeName, false, true);
            }
            else
            {
                var baseType = converterType.BaseType;
                while (baseType != typeof(object))
                {
                    if (baseType.IsGenericType)
                    {
                        sourceType = baseType.GetGenericArguments()[0];
                        break;
                    }

                    baseType = baseType.BaseType;
                }
            }

            Guard.ArgumentNull(sourceType, nameof(sourceType));

            return new ConverterConfigurationSetting
            {
                SourceType = sourceType!,
                ConverterType = converterType!
            };
        }

    }
}
