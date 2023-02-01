// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Fireasy.Common.Emit;
using Fireasy.Common.Extensions;
using System.Reflection;
using System.Reflection.Emit;

namespace Fireasy.Common.Reflection
{
    /// <summary>
    /// 包装 <see cref="PropertyInfo"/> 对象，创建一个委托来提升属性的读写。
    /// </summary>
    public class PropertyAccessor : IPropertyAccessor
    {
        private readonly Func<object?, object?>? _getter;
        private readonly Action<object?, object?>? _setter;

        /// <summary>
        /// 初始化 <see cref="PropertyAccessor"/> 类的新实例。
        /// </summary>
        /// <param name="propertyInfo">要包装的 <see cref="PropertyInfo"/> 对象。</param>
        public PropertyAccessor(PropertyInfo propertyInfo)
        {
            Guard.ArgumentNull(propertyInfo, nameof(propertyInfo));

            _getter = CreateGetterDelegate(propertyInfo);
            _setter = CreateSetterDelegate(propertyInfo);
        }

        private Func<object?, object?>? CreateGetterDelegate(PropertyInfo propertyInfo)
        {
            if (!propertyInfo.CanRead)
            {
                return null;
            }

            var getMethod = propertyInfo.GetGetMethod(true);

            var dm = new DynamicMethod($"{propertyInfo.Name}_Getter", typeof(object),
                new Type[] { typeof(object) }, propertyInfo.DeclaringType, true);

            var emiter = new EmitHelper(dm.GetILGenerator());
            emiter.If(!getMethod.IsStatic, e => e.ldarg_0.end())
                .If(getMethod.IsStatic || propertyInfo.DeclaringType.IsValueType, e => e.callvirt(getMethod), e => e.call(getMethod))
                .If(propertyInfo.PropertyType.IsValueType, e => e.box(propertyInfo.PropertyType))
                .ret();

            return (Func<object?, object?>)dm.CreateDelegate(typeof(Func<object, object>));
        }

        private Action<object?, object?>? CreateSetterDelegate(PropertyInfo propertyInfo)
        {
            if (!propertyInfo.CanWrite)
            {
                return null;
            }

            var setMethod = propertyInfo.GetSetMethod(true);
            var dm = new DynamicMethod($"{propertyInfo.Name}_Setter", null,
                new Type[] { typeof(object), typeof(object) }, propertyInfo.DeclaringType, true);

            var emiter = new EmitHelper(dm.GetILGenerator());
            emiter.If(!setMethod.IsStatic, e => e.ldarg_0.end())
                .ldarg_1
                .If(propertyInfo.PropertyType.IsNullableType(), e => e.newobj(propertyInfo.PropertyType, propertyInfo.PropertyType.GetNonNullableType()))
                .If(propertyInfo.PropertyType.IsValueType, e => e.unbox_any(propertyInfo.PropertyType), e => e.castclass(propertyInfo.PropertyType))
                .If(setMethod.IsStatic || propertyInfo.DeclaringType.IsValueType, e => e.call(setMethod), e => e.callvirt(setMethod))
                .ret();

            return (Action<object?, object?>)dm.CreateDelegate(typeof(Action<object, object>));
        }

        /// <summary>
        /// 获取给定对象的属性的值。
        /// </summary>
        /// <param name="instance">实例对象。</param>
        /// <returns></returns>
        public object? GetValue(object? instance)
        {
            if (_getter == null)
            {
                throw new NotSupportedException("该访问器不提供读取操作。");
            }

            return _getter(instance);
        }

        /// <summary>
        /// 设置给定对象的属性的值。
        /// </summary>
        /// <param name="instance">实例对象。</param>
        /// <param name="value">属性的值。</param>
        public void SetValue(object? instance, object? value)
        {
            if (_setter == null)
            {
                throw new NotSupportedException("该访问器不提供赋值操作。");
            }

            _setter(instance, value);
        }
    }

