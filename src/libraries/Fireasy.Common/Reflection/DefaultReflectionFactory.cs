// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using System.Collections.Concurrent;
using System.Reflection;

namespace Fireasy.Common.Reflection
{
    /// <summary>
    /// 缺省的反射缓存管理器。
    /// </summary>
    public class DefaultReflectionFactory : IReflectionFactory
    {
        private readonly ConcurrentDictionary<Type, ConcurrentDictionary<FieldInfo, IFieldAccessor>> _fieldAccessors = new ConcurrentDictionary<Type, ConcurrentDictionary<FieldInfo, IFieldAccessor>>();
        private readonly ConcurrentDictionary<Type, ConcurrentDictionary<PropertyInfo, IPropertyAccessor>> _propertyAccessors = new ConcurrentDictionary<Type, ConcurrentDictionary<PropertyInfo, IPropertyAccessor>>();
        private readonly ConcurrentDictionary<Type, ConcurrentDictionary<MethodInfo, IMethodInvoker>> _methodInvokers = new ConcurrentDictionary<Type, ConcurrentDictionary<MethodInfo, IMethodInvoker>>();
        private readonly ConcurrentDictionary<Type, ConcurrentDictionary<ConstructorInfo, IConstructorInvoker>> _construtorInvokers = new ConcurrentDictionary<Type, ConcurrentDictionary<ConstructorInfo, IConstructorInvoker>>();

        /// <summary>
        /// 获取字段的访问器。
        /// </summary>
        /// <param name="field">字段。</param>
        /// <returns></returns>
        public IFieldAccessor GetAccessor(FieldInfo field)
        {
            Guard.ArgumentNull(field, nameof(field));

            var dict = _fieldAccessors.GetOrAdd(field.DeclaringType, k => new ConcurrentDictionary<FieldInfo, IFieldAccessor>());
            return dict.GetOrAdd(field, key => new FieldAccessor(field));
        }

        /// <summary>
        /// 获取字段的访问器。
        /// </summary>
        /// <param name="field">字段。</param>
        /// <returns></returns>
        public IFieldAccessor<TValue> GetAccessor<TValue>(FieldInfo field)
        {
            Guard.ArgumentNull(field, nameof(field));

            var dict = _fieldAccessors.GetOrAdd(field.DeclaringType, k => new ConcurrentDictionary<FieldInfo, IFieldAccessor>());
            return (IFieldAccessor<TValue>)dict.GetOrAdd(field, key => new FieldAccessor<TValue>(field));
        }

        /// <summary>
        /// 获取属性的访问器。
        /// </summary>
        /// <param name="property">属性。</param>
        /// <returns></returns>
        public IPropertyAccessor GetAccessor(PropertyInfo property)
        {
            Guard.ArgumentNull(property, nameof(property));

            var dict = _propertyAccessors.GetOrAdd(property.DeclaringType, k => new ConcurrentDictionary<PropertyInfo, IPropertyAccessor>());
            return dict.GetOrAdd(property, key => new PropertyAccessor(key));
        }

        /// <summary>
        /// 获取属性的访问器。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="property"></param>
        /// <returns></returns>
        public IPropertyAccessor<T> GetAccessor<T>(PropertyInfo property)
        {
            Guard.ArgumentNull(property, nameof(property));

            var dict = _propertyAccessors.GetOrAdd(property.DeclaringType, k => new ConcurrentDictionary<PropertyInfo, IPropertyAccessor>());
            return (IPropertyAccessor<T>)dict.GetOrAdd(property, key => new PropertyAccessor<T>(key));
        }

        /// <summary>
        /// 获取方法的执行器。
        /// </summary>
        /// <param name="method">方法。</param>
        /// <returns></returns>
        public IMethodInvoker GetInvoker(MethodInfo method)
        {
            Guard.ArgumentNull(method, nameof(method));

            var dict = _methodInvokers.GetOrAdd(method.DeclaringType, k => new ConcurrentDictionary<MethodInfo, IMethodInvoker>());
            return dict.GetOrAdd(method, key => new MethodInvoker(key));
        }

        /// <summary>
        /// 获取方法的执行器。
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="method">方法。</param>
        /// <returns></returns>
        public IMethodInvoker<TValue> GetInvoker<TValue>(MethodInfo method)
        {
            Guard.ArgumentNull(method, nameof(method));

            var dict = _methodInvokers.GetOrAdd(method.DeclaringType, k => new ConcurrentDictionary<MethodInfo, IMethodInvoker>());
            return (IMethodInvoker<TValue>)dict.GetOrAdd(method, key => new MethodInvoker<TValue>(key));
        }

