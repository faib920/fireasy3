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
    /// 提供对类成员进行拦截的方法。
    /// </summary>
    public interface IInterceptor
    {
        /// <summary>
        /// 使用上下文对象对当前的拦截器进行初始化。
        /// </summary>
        /// <param name="context">包含拦截定义的上下文。</param>
        void Initialize(InterceptContext context);

        /// <summary>
        /// 将自定义方法注入到当前的拦截点。
        /// </summary>
        /// <param name="info">拦截调用信息。</param>
        void Intercept(InterceptCallInfo info);
    }

    /// <summary>
    /// 提供对类成员进行拦截的异步方法。
    /// </summary>
    public interface IAsyncInterceptor
    {
        /// <summary>
        /// 使用上下文对象对当前的拦截器进行初始化。
        /// </summary>
        /// <param name="context">包含拦截定义的上下文。</param>
        ValueTask InitializeAsync(InterceptContext context);

        /// <summary>
        /// 将自定义方法注入到当前的拦截点。
        /// </summary>
        /// <param name="info">拦截调用信息。</param>
        ValueTask InterceptAsync(InterceptCallInfo info);
    }
}
