// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Fireasy.Common.ObjectActivator;
using Fireasy.Common.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fireasy.Configuration
{
    /// <summary>
    /// 一个抽象类，表示配置节的信息。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class ConfigurationSection<T> : ConfigurationSection, IMultipleConfigurationSection where T : IConfigurationSettingItem
    {
        private readonly ConfigurationSettings<IConfigurationSettingItem> _innerSettings = new();
        private ConfigurationSettings<T>? _settings;
        private static readonly object _locker = new();

        /// <summary>
        /// 解析配置节下的所有子节点。
        /// </summary>
        /// <param name="context">配置上下文对象。</param>
        /// <param name="nodeName">要枚举的子节点的名称。默认为 settings。</param>
        /// <param name="typeNodeName">如果配置类中存在 <see cref="Type"/> 的属性，则指定该属性的名称。默认为 type。</param>
        /// <param name="initializer">用于初始化设置项的函数。</param>
        protected void InternalBind(BindingContext context, string nodeName = "settings", string typeNodeName = "type", Func<BindingContext, IConfigurationSettingItem>? initializer = null)
        {
            foreach (var child in context.Configuration.GetSection(nodeName).GetChildren())
            {
                var bindConfiguration = new BindingConfiguration(context.Configuration, child);
                var name = child.Key;
                if (string.IsNullOrEmpty(name))
                {
                    name = string.Concat("setting", _innerSettings.Count);
                }

                if (initializer == null)
                {
                    continue;
                }

                try
                {
                    var setting = initializer(new BindingContext(context.ServiceProvider, bindConfiguration));
                    if (!string.IsNullOrEmpty(typeNodeName))
                    {
                        var typeName = child.GetSection(typeNodeName).Value;
                        if (!string.IsNullOrEmpty(typeName))
                        {
                            var type = Type.GetType(typeName, true, true);

                            var extend = TryParseExtendSetting(new BindingContext(context.ServiceProvider, bindConfiguration), type);
                            if (extend != null)
                            {
                                setting = new ExtendConfigurationSetting { Basic = setting, Extend = extend };
                            }
                        }
                    }

                    if (setting is INamedIConfigurationSettingItem named)
                    {
                        named.Name = name;
                    }

                    if (setting != null)
                    {
                        _innerSettings.Add(name, setting);
                    }
                }
                catch (Exception ex)
                {
                    _innerSettings.AddInvalidSetting(name, ex);
                }
            }
        }

        /// <summary>
        /// 返回绑定时是否记录异常。
        /// </summary>
        /// <param name="exp">异常实例。</param>
        /// <returns></returns>
        public override bool HasException(out Exception? exp)
        {
            exp = _innerSettings.HasException();
            return exp != null;
        }

        /// <summary>
        /// 返回当前节的配置项集合。
        /// </summary>
        public ConfigurationSettings<T> Settings
        {
            get
            {
                if (_settings == null)
                {
                    lock (_locker)
                    {
                        if (_settings != null)
                        {
                            return _settings;
                        }

                        _settings = new ConfigurationSettings<T>();

                        foreach (var kvp in _innerSettings)
                        {
                            if (kvp.Value is ExtendConfigurationSetting extend)
                            {
                                _settings.Add(kvp.Key, (T)extend.Basic);
                            }
                            else
                            {
                                _settings.Add(kvp.Key, (T)kvp.Value);
                            }
                        }
                    }
                }

                return _settings;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        int IMultipleConfigurationSection.Count => Settings.Count;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IConfigurationSettingItem?> GetSettings() => _innerSettings.Values;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IConfigurationSettingItem? GetSetting(string name)
        {
            if (_innerSettings.ContainsKey(name))
            {
                return _innerSettings[name];
            }

            return null;
        }

        private IConfigurationSettingItem? TryParseExtendSetting(BindingContext context, Type type)
        {
            var attSection = type.GetCustomAttributes<ConfigurationSettingAttribute>(true).FirstOrDefault();
            if (attSection != null)
            {
                var attParse = attSection.Type.GetCustomAttributes<ConfigurationSettingParseTypeAttribute>().FirstOrDefault();
                if (attParse == null)
                {
                    return Activator.CreateInstance(attSection.Type) as IConfigurationSettingItem;
                }
                else
                {
                    var objActivator = context.ServiceProvider.GetRequiredService<IObjectActivator>();
                    var handler = objActivator.CreateInstance(attParse.HandlerType) as IConfigurationSettingParseHandler;
                    if (handler != null)
                    {
                        return handler.Parse(context);
                    }
                }
            }

            return null;
        }
    }
}
