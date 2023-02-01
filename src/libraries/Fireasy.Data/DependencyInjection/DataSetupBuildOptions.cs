// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Fireasy.Data.Provider;
using System.Collections.Generic;
using System.Data.Common;

namespace Fireasy.Data
{
    /// <summary>
    /// Fireasy.Data 的配置参数。
    /// </summary>
    public class DataSetupBuildOptions
    {
        private readonly ProviderCustomizer _customizer;

        internal DataSetupBuildOptions(ProviderCustomizer customizer)
        {
            _customizer = customizer;
        }

        /// <summary>
        /// 获取或设置命令拦截器列表。
        /// </summary>
        public List<IDbCommandInterceptor> DbCommandInterceptors { get; set; } = new List<IDbCommandInterceptor>();

        /// <summary>
        /// 添加数据库提供者。
        /// </summary>
        /// <typeparam name="TProvider"></typeparam>
        /// <param name="providerName"></param>
        /// <returns></returns>
        public DataSetupBuildOptions AddProvider<TProvider>(string providerName) where TProvider : IProvider
        {
            _customizer.AddProvider<TProvider>(providerName);
            return this;
        }

        /// <summary>
        /// 指定数据库提供者所使用的 <see cref="DbProviderFactory"/> 类名。
        /// </summary>
        /// <typeparam name="TProvider"></typeparam>
        /// <typeparam name="TFactory"><see cref="DbProviderFactory"/> 的实现类。</typeparam>
        /// <returns></returns>
        public DataSetupBuildOptions AddProivderFactory<TProvider, TFactory>() 
            where TProvider : IProvider
            where TFactory : DbProviderFactory
        {
            _customizer.AddProivderFactory<TProvider, TFactory>();
            return this;
        }

        /// <summary>
        /// 指定数据库提供者所使用的 <see cref="DbProviderFactory"/> 实例。
        /// </summary>
        /// <typeparam name="TProvider"></typeparam>
        /// <returns></returns>
        public DataSetupBuildOptions AddProivderFactory<TProvider>(DbProviderFactory factory)
            where TProvider : IProvider
        {
            _customizer.AddProivderFactory<TProvider>(factory);
            return this;
        }
    }
}