        /// <summary>
        /// 获取方法的执行器。
        /// </summary>
        /// <typeparam name="TArg1"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="method">方法。</param>
        /// <returns></returns>
        public IMethodInvoker<TArg1, TValue> GetInvoker<TArg1, TValue>(MethodInfo method)
        {
            Guard.ArgumentNull(method, nameof(method));

            var dict = _methodInvokers.GetOrAdd(method.DeclaringType, k => new ConcurrentDictionary<MethodInfo, IMethodInvoker>());
            return (IMethodInvoker<TArg1, TValue>)dict.GetOrAdd(method, key => new MethodInvoker<TArg1, TValue>(key));
        }

        /// <summary>
        /// 获取方法的执行器。
        /// </summary>
        /// <typeparam name="TArg1"></typeparam>
        /// <typeparam name="TArg2"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="method">方法。</param>
        /// <returns></returns>
        public IMethodInvoker<TArg1, TArg2, TValue> GetInvoker<TArg1, TArg2, TValue>(MethodInfo method)
        {
            Guard.ArgumentNull(method, nameof(method));

            var dict = _methodInvokers.GetOrAdd(method.DeclaringType, k => new ConcurrentDictionary<MethodInfo, IMethodInvoker>());
            return (IMethodInvoker<TArg1, TArg2, TValue>)dict.GetOrAdd(method, key => new MethodInvoker<TArg1, TArg2, TValue>(key));
        }

        /// <summary>
        /// 获取方法的执行器。
        /// </summary>
        /// <typeparam name="TArg1"></typeparam>
        /// <typeparam name="TArg2"></typeparam>
        /// <typeparam name="TArg3"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="method">方法。</param>
        /// <returns></returns>
        public IMethodInvoker<TArg1, TArg2, TArg3, TValue> GetInvoker<TArg1, TArg2, TArg3, TValue>(MethodInfo method)
        {
            Guard.ArgumentNull(method, nameof(method));

            var dict = _methodInvokers.GetOrAdd(method.DeclaringType, k => new ConcurrentDictionary<MethodInfo, IMethodInvoker>());
            return (IMethodInvoker<TArg1, TArg2, TArg3, TValue>)dict.GetOrAdd(method, key => new MethodInvoker(key));
        }

        /// <summary>
        /// 获取方法的执行器。
        /// </summary>
        /// <param name="method">方法。</param>
        /// <returns></returns>
        public IAsyncMethodInvoker GetAsyncInvoker(MethodInfo method)
        {
            Guard.ArgumentNull(method, nameof(method));

            var dict = _methodInvokers.GetOrAdd(method.DeclaringType, k => new ConcurrentDictionary<MethodInfo, IMethodInvoker>());
            return (IAsyncMethodInvoker)dict.GetOrAdd(method, key => new AsyncMethodInvoker(key));
        }

        /// <summary>
        /// 获取方法的执行器。
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="method">方法。</param>
        /// <returns></returns>
        public IAsyncMethodInvoker<TValue> GetAsyncInvoker<TValue>(MethodInfo method)
        {
            Guard.ArgumentNull(method, nameof(method));

            var dict = _methodInvokers.GetOrAdd(method.DeclaringType, k => new ConcurrentDictionary<MethodInfo, IMethodInvoker>());
            return (IAsyncMethodInvoker<TValue>)dict.GetOrAdd(method, key => new AsyncMethodInvoker<TValue>(key));
        }

        /// <summary>
        /// 获取方法的执行器。
        /// </summary>
        /// <typeparam name="TArg1"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="method">方法。</param>
        /// <returns></returns>
        public IAsyncMethodInvoker<TArg1, TValue> GetAsyncInvoker<TArg1, TValue>(MethodInfo method)
        {
            Guard.ArgumentNull(method, nameof(method));

            var dict = _methodInvokers.GetOrAdd(method.DeclaringType, k => new ConcurrentDictionary<MethodInfo, IMethodInvoker>());
            return (IAsyncMethodInvoker<TArg1, TValue>)dict.GetOrAdd(method, key => new AsyncMethodInvoker<TArg1, TValue>(key));
        }