    /// <summary>
    /// 包装 <see cref="PropertyInfo"/> 对象，创建一个委托来提升属性的读写。
    /// </summary>
    public class PropertyAccessor<TValue> : IPropertyAccessor<TValue>, IPropertyAccessor
    {
        private readonly Func<object?, TValue?>? _getter;
        private readonly Action<object?, TValue?>? _setter;

        /// <summary>
        /// 初始化 <see cref="PropertyAccessor"/> 类的新实例。
        /// </summary>
        /// <param name="propertyInfo">要包装的 <see cref="PropertyInfo"/> 对象。</param>
        public PropertyAccessor(PropertyInfo propertyInfo)
        {
            Guard.ArgumentNull(propertyInfo, nameof(propertyInfo));

            _getter = CreateGetterDelegate(propertyInfo);
            _setter = CreateSetterDelegate(propertyInfo);
        }

        private Func<object?, TValue?>? CreateGetterDelegate(PropertyInfo propertyInfo)
        {
            if (!propertyInfo.CanRead)
            {
                return null;
            }

            if (propertyInfo.PropertyType != typeof(TValue))
            {
                throw new ArgumentException($"访问器的泛型参数与属性 {propertyInfo.Name} 的类型 {propertyInfo.PropertyType} 不一致，因此无法创建取值缓存态委托。");
            }

            var getMethod = propertyInfo.GetGetMethod(true);

            var dm = new DynamicMethod($"{propertyInfo.Name}_Getter", typeof(TValue),
                new Type[] { typeof(object) }, propertyInfo.DeclaringType, true);

            var emiter = new EmitHelper(dm.GetILGenerator());
            emiter.If(!getMethod.IsStatic, e => e.ldarg_0.end())
                .If(getMethod.IsStatic || propertyInfo.DeclaringType.IsValueType, e => e.call(getMethod), e => e.callvirt(getMethod))
                .ret();

            return (Func<object?, TValue?>)dm.CreateDelegate(typeof(Func<object, TValue>));
        }

        private Action<object?, TValue?>? CreateSetterDelegate(PropertyInfo propertyInfo)
        {
            if (!propertyInfo.CanWrite)
            {
                return null;
            }

            if (propertyInfo.PropertyType != typeof(TValue))
            {
                throw new ArgumentException($"访问器的泛型参数与属性 {propertyInfo.Name} 的类型 {propertyInfo.PropertyType} 不一致，因此无法创建取值缓存态委托。");
            }

            var setMethod = propertyInfo.GetSetMethod(true);
            var dm = new DynamicMethod($"{propertyInfo.Name}_Setter", null,
                new Type[] { typeof(object), typeof(TValue) }, propertyInfo.DeclaringType, true);

            var emiter = new EmitHelper(dm.GetILGenerator());
            emiter.If(!setMethod.IsStatic, e => e.ldarg_0.end())
                .ldarg_1
                .If(setMethod.IsStatic || propertyInfo.DeclaringType.IsValueType, e => e.call(setMethod), e => e.callvirt(setMethod))
                .ret();

            return (Action<object?, TValue?>)dm.CreateDelegate(typeof(Action<object, TValue?>));
        }

        /// <summary>
        /// 获取给定对象的属性的值。
        /// </summary>
        /// <param name="instance">实例对象。</param>
        /// <returns></returns>
        public TValue? GetValue(object? instance)
        {
            if (_getter == null)
            {
                throw new NotSupportedException("该访问器不提供读取操作。");
            }

            return _getter(instance);
        }

        /// <summary>
        /// 设置给定对象的属性的值。
        /// </summary>
        /// <param name="instance">实例对象。</param>
        /// <param name="value">属性的值。</param>
        public void SetValue(object? instance, TValue? value)
        {
            if (_setter == null)
            {
                throw new NotSupportedException("该访问器不提供设置操作。");
            }

            _setter(instance, value);
        }

        object? IPropertyAccessor.GetValue(object? instance)
        {
            return GetValue(instance);
        }

        void IPropertyAccessor.SetValue(object? instance, object? value)
        {
            if (value is TValue value1)
            {
                SetValue(instance, value1);
            }
        }
    }
}
