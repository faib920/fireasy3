// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Fireasy.Common.Reflection
{
    /// <summary>
    /// 方法的执行器。
    /// </summary>
    public interface IMethodInvoker
    {
        /// <summary>
        /// 使用指定的参数调用当前实例的方法。
        /// </summary>
        /// <param name="instance">实例对象。</param>
        /// <param name="parameters">方法的参数。</param>
        /// <returns></returns>
        object? Invoke(object? instance, params object[] parameters);
    }

    /// <summary>
    /// 方法的执行器。
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    public interface IMethodInvoker<out TValue>
    {
        /// <summary>
        /// 使用指定的参数调用当前实例的方法。
        /// </summary>
        /// <param name="instance">实例对象。</param>
        /// <param name="parameters">方法的参数。</param>
        /// <returns></returns>
        TValue? Invoke(object? instance, params object[] parameters);
    }

    /// <summary>
    /// 方法的执行器。
    /// </summary>
    /// <typeparam name="TArg1"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public interface IMethodInvoker<in TArg1, out TValue>
    {
        /// <summary>
        /// 使用指定的参数调用当前实例的方法。
        /// </summary>
        /// <param name="instance">实例对象。</param>
        /// <param name="arg1">参数1。</param>
        /// <returns></returns>
        TValue? Invoke(object instance, TArg1? arg1);
    }

    /// <summary>
    /// 方法的执行器。
    /// </summary>
    /// <typeparam name="TArg1"></typeparam>
    /// <typeparam name="TArg2"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public interface IMethodInvoker<in TArg1, in TArg2, out TValue>
    {
        /// <summary>
        /// 使用指定的参数调用当前实例的方法。
        /// </summary>
        /// <param name="instance">实例对象。</param>
        /// <param name="arg1">参数1。</param>
        /// <param name="arg2">参数2。</param>
        /// <returns></returns>
        TValue? Invoke(object? instance, TArg1? arg1, TArg2? arg2);
    }

    /// <summary>
    /// 方法的执行器。
    /// </summary>
    /// <typeparam name="TArg1"></typeparam>
    /// <typeparam name="TArg2"></typeparam>
    /// <typeparam name="TArg3"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public interface IMethodInvoker<in TArg1, in TArg2, in TArg3, out TValue>
    {
        /// <summary>
        /// 使用指定的参数调用当前实例的方法。
        /// </summary>
        /// <param name="instance">实例对象。</param>
        /// <param name="arg1">参数1。</param>
        /// <param name="arg2">参数2。</param>
        /// <param name="arg3">参数3。</param>
        /// <returns></returns>
        TValue? Invoke(object? instance, TArg1? arg1, TArg2? arg2, TArg3? arg3);
    }

    /// <summary>
    /// 异步方法的执行器。
    /// </summary>
    public interface IAsyncMethodInvoker
    {
        /// <summary>
        /// 使用指定的参数调用当前实例的方法。
        /// </summary>
        /// <param name="instance">实例对象。</param>
        /// <param name="parameters">方法的参数。</param>
        /// <returns></returns>
        Task<object?> InvokeAsync(object? instance, params object[] parameters);
    }

    /// <summary>
    /// 异步方法的执行器。
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    public interface IAsyncMethodInvoker<TValue>
    {
        /// <summary>
        /// 使用指定的参数调用当前实例的方法。
        /// </summary>
        /// <param name="instance">实例对象。</param>
        /// <param name="parameters">方法的参数。</param>
        /// <returns></returns>
        Task<TValue?> InvokeAsync(object? instance, params object[] parameters);
    }

    /// <summary>
    /// 异步方法的执行器。
    /// </summary>
    /// <typeparam name="TArg1"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public interface IAsyncMethodInvoker<in TArg1, TValue>
    {
        /// <summary>
        /// 使用指定的参数调用当前实例的方法。
        /// </summary>
        /// <param name="instance">实例对象。</param>
        /// <param name="arg1">参数1。</param>
        /// <returns></returns>
        Task<TValue?> InvokeAsync(object? instance, TArg1? arg1);
    }

    /// <summary>
    /// 异步方法的执行器。
    /// </summary>
    /// <typeparam name="TArg1"></typeparam>
    /// <typeparam name="TArg2"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public interface IAsyncMethodInvoker<in TArg1, in TArg2, TValue>
    {
        /// <summary>
        /// 使用指定的参数调用当前实例的方法。
        /// </summary>
        /// <param name="instance">实例对象。</param>
        /// <param name="arg1">参数1。</param>
        /// <param name="arg2">参数2。</param>
        /// <returns></returns>
        Task<TValue?> InvokeAsync(object? instance, TArg1? arg1, TArg2? arg2);
    }

    /// <summary>
    /// 异步方法的执行器。
    /// </summary>
    /// <typeparam name="TArg1"></typeparam>
    /// <typeparam name="TArg2"></typeparam>
    /// <typeparam name="TArg3"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public interface IAsyncMethodInvoker<in TArg1, in TArg2, in TArg3, TValue>
    {
        /// <summary>
        /// 使用指定的参数调用当前实例的方法。
        /// </summary>
        /// <param name="instance">实例对象。</param>
        /// <param name="arg1">参数1。</param>
        /// <param name="arg2">参数2。</param>
        /// <param name="arg3">参数3。</param>
        /// <returns></returns>
        Task<TValue?> InvokeAsync(object? instance, TArg1? arg1, TArg2? arg2, TArg3? arg3);
    }
}
