﻿// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using System.Collections.ObjectModel;
using System.Reflection;
using System.Reflection.Emit;

namespace Fireasy.Common.Emit
{
    /// <summary>
    /// 用于创建一个动态的类型。
    /// </summary>
    public class DynamicTypeBuilder : DynamicBuilder, ITypeCreator
    {
        private readonly TypeAttributes _attributes;
        private TypeBuilder _typeBuilder;
        private readonly List<Type> _interfaceTypes = new List<Type>();
        private readonly bool _isNesetType;
        private Type _innerType;
        private readonly List<ITypeCreator> _nestedTypeBuilders = new List<ITypeCreator>();
        private Dictionary<string, GtpType> _genericParameterTypes;

        private Type? _baseType;

        /// <summary>
        /// 初始化 <see cref="DynamicTypeBuilder"/> 类的新实例。
        /// </summary>
        /// <param name="context">上下文对象。</param>
        /// <param name="typeName">动态类型的名称。</param>
        /// <param name="accessibility">类的访问修饰符。</param>
        /// <param name="modifier">类的修饰符。</param>
        /// <param name="baseType">继承的基类。</param>
        internal DynamicTypeBuilder(BuildContext context, string typeName, Accessibility accessibility = Accessibility.Public, Modifier modifier = Modifier.Standard, Type? baseType = null)
            : base(accessibility, modifier)
        {
            Context = new BuildContext(context);
            TypeName = typeName;
            _baseType = baseType;
            _attributes = GetTypeAttributes(accessibility, modifier);
            InitBuilder();

            Context.TypeBuilder = this;
        }

        /// <summary>
        /// 初始化 <see cref="DynamicTypeBuilder"/> 类的新实例。
        /// </summary>
        /// <param name="context">上下文对象。</param>
        /// <param name="typeName">动态类型的名称。</param>
        /// <param name="accessibility">类的访问修饰符。</param>
        /// <param name="baseType"></param>
        internal DynamicTypeBuilder(BuildContext context, string typeName, Accessibility accessibility, Type baseType)
            : base(accessibility, Modifier.Standard)
        {
            Context = new BuildContext(context);
            _isNesetType = true;
            TypeName = typeName;
            _baseType = baseType;
            _attributes = GetTypeAttributes(accessibility, Modifier.Standard);
            InitBuilder();

            Context.TypeBuilder = this;
        }

        /// <summary>
        /// 获取或设置动态类型所继承的类型。
        /// </summary>
        public virtual Type BaseType
        {
            get
            {
                return _baseType ?? typeof(object);
            }

            set
            {
                _baseType = value;
                _typeBuilder.SetParent(value);
            }
        }

        /// <summary>
        /// 获取动态类型的名称。
        /// </summary>
        public string TypeName { get; }

        /// <summary>
        /// 获取动态类型所要实现的接口集合。
        /// </summary>
        public ReadOnlyCollection<Type> InterfaceTypes
        {
            get
            {
                return _interfaceTypes.AsReadOnly();
            }
        }

        /// <summary>
        /// 返回此 TypeBuilder 的基础系统类型。
        /// </summary>
        public Type UnderlyingSystemType
        {
            get
            {
                return _typeBuilder.UnderlyingSystemType;
            }
        }

        /// <summary>
        /// 获取当前的 <see cref="TypeBuilder"/>。
        /// </summary>
        /// <returns></returns>
        public TypeBuilder TypeBuilder
        {
            get { return _typeBuilder; }
        }

        /// <summary>
        /// 定义泛型参数。
        /// </summary>
        /// <param name="parameterTypes"></param>
        /// <returns></returns>
        public void DefineGenericParameters(params GtpType[] parameterTypes)
        {
            if (_genericParameterTypes != null)
            {
                throw new InvalidOperationException("已经定义过泛型参数。");
            }

            _genericParameterTypes = parameterTypes.ToDictionary(s => s.Name);
            var builders = _typeBuilder.DefineGenericParameters(parameterTypes.Select(s => s.Name).ToArray());

            for (var i = 0; i < parameterTypes.Length; i++)
            {
                parameterTypes[i].Initialize(builders[i]);
            }
        }

