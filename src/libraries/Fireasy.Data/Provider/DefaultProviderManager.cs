﻿// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Fireasy.Common.DependencyInjection;
using Fireasy.Common.ObjectActivator;
using Fireasy.Common.Threading;
using Fireasy.Configuration;
using Fireasy.Data.Configuration.Providers;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fireasy.Data.Provider
{
    /// <summary>
    /// <see cref="IProvider"/> 的管理类。
    /// </summary>
    public class DefaultProviderManager : IProviderManager
    {
        private List<ProviderWrapper> _providerWappers;
        private readonly IServiceProvider _serviceProvider;
        private readonly ConfigurationUnity? _configurationUnity;
        private readonly ProviderCustomizer? _customizer;

        /// <summary>
        /// 初始化 <see cref="DefaultProviderManager"/> 类的新实例。
        /// </summary>
        /// <param name="serviceProvider"></param>
        public DefaultProviderManager(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _configurationUnity = _serviceProvider!.GetService<ConfigurationUnity>();

            var accessor = serviceProvider.GetService<IObjectAccessor<ProviderCustomizer>>();
            _customizer = accessor?.Value;
        }

        /// <summary>
        /// 根据 <paramref name="providerName"/> 获取对应的 <see cref="IProvider"/> 对象。
        /// </summary>
        /// <param name="providerName">提供者名称。</param>
        /// <returns></returns>
        public IProvider? GetDefinedProviderInstance(string providerName)
        {
            var wapper = GetWrapper().FirstOrDefault(s => s.Contains(providerName));
            if (wapper != null)
            {
                var objActivator = _serviceProvider.GetRequiredService<IObjectActivator>();
                return objActivator.CreateInstance(wapper.ProviderType) as IProvider;
            }

            return null;
        }

        /// <summary>
        /// 获取所提供的所有数据库提供者名称。
        /// </summary>
        /// <returns></returns>
        public string[] GetSupportedProviderNames()
        {
            return GetWrapper().SelectMany(s => s.Alias).ToArray();
        }

        private void AddProvider<T>(string providerName) where T : IProvider
        {
            AddProvider(providerName, typeof(T));
        }

        private void AddProvider(string providerName, Type providerType)
        {
            AddProvider(new[] { providerName }, providerType);
        }

        private void AddProvider(string[] providerNames, Type providerType)
        {
            ProviderWrapper? wapper = null;
            if ((wapper = GetWrapper().FirstOrDefault(s => s.ProviderType == providerType)) != null)
            {
                foreach (var name in providerNames)
                {
                    if (!wapper.Alias.Contains(name))
                    {
                        wapper.Alias.Add(name);
                    }
                }
            }
            else
            {
                GetWrapper().Add(new ProviderWrapper
                {
                    Alias = new List<string>(providerNames),
                    ProviderType = providerType
                });
            }
        }

        private List<ProviderWrapper> GetWrapper()
        {
            return SingletonLocker.Lock(ref _providerWappers, InitializeProviders);
        }

        /// <summary>
        /// 初始化提供者。
        /// </summary>
        private List<ProviderWrapper> InitializeProviders()
        {
            _providerWappers = new List<ProviderWrapper>();

            //预配置
            _customizer?.EachDbProviderTypes((n, t) => AddProvider(n, t));

            RegisterCustomProviders();

            //内置的提供者
            AddProvider<SqlServerProvider>("SqlServer");
            AddProvider<OracleProvider>("Oracle");
            AddProvider<SQLiteProvider>("SQLite");
            AddProvider<MySqlProvider>("MySql");
            AddProvider<PostgreSqlProvider>("PostgreSql");
            AddProvider<FirebirdProvider>("Firebird");
            AddProvider<DamengProvider>("Dameng");
            AddProvider<KingbaseProvider>("Kingbase");
            AddProvider<ShenTongProvider>("ShenTong");

            return _providerWappers;
        }

        /// <summary>
        /// 使用配置注册自定义的插件服务。
        /// </summary>
        private void RegisterCustomProviders()
        {
            //取配置，注册自定义提供者
            var section = _configurationUnity?.GetSection<ProviderConfigurationSection>();
            if (section == null)
            {
                return;
            }

            foreach (var key in section.Settings.Keys)
            {
                var setting = section.Settings[key];
                if (setting?.Type == null)
                {
                    continue;
                }

                AddProvider(setting.Name, setting.Type);
            }
        }
    }

    internal class ProviderWrapper
    {
        internal List<string> Alias { get; set; } = new List<string>();

        internal Type ProviderType { get; set; }

        internal bool Contains(string name)
        {
            return Alias.Any(s => s.Equals(name, StringComparison.OrdinalIgnoreCase));
        }
    }
}
