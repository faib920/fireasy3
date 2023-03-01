// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Fireasy.Common.Analyzers.DynamicProxy.Metadata
{
    /// <summary>
    /// 拦截器的元数据。
    /// </summary>
    public class InterceptorMetadata
    {
        /// <summary>
        /// 初始化 <see cref="InterceptorMetadata"/> 类的新实例。
        /// </summary>
        /// <param name="interceptorTypes">拦截器的类型符号列表。</param>
        /// <param name="throwExpcetion">是否抛出异常。</param>
        public InterceptorMetadata(List<ITypeSymbol> interceptorTypes, bool throwExpcetion)
        {
            InterceptorTypes = interceptorTypes;
            ThrowExpcetion = throwExpcetion;
        }

        /// <summary>
        /// 获取拦截器的类型符号。
        /// </summary>
        public List<ITypeSymbol> InterceptorTypes { get; }

        /// <summary>
        /// 获取拦截时是否抛出异常。
        /// </summary>
        public bool ThrowExpcetion { get; }

        /// <summary>
        /// 克隆。
        /// </summary>
        /// <param name="throwException">是否抛出异常。</param>
        /// <returns></returns>
        public InterceptorMetadata Clone(bool? throwException = null)
        {
            return new InterceptorMetadata(InterceptorTypes, throwException ?? ThrowExpcetion);
        }
    }
}
