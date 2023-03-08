// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using System.Globalization;
using System.Reflection;
using System.Reflection.Emit;

namespace Fireasy.Common.Emit
{
    /// <summary>
    /// 用于标识一个泛型类型参数的类型。无法继承此类。
    /// </summary>
    public sealed class GtpType : Type
    {
        private Type? _baseType;
        private Type[]? _constraintTypes;
        private GenericParameterAttributes? _parameterAttributes;

        /// <summary>
        /// 使用名称初始化 <see cref="GtpType"/> 类的新实例。
        /// </summary>
        /// <param name="name">泛型类型参数名称。</param>
        public GtpType(string name) 
        {
            Name = name;
        }

        /// <summary>
        /// 未实现。
        /// </summary>
        public override Assembly? Assembly => null;

        /// <summary>
        /// 未实现。
        /// </summary>
        public override string? AssemblyQualifiedName => null;

        /// <summary>
        /// 未实现。
        /// </summary>
        public override Type? BaseType => _baseType;

        /// <summary>
        /// 未实现。
        /// </summary>
        public override string? FullName => null;

        /// <summary>
        /// 未实现。
        /// </summary>
        public override Guid GUID => Guid.Empty;

        /// <summary>
        /// 未实现。
        /// </summary>
        public override Module? Module => null;

        /// <summary>
        /// 未实现。
        /// </summary>
        public override string? Namespace => null;

        /// <summary>
        /// 未实现。
        /// </summary>
        public override Type? UnderlyingSystemType => null;

        /// <summary>
        /// 获取名称。
        /// </summary>
        public override string Name { get; }

