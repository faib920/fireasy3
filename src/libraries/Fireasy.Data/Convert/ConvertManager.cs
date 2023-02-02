// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Fireasy.Configuration;
using Fireasy.Data.Configuration.Converters;

namespace Fireasy.Data.Converter
{
    /// <summary>
    /// 转换器管理器。
    /// </summary>
    public class ConvertManager : IConvertManager
    {
        private readonly ConfigurationUnity _configurationUnity;

        public ConvertManager(ConfigurationUnity configurationUnity)
        {
            _configurationUnity = configurationUnity;
        }

        /// <summary>
        /// 根据对象类型创建相应的转换器。
        /// </summary>
        /// <param name="conversionType">要转换的数据类型。</param>
        /// <returns>返回一个 <see cref="IValueConverter"/> 实例，如果未找到对应的转换器，则返回 null。</returns>
        public IValueConverter? GetConverter(Type conversionType)
        {
            //从配置里找 IValueConverter 对象
            var section = _configurationUnity.GetSection<ConverterConfigurationSection>();
            if (section != null)
            {
                var setting = section.Settings.FirstOrDefault(s => s.Value?.SourceType == conversionType).Value;
                if (setting != null)
                {
                    return Activator.CreateInstance(setting.ConverterType) as IValueConverter;
                }
            }

            if (conversionType.IsArray)
            {
                return new ArrayConverter(conversionType.GetElementType());
            }

            return null;
        }

        /// <summary>
        /// 判断指定的类型是否支持转换。
        /// </summary>
        /// <param name="conversionType"></param>
        /// <returns></returns>
        public bool CanConvert(Type conversionType)
        {
            //从配置里找 IValueConverter 对象
            var section = _configurationUnity.GetSection<ConverterConfigurationSection>();
            if (section != null)
            {
                var setting = section.Settings.FirstOrDefault(s => s.Value?.SourceType == conversionType).Value;
                if (setting != null)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
