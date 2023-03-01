// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Fireasy.Common.DependencyInjection
{
    /// <summary>
    /// 指定程序集中部署服务的 <see cref="IServicesDeployer"/> 实现。
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public sealed class ServicesDeployAttribute : Attribute
    {
        /// <summary>
        /// 初始化 <see cref="ServicesDeployAttribute"/> 类的新实例。
        /// </summary>
        /// <param name="deployType"><see cref="IServicesDeployer"/> 的实现类。</param>
        public ServicesDeployAttribute(Type deployType)
        {
            Type = deployType;
        }

        /// <summary>
        /// <see cref="IServicesDeployer"/> 的实现类。
        /// </summary>
        public Type Type { get; }

        /// <summary>
        /// 获取或设置优先级，0 为最高优先级。
        /// </summary>
        public int Priority { get; set; }
    }
}
