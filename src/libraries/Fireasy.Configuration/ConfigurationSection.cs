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
    /// 一个抽象类，表示配置节的信息。
    /// </summary>
    public abstract class ConfigurationSection : IConfigurationSection
    {
        /// <summary>
        /// 使用配置节点对当前配置进行初始化。
        /// </summary>
        /// <param name="context">对应的配置节点。</param>
        public abstract void Bind(BindingContext context);

        /// <summary>
        /// 返回绑定时是否记录异常。
        /// </summary>
        /// <param name="exp">异常实例。</param>
        /// <returns></returns>
        public virtual bool HasException(out Exception? exp)
        {
            exp = null;
            return false;
        }
    }
}