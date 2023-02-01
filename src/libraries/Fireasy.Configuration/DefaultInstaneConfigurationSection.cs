// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Linq;

namespace Fireasy.Configuration
{
    /// <summary>
    /// 具有默认实例配置的配置节。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class DefaultInstaneConfigurationSection<T> : ConfigurationSection<T>, IDefaultConfigurationSection where T : IConfigurationSettingItem
    {
        /// <summary>
        /// 获取或设置默认配置实例名称。
        /// </summary>
        public string DefaultInstanceName { get; set; }

        public IConfigurationSettingItem GetDefault()
        {
            if (Settings.Count == 0)
            {
                return null;
            }

            if (string.IsNullOrEmpty(DefaultInstanceName))
            {
                if (Settings.ContainsKey("setting0"))
                {
                    return GetSetting("setting0");
                }

                return GetSettings().FirstOrDefault();
            }

            return GetSetting(DefaultInstanceName);
        }

        /// <summary>
        /// 获取默认的配置项。
        /// </summary>
        public T Default
        {
            get
            {
                var setting = GetDefault();
                if (setting == null)
                {
                    return default;
                }

                if (setting is ExtendConfigurationSetting extend)
                {
                    return (T)extend.Basic;
                }

                return (T)setting;
            }
        }

        public override void Bind(BindingContext context)
        {
            DefaultInstanceName = context.Configuration.GetSection("Default").Value;

            base.InternalBind(context);
        }
    }
}