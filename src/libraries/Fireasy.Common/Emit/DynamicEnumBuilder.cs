﻿// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using System.Reflection;
using System.Reflection.Emit;

namespace Fireasy.Common.Emit
{
    /// <summary>
    /// 用于创建一个动态的枚举。
    /// </summary>
    public class DynamicEnumBuilder : DynamicBuilder, ITypeCreator
    {
        private readonly TypeAttributes _attributes;
        private EnumBuilder _enumBuilder;
        private Type _innerType;

        /// <summary>
        /// 初始化 <see cref="DynamicEnumBuilder"/> 类的新实例。
        /// </summary>
        /// <param name="context">上下文对象。</param>
        /// <param name="enumName">枚举名称。</param>
        /// <param name="underlyingType">枚举的基类型。</param>
        /// <param name="accessibility">枚举的访问修饰符。</param>
        internal DynamicEnumBuilder(BuildContext context, string enumName, Type underlyingType, Accessibility accessibility = Accessibility.Public)
             : base(accessibility, Modifier.Standard)
        {
            Context = new BuildContext(context) { EnumBuilder = this };
            EnumName = enumName;
            UnderlyingType = underlyingType;
            _attributes = GetTypeAttributes(accessibility);
            InitBuilder();
        }

        /// <summary>
        /// 获取枚举的名称。
        /// </summary>
        public string EnumName { get; }

        /// <summary>
        /// 获取枚举的类型。
        /// </summary>
        public Type UnderlyingType { get; }

        /// <summary>
        /// 获取 <see cref="EnumBuilder"/> 对象。
        /// </summary>
        /// <returns></returns>
        public EnumBuilder EnumBuilder
        {
            get { return _enumBuilder; }
        }

        /// <summary>
        /// 定义一个枚举值。
        /// </summary>
        /// <param name="literalName">标签的名称。</param>
        /// <param name="value">枚举值。</param>
        /// <returns></returns>
        public DynamicFieldBuilder DefineLiteral(string literalName, object value)
        {
            return new DynamicFieldBuilder(Context, literalName, UnderlyingType, Convert.ChangeType(value, UnderlyingType));
        }

        /// <summary>
        /// 创建动态的枚举类型。
        /// </summary>
        /// <returns></returns>
        public Type CreateType()
        {
            if (_innerType != null)
            {
                return _innerType;
            }
#if NETFRAMEWORK
            _innerType = _enumBuilder.CreateType();
#else
            _innerType = _enumBuilder.CreateTypeInfo();
#endif
            return _innerType;
        }

        /// <summary>
        /// 设置一个 <see cref="CustomAttributeBuilder"/> 对象到当前实例关联的 <see cref="EnumBuilder"/> 对象。
        /// </summary>
        /// <param name="customBuilder">一个 <see cref="CustomAttributeBuilder"/> 对象。</param>
        protected override void SetCustomAttribute(CustomAttributeBuilder customBuilder)
        {
            _enumBuilder.SetCustomAttribute(customBuilder);
        }

        private TypeAttributes GetTypeAttributes(Accessibility accessibility)
        {
            var attrs = TypeAttributes.Class;
            switch (accessibility)
            {
                case Accessibility.Internal:
                    break;
                case Accessibility.Public:
                    attrs |= TypeAttributes.Public;
                    break;
            }

            return attrs;
        }

        private void InitBuilder()
        {
            _enumBuilder = Context.AssemblyBuilder.ModuleBuilder.DefineEnum(EnumName, _attributes, UnderlyingType);
        }
    }
}
