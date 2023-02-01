// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Fireasy.Data.Configuration;
using Fireasy.Data.Provider;

namespace Fireasy.Data
{
    /// <summary>
    /// 提供 <see cref="IProviderManager"/> 的扩展方法。
    /// </summary>
    public static class ProviderManagerExtensions
    {
        /// <summary>
        /// 根据 <paramref name="setting"/> 获取对应的 <see cref="IProvider"/> 对象。
        /// </summary>
        /// <param name="providerManager"><see cref="IProvider"/> 管理器。</param>
        /// <param name="setting">数据库实例配置。</param>
        /// <returns></returns>
        public static IProvider? GetDefinedProvider(this IProviderManager providerManager, IInstanceConfigurationSetting setting)
        {
            var provider = providerManager.GetDefinedProviderInstance(setting.ProviderType);
            if (provider is IFeaturedProvider featureProvider)
            {
                var feature = featureProvider.GetFeature(setting.ConnectionString);
                if (!string.IsNullOrEmpty(feature))
                {
                    return featureProvider.Clone(feature!);
                }
            }

            return provider;
        }
    }
}
