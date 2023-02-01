// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Fireasy.Common.DependencyInjection;
using Fireasy.Common.DependencyInjection.Filters;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Fireasy.Common
{
    /// <summary>
    /// 服务发现选项。无法继承此类。
    /// </summary>
    public sealed class DiscoverOptions
    {
        /// <summary>
        /// 初始化 <see cref="DiscoverOptions"/> 类的新实例。
        /// </summary>
        public DiscoverOptions() { }

        /// <summary>
        /// 获取或设置 <see cref="IServiceDiscoverer"/> 的创建工厂。
        /// </summary>
        public Func<IServiceCollection, DiscoverOptions, IServiceDiscoverer> DiscovererFactory { get; set; }

        /// <summary>
        /// 获取程序集过滤器列表。
        /// </summary>
        public List<IAssemblyFilter> AssemblyFilters { get; } = new() { new DefaultAssemblyFilter() };

        /// <summary>
        /// 获取程序集过滤判断式列表。
        /// </summary>
        public List<Func<Assembly, bool>> AssemblyFilterPredicates { get; } = new();

        /// <summary>
        /// 获取类型过滤器列表。
        /// </summary>
        public List<ITypeFilter> TypeFilters { get; } = new() { new DefaultTypeFilter() };

        /// <summary>
        /// 获取类型过滤判断式列表。
        /// </summary>
        public List<Func<Assembly, Type, bool>> TypeFilterPredicates { get; } = new();
    }
}
