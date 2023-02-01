// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Fireasy.Common.ObjectActivator;
using Fireasy.Common.Extensions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace Fireasy.Configuration
{
    /// <summary>
    /// 提供可配置工厂的抽象方法。
    /// </summary>
    /// <typeparam name="TSection"></typeparam>
    /// <typeparam name="TInstance"></typeparam>
    public abstract class ConfigurableFactory<TSection, TInstance> where TSection : ConfigurationSection, new() where TInstance : class, new()
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly Dictionary<string, Func<IServiceProvider, TInstance?>> _container;
        private const string NULL_NAME = "__nullable";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceProvider"></param>
        protected ConfigurableFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _container = new Dictionary<string, Func<IServiceProvider, TInstance?>>();
        }

        /// <summary>
        /// 创建实例对象。
        /// </summary>
        /// <param name="configName">应用程序配置项的名称。</param>
        /// <returns></returns>
        protected virtual TInstance? Create(string configName)
        {
            configName ??= NULL_NAME;

            if (_container.TryGetValue(configName, out var creator) && creator != null)
            {
                return creator(_serviceProvider);
            }

            IConfigurationSettingItem? setting = null;
            var unity = _serviceProvider.GetService<ConfigurationUnity>();
            var section = unity!.GetSection<TSection>();
            if (section is IManagableConfigurationSection managableSect && managableSect.Factory != null)
            {
                _container.TryAdd(configName, sp => managableSect.Factory.CreateInstance(sp, configName) as TInstance);
            }

            if (section is IDefaultConfigurationSection defaultSect)
            {
                if (string.IsNullOrEmpty(configName) || (setting = defaultSect.GetDefault()) == null)
                {
                    return _serviceProvider!.GetService<TInstance>();
                }
            }
            else if (section is IMultipleConfigurationSection multipleSect)
            {
                setting = multipleSect.GetSetting(configName);
            }

            IConfigurationSettingItem? extend = null;
            if (setting is ExtendConfigurationSetting extendSetting)
            {
                setting = extendSetting.Basic;
                extend = extendSetting.Extend;
            }

            if (setting is ICreatableSettingItem createSetting)
            {
                _container.Add(configName, sp => sp!.TryGetService(() => OnAttachExtendSetting(OnInitialize(sp.GetRequiredService<IObjectActivator>().CreateInstance(createSetting.CreationType) as TInstance, section), extend)));
            }

            if (_container.TryGetValue(configName, out creator) && creator != null)
            {
                return creator(_serviceProvider);
            }

            return default;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="extend"></param>
        /// <returns></returns>
        protected virtual TInstance? OnAttachExtendSetting(TInstance? instance, IConfigurationSettingItem extend)
        {
            if (extend != null && instance is IConfigurationSettingHostService hostService)
            {
                hostService.Attach(extend);
            }

            return instance;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="section"></param>
        /// <returns></returns>
        protected virtual TInstance? OnInitialize(TInstance? instance, TSection section)
        {
            return instance;
        }
    }
}