        /// <summary>
        /// 获取方法的执行器。
        /// </summary>
        /// <typeparam name="TArg1"></typeparam>
        /// <typeparam name="TArg2"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="method">方法。</param>
        /// <returns></returns>
        public IAsyncMethodInvoker<TArg1, TArg2, TValue> GetAsyncInvoker<TArg1, TArg2, TValue>(MethodInfo method)
        {
            Guard.ArgumentNull(method, nameof(method));

            var dict = _methodInvokers.GetOrAdd(method.DeclaringType, k => new ConcurrentDictionary<MethodInfo, IMethodInvoker>());
            return (IAsyncMethodInvoker<TArg1, TArg2, TValue>)dict.GetOrAdd(method, key => new AsyncMethodInvoker<TArg1, TArg2, TValue>(key));
        }

        /// <summary>
        /// 获取方法的执行器。
        /// </summary>
        /// <typeparam name="TArg1"></typeparam>
        /// <typeparam name="TArg2"></typeparam>
        /// <typeparam name="TArg3"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="method">方法。</param>
        /// <returns></returns>
        public IAsyncMethodInvoker<TArg1, TArg2, TArg3, TValue> GetAsyncInvoker<TArg1, TArg2, TArg3, TValue>(MethodInfo method)
        {
            Guard.ArgumentNull(method, nameof(method));

            var dict = _methodInvokers.GetOrAdd(method.DeclaringType, k => new ConcurrentDictionary<MethodInfo, IMethodInvoker>());
            return (IAsyncMethodInvoker<TArg1, TArg2, TArg3, TValue>)dict.GetOrAdd(method, key => new AsyncMethodInvoker<TArg1, TArg2, TArg3, TValue>(key));
        }

        /// <summary>
        /// 获取构造函数的执行器。
        /// </summary>
        /// <param name="constructor">构造函数。</param>
        /// <returns></returns>
        public IConstructorInvoker GetInvoker(ConstructorInfo constructor)
        {
            Guard.ArgumentNull(constructor, nameof(constructor));

            var dict = _construtorInvokers.GetOrAdd(constructor.DeclaringType, k => new ConcurrentDictionary<ConstructorInfo, IConstructorInvoker>());
            return dict.GetOrAdd(constructor, key => new ConstructorInvoker(key));
        }


        /// <summary>
        /// 获取构造函数的执行器。
        /// </summary>
        /// <param name="constructor">构造函数。</param>
        /// <returns></returns>
        public IConstructorInvoker<TArg1> GetInvoker<TArg1>(ConstructorInfo constructor)
        {
            Guard.ArgumentNull(constructor, nameof(constructor));

            var dict = _construtorInvokers.GetOrAdd(constructor.DeclaringType, k => new ConcurrentDictionary<ConstructorInfo, IConstructorInvoker>());
            return (IConstructorInvoker<TArg1>)dict.GetOrAdd(constructor, key => new ConstructorInvoker<TArg1>(key));
        }

        /// <summary>
        /// 获取构造函数的执行器。
        /// </summary>
        /// <param name="constructor">构造函数。</param>
        /// <returns></returns>
        public IConstructorInvoker<TArg1, TArg2> GetInvoker<TArg1, TArg2>(ConstructorInfo constructor)
        {
            Guard.ArgumentNull(constructor, nameof(constructor));

            var dict = _construtorInvokers.GetOrAdd(constructor.DeclaringType, k => new ConcurrentDictionary<ConstructorInfo, IConstructorInvoker>());
            return (IConstructorInvoker<TArg1, TArg2>)dict.GetOrAdd(constructor, key => new ConstructorInvoker<TArg1, TArg2>(key));
        }

        /// <summary>
        /// 获取构造函数的执行器。
        /// </summary>
        /// <param name="constructor">构造函数。</param>
        /// <returns></returns>
        public IConstructorInvoker<TArg1, TArg2, TArg3> GetInvoker<TArg1, TArg2, TArg3>(ConstructorInfo constructor)
        {
            Guard.ArgumentNull(constructor, nameof(constructor));

            var dict = _construtorInvokers.GetOrAdd(constructor.DeclaringType, k => new ConcurrentDictionary<ConstructorInfo, IConstructorInvoker>());
            return (IConstructorInvoker<TArg1, TArg2, TArg3>)dict.GetOrAdd(constructor, key => new ConstructorInvoker<TArg1, TArg2, TArg3>(key));
        }

    }
}
