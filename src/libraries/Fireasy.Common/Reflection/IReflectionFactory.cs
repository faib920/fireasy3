// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using System.Reflection;

namespace Fireasy.Common.Reflection
{
    /// <summary>
    /// 提供基于缓存的反射工厂。
    /// </summary>
    public interface IReflectionFactory
    {
        /// <summary>
        /// 获取字段的访问器。
        /// </summary>
        /// <param name="field">要访问的字段。</param>
        /// <returns></returns>
        IFieldAccessor GetAccessor(FieldInfo field);

        /// <summary>
        /// 获取字段的访问器。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field">要访问的字段。</param>
        /// <returns></returns>
        IFieldAccessor<T> GetAccessor<T>(FieldInfo field);

        /// <summary>
        /// 获取属性的访问器。
        /// </summary>
        /// <param name="property">要访问的属性。</param>
        /// <returns></returns>
        IPropertyAccessor GetAccessor(PropertyInfo property);

        /// <summary>
        /// 获取属性的访问器。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="property">要访问的属性。</param>
        /// <returns></returns>
        IPropertyAccessor<T> GetAccessor<T>(PropertyInfo property);

        /// <summary>
        /// 获取方法的执行器。
        /// </summary>
        /// <param name="method">方法。</param>
        /// <returns></returns>
        IMethodInvoker GetInvoker(MethodInfo method);

        /// <summary>
        /// 获取方法的执行器。
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="method">方法。</param>
        /// <returns></returns>
        IMethodInvoker<TValue> GetInvoker<TValue>(MethodInfo method);

        /// <summary>
        /// 获取方法的执行器。
        /// </summary>
        /// <typeparam name="TArg1"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="method">方法。</param>
        /// <returns></returns>
        IMethodInvoker<TArg1, TValue> GetInvoker<TArg1, TValue>(MethodInfo method);

        /// <summary>
        /// 获取方法的执行器。
        /// </summary>
        /// <typeparam name="TArg1"></typeparam>
        /// <typeparam name="TArg2"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="method">方法。</param>
        /// <returns></returns>
        IMethodInvoker<TArg1, TArg2, TValue> GetInvoker<TArg1, TArg2, TValue>(MethodInfo method);

        /// <summary>
        /// 获取方法的执行器。
        /// </summary>
        /// <typeparam name="TArg1"></typeparam>
        /// <typeparam name="TArg2"></typeparam>
        /// <typeparam name="TArg3"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="method">方法。</param>
        /// <returns></returns>
        IMethodInvoker<TArg1, TArg2, TArg3, TValue> GetInvoker<TArg1, TArg2, TArg3, TValue>(MethodInfo method);

        /// <summary>
        /// 获取方法的执行器。
        /// </summary>
        /// <param name="method">方法。</param>
        /// <returns></returns>
        IAsyncMethodInvoker GetAsyncInvoker(MethodInfo method);

        /// <summary>
        /// 获取方法的执行器。
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="method">方法。</param>
        /// <returns></returns>
        IAsyncMethodInvoker<TValue> GetAsyncInvoker<TValue>(MethodInfo method);

        /// <summary>
        /// 获取方法的执行器。
        /// </summary>
        /// <typeparam name="TArg1"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="method">方法。</param>
        /// <returns></returns>
        IAsyncMethodInvoker<TArg1, TValue> GetAsyncInvoker<TArg1, TValue>(MethodInfo method);

        /// <summary>
        /// 获取方法的执行器。
        /// </summary>
        /// <typeparam name="TArg1"></typeparam>
        /// <typeparam name="TArg2"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="method">方法。</param>
        /// <returns></returns>
        IAsyncMethodInvoker<TArg1, TArg2, TValue> GetAsyncInvoker<TArg1, TArg2, TValue>(MethodInfo method);

        /// <summary>
        /// 获取方法的执行器。
        /// </summary>
        /// <typeparam name="TArg1"></typeparam>
        /// <typeparam name="TArg2"></typeparam>
        /// <typeparam name="TArg3"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="method">方法。</param>
        /// <returns></returns>
        IAsyncMethodInvoker<TArg1, TArg2, TArg3, TValue> GetAsyncInvoker<TArg1, TArg2, TArg3, TValue>(MethodInfo method);

        /// <summary>
        /// 获取构造函数的执行器。
        /// </summary>
        /// <param name="constructor">构造函数。</param>
        /// <returns></returns>
        IConstructorInvoker GetInvoker(ConstructorInfo constructor);

        /// <summary>
        /// 获取构造函数的执行器。
        /// </summary>
        /// <param name="constructor">构造函数。</param>
        /// <returns></returns>
        IConstructorInvoker<TArg1> GetInvoker<TArg1>(ConstructorInfo constructor);

        /// <summary>
        /// 获取构造函数的执行器。
        /// </summary>
        /// <param name="constructor">构造函数。</param>
        /// <returns></returns>
        IConstructorInvoker<TArg1, TArg2> GetInvoker<TArg1, TArg2>(ConstructorInfo constructor);

        /// <summary>
        /// 获取构造函数的执行器。
        /// </summary>
        /// <param name="constructor">构造函数。</param>
        /// <returns></returns>
        IConstructorInvoker<TArg1, TArg2, TArg3> GetInvoker<TArg1, TArg2, TArg3>(ConstructorInfo constructor);
    }
}
