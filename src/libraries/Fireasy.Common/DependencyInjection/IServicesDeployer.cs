// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Microsoft.Extensions.DependencyInjection;

namespace Fireasy.Common.DependencyInjection
{
    /// <summary>
    /// 用于部署服务。
    /// </summary>
    public interface IServicesDeployer
    {
        /// <summary>
        /// 配置 <see cref="IServiceCollection"/> 实例。
        /// </summary>
        /// <param name="services"></param>
        void Configure(IServiceCollection services);
    }
}
