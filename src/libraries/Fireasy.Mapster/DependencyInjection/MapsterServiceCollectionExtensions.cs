// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Fireasy.Mapster;
using Fireasy.ObjectMapping;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// 
    /// </summary>
    public static class MapsterServiceCollectionExtensions
    {
        /// <summary>
        /// 添加 Mapster 对象映射组件。
        /// </summary>
        /// <param name="services"></param>
        /// <param name="setupAction"></param>
        /// <returns></returns>
        public static IServiceCollection AddMapster(this IServiceCollection services, Action<MapsterOptions>? setupAction = null)
        {
            var options = new MapsterOptions();
            setupAction?.Invoke(options);

            services.AddSingleton<IObjectMapper, ObjectMapper>();

            return services;
        }
    }
}