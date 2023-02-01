// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Fireasy.Common;
using Fireasy.Common.DependencyInjection;
using Fireasy.Data;
using Fireasy.Data.Provider;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// <see cref="SetupBuilder"/> 扩展类。
    /// </summary>
    public static class SetupBuilderExtensions
    {
        /// <summary>
        /// 配置 Fireasy.Data 模块相关服务。
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        public static SetupBuilder ConfigureData(this SetupBuilder builder, Action<DataSetupBuildOptions>? configure = null)
        {
            var customizer = new ProviderCustomizer();

            var accessor = builder.Services.AddObjectAccessor(customizer);

            //如果已经配置过，则使用以前的定制器
            if (accessor?.Value != null)
            {
                customizer = accessor.Value;
            }

            var options = new DataSetupBuildOptions(customizer);

            configure?.Invoke(options);

            if (options.DbCommandInterceptors.Count > 0)
            {
                builder.Services.Replace(ServiceDescriptor.Transient<IDbCommandInterceptor>(sp => new DbCommandInterceptors(options.DbCommandInterceptors)));
            }

            return builder;
        }
    }
}
