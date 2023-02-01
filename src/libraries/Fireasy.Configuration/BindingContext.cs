// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Microsoft.Extensions.Configuration;
using System;

namespace Fireasy.Configuration
{
    /// <summary>
    /// 配置绑定上下文。
    /// </summary>
    public sealed class BindingContext
    {
        /// <summary>
        /// 初始化 <see cref="BindingContext"/> 类的新实例。
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <param name="configuration"><see cref="IConfiguration"/> 对象。</param>
        public BindingContext(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            ServiceProvider = serviceProvider;
            Configuration = configuration;
        }

        /// <summary>
        /// 
        /// </summary>
        public IServiceProvider ServiceProvider { get; }

        /// <summary>
        /// 返回 <see cref="IConfiguration"/> 对象。
        /// </summary>
        public IConfiguration Configuration { get; }
    }
}
