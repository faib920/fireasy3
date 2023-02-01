namespace Fireasy.Common.Reflection
{
    /// <summary>
    /// 
    /// </summary>
    public interface IConstructorInvoker
    {
        /// <summary>
        /// 使用指定的参数执行构造函数。
        /// </summary>
        /// <param name="parameters">构造函数的参数。</param>
        /// <returns></returns>
        object Invoke(params object[] parameters);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TArg1"></typeparam>
    public interface IConstructorInvoker<TArg1>
    {
        /// <summary>
        /// 使用指定的参数执行构造函数。
        /// </summary>
        /// <param name="arg1">参数1。</param>
        /// <returns></returns>
        object Invoke(TArg1 arg1);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TArg1"></typeparam>
    /// <typeparam name="TArg2"></typeparam>
    public interface IConstructorInvoker<TArg1, TArg2>
    {
        /// <summary>
        /// 使用指定的参数执行构造函数。
        /// </summary>
        /// <param name="arg1">参数1。</param>
        /// <param name="arg2">参数2。</param>
        /// <returns></returns>
        object Invoke(TArg1 arg1, TArg2 arg2);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TArg1"></typeparam>
    /// <typeparam name="TArg2"></typeparam>
    /// <typeparam name="TArg3"></typeparam>
    public interface IConstructorInvoker<TArg1, TArg2, TArg3>
    {
        /// <summary>
        /// 使用指定的参数执行构造函数。
        /// </summary>
        /// <param name="arg1">参数1。</param>
        /// <param name="arg2">参数2。</param>
        /// <param name="arg3">参数3。</param>
        /// <returns></returns>
        object Invoke(TArg1 arg1, TArg2 arg2, TArg3 arg3);
    }
}
