// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Microsoft.Extensions.DependencyInjection;

namespace Fireasy.Common
{
    /// <summary>
    /// 配置构造器。
    /// </summary>
    public class SetupBuilder
    {
        /// <summary>
        /// 初始化 <see cref="SetupBuilder"/> 类的新实例。
        /// </summary>
        /// <param name="services"></param>
        /// <param name="options"></param>
        public SetupBuilder(IServiceCollection services, SetupOptions options)
        {
            Services = services;
            Options = options;
        }

        /// <summary>
        /// 获取 <see cref="IServiceCollection"/> 实例。
        /// </summary>
        public IServiceCollection Services { get; }

        /// <summary>
        /// 获取 <see cref="SetupOptions"/> 实例。
        /// </summary>
        public SetupOptions Options { get; }
    }
}
