// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Fireasy.Common.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System.Data.Common;

namespace Fireasy.Data.Provider
{
    /// <summary>
    /// 从注入的定制器中发现。
    /// </summary>
    /// <typeparam name="TProvider"></typeparam>
    public class InjectedProviderFactoryResolver<TProvider> : IProviderFactoryResolver where TProvider : IProvider
    {
        private readonly ProviderCustomizer? _customizer;

        /// <summary>
        /// 初始化 <see cref="InjectedProviderFactoryResolver{TProvider}"/> 类的新实例。
        /// </summary>
        /// <param name="serviceProvider"></param>
        public InjectedProviderFactoryResolver(IServiceProvider serviceProvider)
        {
            var accessor = serviceProvider.GetService<IObjectAccessor<ProviderCustomizer>>();
            _customizer = accessor?.Value;
        }

        bool IProviderFactoryResolver.TryResolve(out DbProviderFactory? factory)
        {
            factory = _customizer?.GetDbProviderFactory(typeof(TProvider));
            if (factory != null)
            {
                return factory != null;
            }

            factory = null;

            return false;
        }
    }
}
