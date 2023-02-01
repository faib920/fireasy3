// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace Fireasy.Configuration
{
    /// <summary>
    /// 提供配置节的方法。
    /// </summary>
    public interface IConfigurationSection
    {
        /// <summary>
        /// 使用配置节点对当前配置进行初始化。
        /// </summary>
        /// <param name="context">配置上下文对象。</param>
        void Bind(BindingContext context);

        /// <summary>
        /// 返回绑定时是否记录异常。
        /// </summary>
        /// <param name="exp">异常实例。</param>
        /// <returns></returns>
        bool HasException(out Exception? exp);
    }
}