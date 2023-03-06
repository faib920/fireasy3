// -----------------------------------------------------------------------
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
    /// 用于创建一个字段。
    /// </summary>
    public class DynamicFieldBuilder : DynamicBuilder
    {
        private FieldBuilder _fieldBuilder;
        private readonly object? _defaultValue;
        private readonly FieldAttributes _attributes;

        /// <summary>
        /// 初始化 <see cref="DynamicFieldBuilder"/> 类的新实例。
        /// </summary>
        /// <param name="context">上下文对象。</param>
        /// <param name="fieldName">字段名称。</param>
        /// <param name="fieldType">字段的类型。</param>
        /// <param name="defaultValue">默认值。</param>
        /// <param name="accessibility">字段的访问修饰符。</param>
        /// <param name="modifier">字段的修饰符。</param>
        internal DynamicFieldBuilder(BuildContext context, string fieldName, Type fieldType, object? defaultValue = null, Accessibility accessibility = Accessibility.Private, Modifier modifier = Modifier.Standard)
            : base(accessibility, modifier)
        {
            FieldName = fieldName;
            FieldType = fieldType;
            _attributes = GetAttributes(accessibility, modifier);
            _defaultValue = defaultValue;
            Context = context;
            InitBuilder();
        }

        /// <summary>
        /// 获取字段的名称。
        /// </summary>
        public string FieldName { get; }

        /// <summary>
        /// 获取字段的类型。
        /// </summary>
        public Type FieldType { get; }

        /// <summary>
        /// 设置一个 <see cref="CustomAttributeBuilder"/> 对象到当前实例关联的 <see cref="FieldBuilder"/> 对象。
        /// </summary>
        /// <param name="customBuilder">一个 <see cref="CustomAttributeBuilder"/> 对象。</param>
        protected override void SetCustomAttribute(CustomAttributeBuilder customBuilder)
        {
            _fieldBuilder.SetCustomAttribute(customBuilder);
        }

        /// <summary>
        /// 获取 <see cref="FieldBuilder"/> 对象。
        /// </summary>
        /// <returns></returns>
        public FieldBuilder FieldBuilder
        {
            get { return _fieldBuilder; }
        }

        private FieldAttributes GetAttributes(Accessibility accessibility, Modifier modifier)
        {
            var attrs = FieldAttributes.HasDefault;
            switch (modifier)
            {
                case Modifier.Static:
                    attrs |= FieldAttributes.Static;
                    break;
            }

            switch (accessibility)
            {
                case Accessibility.Internal:
                    attrs |= FieldAttributes.Assembly;
                    break;
                case Accessibility.Private:
                    attrs |= FieldAttributes.Private;
                    break;
                case Accessibility.Public:
                    attrs |= FieldAttributes.Public;
                    break;
            }

            return attrs;
        }

        private void InitBuilder()
        {
            if (Context.EnumBuilder == null)
            {
                _fieldBuilder = Context.TypeBuilder.TypeBuilder.DefineField(FieldName, FieldType, _attributes);
                if (_defaultValue != null)
                {
                    _fieldBuilder.SetConstant(_defaultValue);
                }
            }
            else
            {
                _fieldBuilder = Context.EnumBuilder.EnumBuilder.DefineLiteral(FieldName, _defaultValue);
            }
        }
    }
}