        /// <summary>
        /// 未实现。
        /// </summary>
        /// <param name="bindingAttr"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public override ConstructorInfo[] GetConstructors(BindingFlags bindingAttr)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 未实现。
        /// </summary>
        /// <param name="inherit"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public override object[] GetCustomAttributes(bool inherit)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 未实现。
        /// </summary>
        /// <param name="attributeType"></param>
        /// <param name="inherit"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public override object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 未实现。
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public override Type? GetElementType()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 未实现。
        /// </summary>
        /// <param name="name"></param>
        /// <param name="bindingAttr"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public override EventInfo? GetEvent(string name, BindingFlags bindingAttr)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 未实现。
        /// </summary>
        /// <param name="bindingAttr"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public override EventInfo[] GetEvents(BindingFlags bindingAttr)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 未实现。
        /// </summary>
        /// <param name="name"></param>
        /// <param name="bindingAttr"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public override FieldInfo? GetField(string name, BindingFlags bindingAttr)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 未实现。
        /// </summary>
        /// <param name="bindingAttr"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public override FieldInfo[] GetFields(BindingFlags bindingAttr)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 未实现。
        /// </summary>
        /// <param name="name"></param>
        /// <param name="ignoreCase"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public override Type? GetInterface(string name, bool ignoreCase)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 未实现。
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public override Type[] GetInterfaces()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 未实现。
        /// </summary>
        /// <param name="bindingAttr"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public override MemberInfo[] GetMembers(BindingFlags bindingAttr)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 未实现。
        /// </summary>
        /// <param name="bindingAttr"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public override MethodInfo[] GetMethods(BindingFlags bindingAttr)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 未实现。
        /// </summary>
        /// <param name="name"></param>
        /// <param name="bindingAttr"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public override Type? GetNestedType(string name, BindingFlags bindingAttr)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 未实现。
        /// </summary>
        /// <param name="bindingAttr"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public override Type[] GetNestedTypes(BindingFlags bindingAttr)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 未实现。
        /// </summary>
        /// <param name="bindingAttr"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public override PropertyInfo[] GetProperties(BindingFlags bindingAttr)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 未实现。
        /// </summary>
        /// <param name="name"></param>
        /// <param name="invokeAttr"></param>
        /// <param name="binder"></param>
        /// <param name="target"></param>
        /// <param name="args"></param>
        /// <param name="modifiers"></param>
        /// <param name="culture"></param>
        /// <param name="namedParameters"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public override object? InvokeMember(string name, BindingFlags invokeAttr, Binder? binder, object? target, object?[]? args, ParameterModifier[]? modifiers, CultureInfo? culture, string[]? namedParameters)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 未实现。
        /// </summary>
        /// <param name="attributeType"></param>
        /// <param name="inherit"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public override bool IsDefined(Type attributeType, bool inherit)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 未实现。
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        protected override TypeAttributes GetAttributeFlagsImpl()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 未实现。
        /// </summary>
        /// <param name="bindingAttr"></param>
        /// <param name="binder"></param>
        /// <param name="callConvention"></param>
        /// <param name="types"></param>
        /// <param name="modifiers"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        protected override ConstructorInfo? GetConstructorImpl(BindingFlags bindingAttr, Binder? binder, CallingConventions callConvention, Type[] types, ParameterModifier[]? modifiers)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 未实现。
        /// </summary>
        /// <param name="name"></param>
        /// <param name="bindingAttr"></param>
        /// <param name="binder"></param>
        /// <param name="callConvention"></param>
        /// <param name="types"></param>
        /// <param name="modifiers"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        protected override MethodInfo? GetMethodImpl(string name, BindingFlags bindingAttr, Binder? binder, CallingConventions callConvention, Type[]? types, ParameterModifier[]? modifiers)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 未实现。
        /// </summary>
        /// <param name="name"></param>
        /// <param name="bindingAttr"></param>
        /// <param name="binder"></param>
        /// <param name="returnType"></param>
        /// <param name="types"></param>
        /// <param name="modifiers"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        protected override PropertyInfo? GetPropertyImpl(string name, BindingFlags bindingAttr, Binder? binder, Type? returnType, Type[]? types, ParameterModifier[]? modifiers)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 未实现。
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        protected override bool HasElementTypeImpl()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 未实现。
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        protected override bool IsArrayImpl()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 未实现。
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        protected override bool IsByRefImpl()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 未实现。
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        protected override bool IsCOMObjectImpl()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 未实现。
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        protected override bool IsPointerImpl()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 未实现。
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        protected override bool IsPrimitiveImpl()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 获取 <see cref="GenericTypeParameterBuilder"/> 实例。
        /// </summary>
        public GenericTypeParameterBuilder? GenericTypeParameterBuilder { get; private set; }

        /// <summary>
        /// 设置基类约束。
        /// </summary>
        /// <param name="baseType">基类类型。</param>
        /// <returns></returns>
        public GtpType SetBaseTypeConstraint(Type baseType)
        {
            _baseType = baseType;
            return this;
        }

        /// <summary>
        /// 设置接口约束。
        /// </summary>
        /// <param name="constraintTypes">约束类型。</param>
        /// <returns></returns>
        public GtpType SetInterfaceConstraints(params Type[] constraintTypes)
        {
            _constraintTypes = constraintTypes;
            return this;
        }

        /// <summary>
        /// 设置参数特性。
        /// </summary>
        /// <param name="attributes"></param>
        /// <returns></returns>
        public GtpType SetGenericParameterAttributes(GenericParameterAttributes attributes)
        {
            _parameterAttributes = attributes;
            return this;
        }

        internal GenericTypeParameterBuilder Initialize(GenericTypeParameterBuilder builder)
        {
            if (GenericTypeParameterBuilder == null)
            {
                builder.SetBaseTypeConstraint(_baseType);

                if (_constraintTypes != null)
                {
                    builder.SetInterfaceConstraints(_constraintTypes);
                }
                if (_parameterAttributes != null)
                {
                    builder.SetGenericParameterAttributes(_parameterAttributes.Value);
                }

                GenericTypeParameterBuilder = builder;
            }

            return builder;
        }
    }
}
