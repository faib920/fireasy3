// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Fireasy.Common.DependencyInjection;
using Fireasy.Common.Threading;
using Fireasy.Composition.Configuration;
using Fireasy.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Linq;

namespace Fireasy.Composition
{
    /// <summary>
    /// 针对 MEF 提供的服务扩展类。
    /// </summary>
    public static class ServiceProviderExtensions
    {
        private static CompositionContainer _container = null;

        /// <summary>
        /// 获取可导出的服务实例。该服务是使用 <see cref="ExportAttribute"/> 进行标记的。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serviceProvider"></param>
        /// <param name="contractName"></param>
        /// <returns></returns>
        public static IEnumerable<T> GetExportedServices<T>(this IServiceProvider serviceProvider, string? contractName = null)
        {
            var definition = CreateImportDefinition<T>(contractName);

            var container = GetContainer(serviceProvider);
            foreach (var export in container.GetExports(definition))
            {
                if (export == null || export.Value == null)
                {
                    continue;
                }

                var service = (T)export.Value;
                if (service is IServiceProviderAccessor accessor) 
                {
                    accessor.ServiceProvider = serviceProvider;
                }

                yield return service;
            }
        }

        private static ImportDefinition CreateImportDefinition<T>(string? contractName = null)
        {
            //使用 GetExports<T> 方法会导出一个空引用的 Lazy 对象，故使用 ImportDefinition 来调用另一个方法
            return new ContractBasedImportDefinition(
                string.IsNullOrEmpty(contractName) ? AttributedModelServices.GetContractName(typeof(T)) : contractName,
                AttributedModelServices.GetTypeIdentity(typeof(T)),
                Enumerable.Empty<KeyValuePair<string, Type>>(),
                ImportCardinality.ZeroOrMore, false, true, CreationPolicy.Shared);
        }

        private static CompositionContainer GetContainer(IServiceProvider serviceProvider)
        {
            SingletonLocker.Lock(ref _container, () =>
            {
                var configurationUnity = serviceProvider.GetService<ConfigurationUnity>();

                var section = configurationUnity?.GetSection<ImportConfigurationSection>();

                //如果有配置，使用 ConfigurationCatalog，否则需要在所有程序集中查找（性能有影响）
                var catalog = section == null || section.Settings.Count == 0 ?
                    (ComposablePartCatalog)new AssemblyDirectoryCatalog() : new ConfigurationCatalog(section);
                return new CompositionContainer(catalog);
            });

            return _container;
        }
    }
}
