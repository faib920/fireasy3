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
    /// 用于标识此类型能够被自动发现并加入到容器中。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ServiceRegisterAttribute : Attribute
    {
        /// <summary>
        /// 初始化 <see cref="ServiceRegisterAttribute"/> 类的新实例。
        /// </summary>
        /// <param name="lifetime">对象的生命周期。</param>
        public ServiceRegisterAttribute(ServiceLifetime lifetime)
        {
            Lifetime = lifetime;
        }

        /// <summary>
        /// 获取生命周期。
        /// </summary>
        public ServiceLifetime Lifetime { get; }
    }
}