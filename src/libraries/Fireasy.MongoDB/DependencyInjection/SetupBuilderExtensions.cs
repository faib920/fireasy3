// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Fireasy.Common;
using Fireasy.Data;

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
        /// <returns></returns>
        public static SetupBuilder UseMongoDB(this SetupBuilder builder)
        {
            return builder;
        }
    }
}