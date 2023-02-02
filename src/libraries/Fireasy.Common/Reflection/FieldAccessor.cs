// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Fireasy.Common.Emit;
using System.Reflection;
using System.Reflection.Emit;

namespace Fireasy.Common.Reflection
{
    /// <summary>
    /// 包装 <see cref="FieldInfo"/> 对象，创建一个委托来提升字段的读写。
    /// </summary>
    public class FieldAccessor : IFieldAccessor
    {
        private readonly Func<object?, object?> _getter;
        private readonly Action<object?, object?>? _setter;

        /// <summary>
        /// 初始化 <see cref="FieldAccessor"/> 类的新实例。
        /// </summary>
        /// <param name="fieldInfo">要包装的 <see cref="FieldInfo"/> 对象。</param>
        public FieldAccessor(FieldInfo fieldInfo)
        {
            Guard.ArgumentNull(fieldInfo, nameof(fieldInfo));

            _getter = CreateGetterDelegate(fieldInfo);
            _setter = CreateSetterDelegate(fieldInfo);
        }

        private Func<object?, object?> CreateGetterDelegate(FieldInfo fieldInfo)
        {
            var dm = new DynamicMethod($"{fieldInfo.Name}_Getter", typeof(object),
                new Type[] { typeof(object) }, fieldInfo.DeclaringType, true);

            var emiter = new EmitHelper(dm.GetILGenerator());
            emiter.If(!fieldInfo.IsStatic, e => e.ldarg_0.end())
                .If(fieldInfo.IsStatic, e => e.ldsfld(fieldInfo), e => e.ldfld(fieldInfo))
                .If(fieldInfo.FieldType.IsValueType,
                    e => e.box(fieldInfo.FieldType))
                .ret();

            return (Func<object?, object?>)dm.CreateDelegate(typeof(Func<object, object>));
        }

        private Action<object?, object?>? CreateSetterDelegate(FieldInfo fieldInfo)
        {
            if (fieldInfo.IsInitOnly)
            {
                return null;
            }

            var dm = new DynamicMethod($"{fieldInfo.Name}_Setter", null,
                new Type[] { typeof(object), typeof(object) }, fieldInfo.DeclaringType, true);

            var emiter = new EmitHelper(dm.GetILGenerator());
            emiter.If(!fieldInfo.IsStatic, e => e.ldarg_0.end())
                .ldarg_1
                .If(fieldInfo.FieldType.IsValueType,
                    e => e.unbox_any(fieldInfo.FieldType), e => e.castclass(fieldInfo.FieldType))
                .If(fieldInfo.IsStatic, e => e.stsfld(fieldInfo), e => e.stfld(fieldInfo))
                .ret();

            return (Action<object?, object?>)dm.CreateDelegate(typeof(Action<object, object>));
        }

        /// <summary>
        /// 获取给定对象的字段的值。
        /// </summary>
        /// <param name="instance">实例对象。</param>
        /// <returns></returns>
        public object? GetValue(object? instance)
        {
            return _getter(instance);
        }

        /// <summary>
        /// 设置给定对象的字段的值。
        /// </summary>
        /// <param name="instance">实例对象。</param>
        /// <param name="value"></param>
        public void SetValue(object? instance, object? value)
        {
            if (_setter == null)
            {
                throw new NotSupportedException("该访问器不提供设置操作。");
            }

            _setter(instance, value);
        }
    }

    /// <summary>
    /// 包装 <see cref="FieldInfo"/> 对象，创建一个委托来提升字段的读写。
    /// </summary>
    public class FieldAccessor<TValue> : IFieldAccessor<TValue>, IFieldAccessor
    {
        private readonly Func<object?, TValue?> _getter;
        private readonly Action<object?, TValue?>? _setter;

        /// <summary>
        /// 初始化 <see cref="FieldAccessor"/> 类的新实例。
        /// </summary>
        /// <param name="fieldInfo">要包装的 <see cref="FieldInfo"/> 对象。</param>
        public FieldAccessor(FieldInfo fieldInfo)
        {
            Guard.ArgumentNull(fieldInfo, nameof(fieldInfo));

            _getter = CreateGetterDelegate(fieldInfo);
            _setter = CreateSetterDelegate(fieldInfo);
        }

        private Func<object?, TValue?> CreateGetterDelegate(FieldInfo fieldInfo)
        {
            if (fieldInfo.FieldType != typeof(TValue))
            {
                throw new ArgumentException($"访问器的泛型参数与字段 {fieldInfo.Name} 的类型 {fieldInfo.FieldType} 不一致，因此无法创建取值缓存态委托。");
            }

            var dm = new DynamicMethod($"{fieldInfo.Name}_Getter", typeof(TValue),
                new Type[] { typeof(object) }, fieldInfo.DeclaringType, true);

            var emiter = new EmitHelper(dm.GetILGenerator());
            emiter.If(!fieldInfo.IsStatic, e => e.ldarg_0.end())
                .If(fieldInfo.IsStatic, e => e.ldsfld(fieldInfo), e => e.ldfld(fieldInfo))
                .ret();

            return (Func<object?, TValue?>)dm.CreateDelegate(typeof(Func<object, TValue?>));
        }

        private Action<object?, TValue?>? CreateSetterDelegate(FieldInfo fieldInfo)
        {
            if (fieldInfo.IsInitOnly)
            {
                return null;
            }

            if (fieldInfo.FieldType != typeof(TValue))
            {
                throw new ArgumentException($"访问器的泛型参数与字段 {fieldInfo.Name} 的类型 {fieldInfo.FieldType} 不一致，因此无法创建取值缓存态委托。");
            }

            var dm = new DynamicMethod($"{fieldInfo.Name}_Setter", null,
                new Type[] { typeof(object), typeof(TValue) }, fieldInfo.DeclaringType, true);

            var emiter = new EmitHelper(dm.GetILGenerator());
            emiter.If(!fieldInfo.IsStatic, e => e.ldarg_0.end())
                .ldarg_1
                .If(fieldInfo.IsStatic, e => e.stsfld(fieldInfo), e => e.stfld(fieldInfo))
                .ret();

            return (Action<object?, TValue?>)dm.CreateDelegate(typeof(Action<object, TValue?>));
        }

        /// <summary>
        /// 获取给定对象的字段的值。
        /// </summary>
        /// <param name="instance">实例对象。</param>
        /// <returns></returns>
        public TValue? GetValue(object? instance)
        {
            return _getter(instance);
        }

        /// <summary>
        /// 设置给定对象的字段的值。
        /// </summary>
        /// <param name="instance">实例对象。</param>
        /// <param name="value"></param>
        public void SetValue(object? instance, TValue? value)
        {
            if (_setter == null)
            {
                throw new NotSupportedException("该访问器不提供设置操作。");
            }

            _setter(instance, value);
        }

        object? IFieldAccessor.GetValue(object? instance)
        {
            return GetValue(instance);
        }

        void IFieldAccessor.SetValue(object? instance, object? value)
        {
            if (value is TValue value1)
            {
                SetValue(instance, value1);
            }
        }
    }
}
