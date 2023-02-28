// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Fireasy.Common.DynamicProxy
{
    /// <summary>
    /// 表示一个属性或方法使用指定的拦截器注入一段执行代码。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = true)]
    public class InterceptAttribute : Attribute
    {
        /// <summary>
        /// 初始化 <see cref="InterceptAttribute"/> 类的新实例。
        /// </summary>
        /// <param name="interceptorType">拦截器的类型。</param>
        public InterceptAttribute(Type interceptorType)
        {
            InterceptorType = interceptorType;
        }

        /// <summary>
        /// 获取或设置拦截器的类型。
        /// </summary>
        public Type InterceptorType { get; set; }
    }
}