        /// <summary>
        /// 获取指定名称对应的 <see cref="GtpType"/> 实例。
        /// </summary>
        /// <param name="name">参数名称。</param>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool TryGetGenericParameterType(string name, out GtpType? type)
        {
            if (_genericParameterTypes == null)
            {
                type = null;
                return false;
            }

            return _genericParameterTypes.TryGetValue(name, out type);
        }

        /// <summary>
        /// 创建动态类型。
        /// </summary>
        /// <returns></returns>
        public Type CreateType()
        {
            if (_innerType != null)
            {
                return _innerType;
            }

            foreach (var builder in _nestedTypeBuilders)
            {
                builder.CreateType();
            }

#if NETFRAMEWORK
            _innerType = _typeBuilder.CreateType();
#else
            _innerType = _typeBuilder.CreateTypeInfo();
#endif
            return _innerType;
        }

        /// <summary>
        /// 添加此类型实现的接口。
        /// </summary>
        /// <param name="type">接口的类型。</param>
        /// <returns>当前的 <see cref="TypeBuilder"/>。</returns>
        public DynamicTypeBuilder ImplementInterface(Type type)
        {
            if (_interfaceTypes.Contains(type))
            {
                return this;
            }

            _interfaceTypes.Add(type);
            _typeBuilder.AddInterfaceImplementation(type);

            return this;
        }

        /// <summary>
        /// 定义一个属性。
        /// </summary>
        /// <param name="propertyName">属性的名称。</param>
        /// <param name="propertyType">属性的类型。</param>
        /// <param name="accessibility">指定属性的可见性。</param>
        /// <param name="modifier">指定属性的调用属性。</param>
        /// <returns>新的 <see cref="DynamicPropertyBuilder"/>。</returns>
        public virtual DynamicPropertyBuilder DefineProperty(string propertyName, Type propertyType, Accessibility accessibility = Accessibility.Public, Modifier modifier = Modifier.Standard)
        {
            return new DynamicPropertyBuilder(Context, propertyName, propertyType, accessibility, modifier);
        }

        /// <summary>
        /// 定义一个方法。
        /// </summary>
        /// <param name="methodName">方法的名称。</param>
        /// <param name="returnType">返回值的类型，如果为 void 则该参数为 null。</param>
        /// <param name="parameterTypes">一个数组，表示方法的传入参数类型。</param>
        /// <param name="accessibility">指定方法的可见性。</param>
        /// <param name="modifier">指定方法的调用属性。</param>
        /// <param name="ilCoding">方法体的 IL 过程。</param>
        /// <returns>新的 <see cref="DynamicMethodBuilder"/>。</returns>
        public virtual DynamicMethodBuilder DefineMethod(string methodName, Type? returnType = null, Type[]? parameterTypes = null, Accessibility accessibility = Accessibility.Public, Modifier modifier = Modifier.Standard, Action<BuildContext> ilCoding = null)
        {
            return new DynamicMethodBuilder(Context, methodName, returnType, parameterTypes, accessibility, modifier, ilCoding);
        }

        /// <summary>
        /// 定义一个构造函数。
        /// </summary>
        /// <param name="parameterTypes"></param>
        /// <param name="accessibility"></param>
        /// <param name="modifier"></param>
        /// <param name="ilCoding"></param>
        /// <returns></returns>
        public virtual DynamicConstructorBuilder DefineConstructor(Type[] parameterTypes, Accessibility accessibility = Accessibility.Public, Modifier modifier = Modifier.Standard, Action<BuildContext> ilCoding = null)
        {
            return new DynamicConstructorBuilder(Context, parameterTypes, accessibility, modifier, ilCoding);
        }

        /// <summary>
        /// 定义一个字段。
        /// </summary>
        /// <param name="fieldName">字段的名称。</param>
        /// <param name="fieldType">字段的类型。</param>
        /// <param name="defaultValue">默认值。</param>
        /// <param name="accessibility"></param>
        /// <param name="modifier"></param>
        /// <returns></returns>
        public virtual DynamicFieldBuilder DefineField(string fieldName, Type fieldType, object? defaultValue = null, Accessibility accessibility = Accessibility.Private, Modifier modifier = Modifier.Standard)
        {
            return new DynamicFieldBuilder(Context, fieldName, fieldType, defaultValue, accessibility, modifier);
        }

