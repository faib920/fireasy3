// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using AutoMapper;
using Fireasy.AutoMapper;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// 
    /// </summary>
    public static class AutoMapperServiceCollectionExtensions
    {
        /// <summary>
        /// 添加 AutoMapper 对象映射组件。
        /// </summary>
        /// <param name="services"></param>
        /// <param name="setupAction"></param>
        /// <returns></returns>
        public static IServiceCollection AddAutoMapper(this IServiceCollection services, Action<AutoMapperOptions>? setupAction = null)
        {
            var options = new AutoMapperOptions();
            setupAction?.Invoke(options);

            var mapperConfiguration = new MapperConfiguration(c =>
            {
                options.Configurators!.ForEach(s => s?.Invoke(c));
            });

            options.Setup(services);

            services.AddSingleton(sp => mapperConfiguration.CreateMapper(t => sp.GetService(t)));
            services.AddSingleton<Fireasy.ObjectMapping.IObjectMapper, ObjectMapper>();

            return services;
        }
    }
}
