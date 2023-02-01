// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Fireasy.Common.Collections;
using Fireasy.Common.Extensions;
using Fireasy.Configuration.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Reflection;

namespace Fireasy.Configuration
{
    /// <summary>
    /// 应用程序配置的管理单元。
    /// </summary>
    public sealed class ConfigurationUnity
    {
        private readonly ExtraConcurrentDictionary<string, IConfigurationSection> _sections = new ExtraConcurrentDictionary<string, IConfigurationSection>();
        private readonly ILogger<ConfigurationUnity> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IConfiguration _configuration;

        /// <summary>
        /// 初始化 <see cref="ConfigurationUnity"/> 类的新实例。
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="serviceProvider"></param>
        /// <param name="configuration"></param>
        public ConfigurationUnity(ILogger<ConfigurationUnity> logger, IServiceProvider serviceProvider, IConfiguration configuration)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _configuration = configuration;
        }

        /// <summary>
        /// 获取配置节实例。
        /// </summary>
        /// <typeparam name="T">配置节的类型。</typeparam>
        /// <returns></returns>
        public T? GetSection<T>() where T : IConfigurationSection, new()
        {
            var attribute = typeof(T).GetCustomAttributes<ConfigurationSectionStorageAttribute>().FirstOrDefault();
            if (attribute == null)
            {
                return default;
            }

            var section = _sections.GetOrAdd(attribute.Path, path =>
            {
                var context = new BindingContext(_serviceProvider, _configuration.GetSection(path));
                var section = new T();
                section.Bind(context);

                if (section.HasException(out var exp))
                {
                    _logger?.LogError(exp, $"Read configuration section of '{path}'.");
                }
                else if (section is IMultipleConfigurationSection multipleSect)
                {
                    _logger?.LogDebug($"The {typeof(T).Name} was bound ({multipleSect.Count} items).");
                }
                else
                {
                    _logger?.LogDebug($"The {typeof(T).Name} was bound.");
                }

                return section;
            });

            return section == null ? default : (T)section;
        }
    }
}