        /// <summary>
        /// 定义一个嵌套的类型。
        /// </summary>
        /// <param name="typeName"></param>
        /// <param name="accessibility"></param>
        /// <param name="baseType"></param>
        /// <returns></returns>
        public virtual DynamicTypeBuilder DefineNestedType(string typeName, Accessibility accessibility = Accessibility.Private, Type? baseType = null)
        {
            var nestedType = new DynamicTypeBuilder(Context, typeName, accessibility, baseType);
            _nestedTypeBuilders.Add(nestedType);
            return nestedType;
        }

        /// <summary>
        /// 使用当前的构造器定义一个动态接口。
        /// </summary>
        /// <param name="typeName">类型的名称。</param>
        /// <param name="accessibility">指定类的可见性。</param>
        /// <returns></returns>
        public DynamicInterfaceBuilder DefineNestedInterface(string typeName, Accessibility accessibility = Accessibility.Public)
        {
            var typeBuilder = new DynamicInterfaceBuilder(Context, typeName, accessibility);
            _nestedTypeBuilders.Add(typeBuilder);
            return typeBuilder;
        }

        /// <summary>
        /// 使用当前构造器定义一个枚举。
        /// </summary>
        /// <param name="enumName">枚举的名称。</param>
        /// <param name="underlyingType">枚举的类型。</param>
        /// <param name="accessibility">指定枚举的可见性。</param>
        /// <returns></returns>
        public DynamicEnumBuilder DefineNestedEnum(string enumName, Type? underlyingType = null, Accessibility accessibility = Accessibility.Public)
        {
            var enumBuilder = new DynamicEnumBuilder(Context, enumName, underlyingType ?? typeof(int), accessibility);
            _nestedTypeBuilders.Add(enumBuilder);
            return enumBuilder;
        }

        /// <summary>
        /// 获取 <see cref="TypeAttributes"/>。
        /// </summary>
        /// <returns></returns>
        protected virtual TypeAttributes GetTypeAttributes()
        {
            return TypeAttributes.Class | TypeAttributes.BeforeFieldInit;
        }

        /// <summary>
        /// 设置一个 <see cref="CustomAttributeBuilder"/> 对象到当前实例关联的 <see cref="TypeBuilder"/> 对象。
        /// </summary>
        /// <param name="customBuilder">一个 <see cref="CustomAttributeBuilder"/> 对象。</param>
        protected override void SetCustomAttribute(CustomAttributeBuilder customBuilder)
        {
            _typeBuilder.SetCustomAttribute(customBuilder);
        }

        private TypeAttributes GetTypeAttributes(Accessibility accessibility, Modifier modifier)
        {
            var attrs = GetTypeAttributes();
            switch (modifier)
            {
                case Modifier.Abstract:
                    attrs |= TypeAttributes.Abstract;
                    break;
                case Modifier.Sealed:
                    attrs |= TypeAttributes.Sealed;
                    break;
            }

            switch (accessibility)
            {
                case Accessibility.Internal:
                    if (_isNesetType)
                    {
                        attrs |= TypeAttributes.NestedAssembly;
                    }

                    break;
                case Accessibility.Private:
                    if (_isNesetType)
                    {
                        attrs |= TypeAttributes.NestedPrivate;
                    }

                    break;
                case Accessibility.Public:
                    attrs |= _isNesetType ? TypeAttributes.NestedPublic : TypeAttributes.Public;
                    break;
            }

            return attrs;
        }

        internal virtual PropertyAttributes GetPropertyAttributes()
        {
            return PropertyAttributes.None;
        }

        internal virtual MethodAttributes GetMethodAttributes()
        {
            return MethodAttributes.SpecialName | MethodAttributes.RTSpecialName | MethodAttributes.HideBySig;
        }

        private void InitBuilder()
        {
            Func<string, TypeAttributes, Type, TypeBuilder> defineFunc = _isNesetType ?
                Context.TypeBuilder.TypeBuilder.DefineNestedType :
                Context.AssemblyBuilder.ModuleBuilder.DefineType;

            _typeBuilder = defineFunc(TypeName, _attributes, BaseType);
        }
    }
}
