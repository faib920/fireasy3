// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Fireasy.Composition.Configuration;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using System.Reflection;

namespace Fireasy.Composition
{
    /// <summary>
    /// 基于配置的对象组合部件目录。
    /// </summary>
    public class ConfigurationCatalog : ComposablePartCatalog
    {
        private IQueryable<ComposablePartDefinition> _partsQuery;
        private List<string> _files = null;
        protected readonly ImportConfigurationSection _configSection;

        /// <summary>
        /// 初始化 <see cref="ConfigurationCatalog"/> 类的新实例。
        /// </summary>
        /// <param name="configSection"></param>
        public ConfigurationCatalog(ImportConfigurationSection configSection)
        {
            _configSection = configSection;
        }

        /// <summary>
        /// 获取目录中包含的部件定义。
        /// </summary>
        public override IQueryable<ComposablePartDefinition> Parts
        {
            get
            {
                return _partsQuery ?? (_partsQuery = CreateDefinitions());
            }
        }

        /// <summary>
        /// 返回加载的程序集文件。
        /// </summary>
        public ReadOnlyCollection<string> LoadedFiles
        {
            get
            {
                if (_files == null)
                {
                    _partsQuery = CreateDefinitions();
                }

                return _files.AsReadOnly();
            }
        }

        /// <summary>
        /// 从 <see cref="ImportConfigurationSetting"/> 对象中解析出 <see cref="ComposablePartCatalog"/> 对象。
        /// </summary>
        /// <param name="setting">用于配置目录的配置对象。</param>
        /// <returns>从 <paramref name="setting"/> 解析出的 <see cref="ComposablePartCatalog"/> 对象。</returns>
        protected ComposablePartCatalog? ResolveCatalog(ImportConfigurationSetting? setting)
        {
            if (setting == null)
            {
                return null;
            }

            if (!string.IsNullOrEmpty(setting.Assembly))
            {
                var assembly = Assembly.Load(new AssemblyName(setting.Assembly));
                _files.Add(assembly.Location);
                return new AssemblyCatalog(assembly);
            }

            return setting.ImportType == null || setting.ContractType == null
                       ? null : new TypeCatalog(setting.ImportType);
        }

        /// <summary>
        /// 通过配置节创建 <see cref="ComposablePartDefinition"/> 序列。
        /// </summary>
        /// <returns></returns>
        protected virtual IQueryable<ComposablePartDefinition>? CreateDefinitions()
        {
            var list = _configSection.Settings
                .Select(setting => ResolveCatalog(setting.Value))
                .Where(catalog => catalog != null).ToList();

            return list.SelectMany(s => s.Parts).AsQueryable();
        }
    }

    /// <summary>
    /// 基于配置的对象组合部件目录。
    /// </summary>
    /// <typeparam name="T">要导入的协定类型。</typeparam>
    public sealed class ConfigurationCatalog<T> : ConfigurationCatalog
    {
        /// <summary>
        /// 初始化 <see cref="ConfigurationCatalog{T}"/> 类的新实例。
        /// </summary>
        /// <param name="configSection"></param>
        public ConfigurationCatalog(ImportConfigurationSection configSection)
            : base(configSection)
        {
        }

        /// <summary>
        /// 通过配置节创建 <see cref="ComposablePartDefinition"/> 序列。
        /// </summary>
        /// <returns></returns>
        protected override IQueryable<ComposablePartDefinition>? CreateDefinitions()
        {
            var contractType = typeof(T);
            var list = _configSection.Settings
                .Where(s => s.Value?.ContractType == contractType)
                .Select(setting => ResolveCatalog(setting.Value))
                .Where(catalog => catalog != null).ToList();

            return list.SelectMany(s => s.Parts).AsQueryable();
        }
    }
}
