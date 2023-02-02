// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Fireasy.Configuration.DependencyInjection
{
    /// <summary>
    /// 提供用于获取 <see cref="IConfiguration"/> 的接口。
    /// </summary>
    public interface IConfigurationAccessor
    {
        /// <summary>
        /// 获取 <see cref="IConfiguration"/> 实例。
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        IConfiguration? GetConfiguration(IServiceCollection services);
    }
}
