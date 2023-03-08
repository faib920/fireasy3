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
    /// 用于创建一个动态的构造器。
    /// </summary>
    public class DynamicConstructorBuilder : DynamicBuilder
    {
        private ConstructorBuilder _constrBuilder;
        private readonly Action<BuildContext>? _buildAction;
        private readonly MethodAttributes _attributes;
        private readonly Dictionary<string, object> _parameters = new Dictionary<string, object>();

        /// <summary>
        /// 初始化 <see cref="DynamicConstructorBuilder"/> 类的新实例。
        /// </summary>
        /// <param name="context">上下文对象。</param>
        /// <param name="parameterTypes">参数类型数组。</param>
        /// <param name="accessibility">构造函数的访问修饰符。</param>
        /// <param name="modifier">构造函数的修饰符。</param>
        /// <param name="ilCoding">提供一个函数，用于 IL 代码编织。</param>
        internal DynamicConstructorBuilder(BuildContext context, Type[] parameterTypes, Accessibility accessibility = Accessibility.Public, Modifier modifier = Modifier.Standard, Action<BuildContext> ilCoding = null)
            : base(accessibility, modifier)
        {
            Context = new BuildContext(context) { ConstructorBuilder = this };
            ParameterTypes = parameterTypes;

            if (ilCoding == null)
            {
                if (context.TypeBuilder.BaseType != typeof(object))
                {
                    var baseCon = parameterTypes == null ?
                        context.TypeBuilder.BaseType.GetConstructors().FirstOrDefault(s => !s.IsStatic) :
                        context.TypeBuilder.BaseType.GetConstructor(parameterTypes);

                    if (baseCon != null)
                    {
                        ilCoding = c => c.Emitter.ldarg_0
                            .If(parameterTypes != null, b => b.For(0, parameterTypes.Length, (e, i) => e.ldarg(i + 1)))
                            .call(baseCon).ret();
                    }
                }
            }

            ilCoding ??= c => c.Emitter.ret();

            _buildAction = ilCoding;
            _attributes = GetMethodAttributes(accessibility, modifier);
            InitBuilder();
        }

        /// <summary>
        /// 获取参数类型。
        /// </summary>
        public Type[] ParameterTypes { get; private set; }

        /// <summary>
        /// 追加新的 MSIL 代码到构造器中。
        /// </summary>
        /// <param name="ilCoding"></param>
        /// <returns></returns>
        public DynamicConstructorBuilder AppendCode(Action<EmitHelper> ilCoding)
        {
            ilCoding?.Invoke(Context.Emitter);

            return this;
        }

        /// <summary>
        /// 使用新的 MSIL 代码覆盖构造器中的现有代码。
        /// </summary>
        /// <param name="ilCoding"></param>
        /// <returns></returns>
        public DynamicConstructorBuilder OverwriteCode(Action<EmitHelper> ilCoding)
        {
            var builderField = typeof(ConstructorBuilder).GetField("m_methodBuilder", BindingFlags.NonPublic | BindingFlags.Instance);
            var builder = builderField.GetValue(_constrBuilder);
            var field = typeof(MethodBuilder).GetField("m_ilGenerator", BindingFlags.NonPublic | BindingFlags.Instance);
            if (field != null)
            {
                var cons = typeof(ILGenerator).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)[0];
                field.SetValue(builder, cons.Invoke(new[] { builder }));
                Context.Emitter = new EmitHelper(_constrBuilder.GetILGenerator());
            }

            return AppendCode(ilCoding);
        }

        /// <summary>
        /// 定义一个参数。
        /// </summary>
        /// <param name="name">参数的名称。</param>
        /// <returns></returns>
        public DynamicConstructorBuilder DefineParameter(string name)
        {
            ConstructorBuilder.DefineParameter(_parameters.Count + 1, ParameterAttributes.None, name);
            _parameters.Add(name, null);
            return this;
        }

        /// <summary>
        /// 定义一个参数。
        /// </summary>
        /// <param name="name">参数的名称。</param>
        /// <returns></returns>
        public DynamicConstructorBuilder DefineParameter(string name, object defaultValue = null)
        {
            var attrs = ParameterAttributes.Optional;
            if (defaultValue != null)
            {
                attrs |= ParameterAttributes.HasDefault;
            }

            var parameterBuilder = ConstructorBuilder.DefineParameter(_parameters.Count + 1, attrs, name);
            if (defaultValue != null)
            {
                parameterBuilder.SetConstant(defaultValue);
            }

            _parameters.Add(name, defaultValue);
            return this;
        }

        /// <summary>
        /// 获取参数的缺省值。
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public object GetParameterDefaultValue(int index)
        {
            var kvp = _parameters.ElementAt(index);
            if (kvp.Value != null)
            {
                return kvp.Value;
            }

            return null;
        }

        /// <summary>
        /// 获取 <see cref="ConstructorBuilder"/> 对象。
        /// </summary>
        /// <returns></returns>
        public ConstructorBuilder ConstructorBuilder
        {
            get { return _constrBuilder; }
        }

        /// <summary>
        /// 设置一个 <see cref="CustomAttributeBuilder"/> 对象到当前实例关联的 <see cref="ConstructorBuilder"/> 对象。
        /// </summary>
        /// <param name="customBuilder">一个 <see cref="CustomAttributeBuilder"/> 对象。</param>
        protected override void SetCustomAttribute(CustomAttributeBuilder customBuilder)
        {
            ConstructorBuilder.SetCustomAttribute(customBuilder);
        }

        private void InitBuilder()
        {
            var isDefault = (ParameterTypes == null || ParameterTypes.Length == 0) && _buildAction == null;

            if (ParameterTypes != null)
            {
                for (var i = 0; i < ParameterTypes.Length; i++)
                {
                    if (ParameterTypes[i] is GtpType gt && Context.TypeBuilder.TryGetGenericParameterType(gt.Name, out var gt1))
                    {
                        ParameterTypes[i] = gt1.GenericTypeParameterBuilder;
                    }
                }
            }

            _constrBuilder = isDefault ?
                Context.TypeBuilder.TypeBuilder.DefineDefaultConstructor(_attributes) :
                Context.TypeBuilder.TypeBuilder.DefineConstructor(_attributes, CallingConventions.Standard, ParameterTypes);
            if (!isDefault)
            {
                Context.Emitter = new EmitHelper(_constrBuilder.GetILGenerator());
            }

            _buildAction?.Invoke(Context);
        }

        private MethodAttributes GetMethodAttributes(Accessibility accessibility = Accessibility.Public, Modifier modifier = Modifier.Standard)
        {
            var attrs = Context.TypeBuilder.GetMethodAttributes();

            switch (modifier)
            {
                case Modifier.Abstract:
                    attrs |= MethodAttributes.Abstract | MethodAttributes.Virtual | MethodAttributes.NewSlot;
                    break;
                case Modifier.Virtual:
                    attrs |= MethodAttributes.Virtual | MethodAttributes.NewSlot;
                    break;
                case Modifier.Sealed:
                    attrs |= MethodAttributes.Final;
                    break;
                case Modifier.Static:
                    attrs |= MethodAttributes.Static;
                    break;
            }

            switch (accessibility)
            {
                case Accessibility.Internal:
                    attrs |= MethodAttributes.Assembly;
                    break;
                case Accessibility.Public:
                    attrs |= MethodAttributes.Public;
                    break;
                case Accessibility.Protected:
                    attrs |= MethodAttributes.Family;
                    break;
            }

            return attrs;
        }
    }
}
