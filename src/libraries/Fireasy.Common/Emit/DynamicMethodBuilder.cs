// -----------------------------------------------------------------------
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
    /// 用于创建一个动态的方法。
    /// </summary>
    public class DynamicMethodBuilder : DynamicBuilder
    {
        private MethodBuilder _methodBuilder;
        private readonly MethodAttributes _attributes;
        private readonly Action<BuildContext>? _buildAction;
        private readonly List<string> _parameters = new List<string>();
        private Type? _returnType;
        private Type[]? _parameterTypes;

        /// <summary>
        /// 初始化 <see cref="DynamicMethodBuilder"/> 类的新实例。
        /// </summary>
        /// <param name="context">上下文对象。</param>
        /// <param name="methodName">方法名称。</param>
        /// <param name="returnType">返回类型。</param>
        /// <param name="parameterTypes">参数类型数组。</param>
        /// <param name="accessibility">方法的访问修饰符。</param>
        /// <param name="midifier">方法的修饰符。</param>
        /// <param name="ilCoding">提供一个函数，用于 IL 代码编织。</param>
        internal DynamicMethodBuilder(BuildContext context, string methodName, Type? returnType = null, Type[]? parameterTypes = null, Accessibility accessibility = Accessibility.Public, Modifier midifier = Modifier.Standard, Action<BuildContext>? ilCoding = null)
             : base(accessibility, midifier)
        {
            Context = new BuildContext(context) { MethodBuilder = this };
            Name = methodName;
            ReturnType = returnType;
            ParameterTypes = parameterTypes;
            _buildAction = ilCoding;
            _attributes = GetMethodAttributes(methodName, parameterTypes, accessibility, midifier);
            InitBuilder();
            ProcessGenericMethod();
        }

        /// <summary>
        /// 定义一个参数。
        /// </summary>
        /// <param name="name">参数的名称。</param>
        /// <param name="isOut">是否为 out 类型的参数。</param>
        /// <param name="hasDefaultValue">是否指定缺省值。</param>
        /// <param name="defaultValue">缺省的参数值。</param>
        /// <returns></returns>
        public DynamicMethodBuilder DefineParameter(string name, bool isOut = false, bool hasDefaultValue = false, object? defaultValue = null)
        {
            var attr = hasDefaultValue ? ParameterAttributes.HasDefault : ParameterAttributes.None;
            if (isOut)
            {
                attr |= ParameterAttributes.Out;
            }

            var parameter = MethodBuilder.DefineParameter(_parameters.Count + 1, attr, name);
            if (hasDefaultValue)
            {
                parameter.SetConstant(defaultValue);
            }

            _parameters.Add(name);
            return this;
        }

        /// <summary>
        /// 获取方法的名称。
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// 获取或设置方法的返回类型。
        /// </summary>
        public Type? ReturnType
        {
            get
            {
                return _returnType;
            }

            set
            {
                _returnType = value;
                if (_methodBuilder != null)
                {
                    _methodBuilder.SetReturnType(_returnType);
                }
            }
        }

        /// <summary>
        /// 获取或设置方法的参数类型数组。
        /// </summary>
        public Type[]? ParameterTypes
        {
            get
            {
                return _parameterTypes;
            }

            set
            {
                _parameterTypes = value;
                if (_methodBuilder != null)
                {
                    _methodBuilder.SetParameters(_parameterTypes);
                }
            }
        }

        /// <summary>
        /// 获取当前的 <see cref="MethodBuilder"/>。
        /// </summary>
        /// <returns></returns>
        public MethodBuilder MethodBuilder
        {
            get { return _methodBuilder; }
        }

        /// <summary>
        /// 追加新的 MSIL 代码到构造器中。
        /// </summary>
        /// <param name="ilCoding"></param>
        /// <returns></returns>
        public DynamicMethodBuilder AppendCode(Action<EmitHelper> ilCoding)
        {
            ilCoding?.Invoke(Context.Emitter);

            return this;
        }

        /// <summary>
        /// 使用新的 MSIL 代码覆盖构造器中的现有代码。
        /// </summary>
        /// <param name="ilCoding"></param>
        /// <returns></returns>
        public DynamicMethodBuilder OverwriteCode(Action<EmitHelper> ilCoding)
        {
            var field = typeof(MethodBuilder).GetField("m_ilGenerator", BindingFlags.NonPublic | BindingFlags.Instance);
            if (field != null)
            {
                var cons = typeof(ILGenerator).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)[0];
                field.SetValue(_methodBuilder, cons.Invoke(new[] { _methodBuilder }));
                Context.Emitter = new EmitHelper(_methodBuilder.GetILGenerator(), _methodBuilder);
            }

            return AppendCode(ilCoding);
        }

        /// <summary>
        /// 设置一个 <see cref="CustomAttributeBuilder"/> 对象到当前实例关联的 <see cref="MethodBuilder"/> 对象。
        /// </summary>
        /// <param name="customBuilder">一个 <see cref="CustomAttributeBuilder"/> 对象。</param>
        protected override void SetCustomAttribute(CustomAttributeBuilder customBuilder)
        {
            MethodBuilder.SetCustomAttribute(customBuilder);
        }

        private MethodInfo? FindMethod(string methodName, IEnumerable<Type> parameterTypes)
        {
            MethodInfo? method = null;
            if (Context.TypeBuilder.BaseType != null)
            {
                if (parameterTypes == null || parameterTypes.Count() == 0)
                {
                    method = Context.TypeBuilder.BaseType.GetMethods().FirstOrDefault(s => s.Name == methodName && s.GetParameters().Length == 0 && (s.ReturnType == ReturnType || (ReturnType == null && s.ReturnType == typeof(void))));
                }
                else if (parameterTypes.Any(s => s is GtpType))
                {
                    var gtypes = parameterTypes.Where(s => s is GtpType).Cast<GtpType>().ToArray();
                    method = Context.TypeBuilder.BaseType.GetMethods().FirstOrDefault(s => s.Name == methodName && IsEquals(s.GetParameters(), gtypes));
                }
                else
                {
                    method = Context.TypeBuilder.BaseType.GetMethods().FirstOrDefault(s => s.Name == methodName && IsEquals(s.GetParameters(), parameterTypes.ToArray()));
                }

                if (method != null && !method.IsVirtual)
                {
                    throw new DynamicBuildException("所定义的方法在父类中未标记 virtual、abstract 或 override。");
                }
            }

            //在实现的接口中查找方法
            var interfaceTypes = Context.TypeBuilder.InterfaceTypes
                .Union(Context.TypeBuilder.InterfaceTypes.SelectMany(s => s.GetInterfaces()))
                .Distinct().ToList();

            if (method == null && interfaceTypes.Count != 0)
            {
                foreach (var type in interfaceTypes)
                {
                    method = type.GetMethod(methodName, parameterTypes == null ? Type.EmptyTypes : parameterTypes.ToArray());
                    if (method != null)
                    {
                        break;
                    }
                }
            }

            return method;
        }

        private MethodAttributes GetMethodAttributes(Accessibility accessibility = Accessibility.Public, Modifier modifier = Modifier.Standard)
        {
            var attributes = Context.TypeBuilder.GetMethodAttributes();

            switch (modifier)
            {
                case Modifier.Abstract:
                    attributes |= MethodAttributes.Abstract | MethodAttributes.Virtual | MethodAttributes.NewSlot;
                    break;
                case Modifier.Virtual:
                    attributes |= MethodAttributes.Virtual | MethodAttributes.NewSlot;
                    break;
                case Modifier.Sealed:
                    attributes |= MethodAttributes.Final;
                    break;
                case Modifier.Static:
                    attributes |= MethodAttributes.Static;
                    break;
                case Modifier.ExplicitImpl:
                    attributes |= MethodAttributes.Private | MethodAttributes.Final;
                    break;
            }

            switch (accessibility)
            {
                case Accessibility.Internal:
                    attributes |= MethodAttributes.Assembly;
                    break;
                case Accessibility.Public:
                    if (modifier != Modifier.ExplicitImpl)
                    {
                        attributes |= MethodAttributes.Public;
                    }
                    break;
                case Accessibility.Protected:
                    attributes |= MethodAttributes.Family;
                    break;
            }

            return attributes;
        }

        private MethodAttributes GetMethodAttributes(string methodName, IEnumerable<Type> parameterTypes, Accessibility accessibility, Modifier modifier)
        {
            var method = FindMethod(methodName, parameterTypes);
            var isOverride = method != null && method.IsVirtual;
            var isInterface1 = isOverride && method!.DeclaringType!.IsInterface;
            var isBaseType = isOverride && method!.DeclaringType == Context.TypeBuilder.BaseType;
            if (method != null)
            {
                Context.BaseMethod = method;
            }

            var attrs = GetMethodAttributes(accessibility, modifier);
            if (isOverride)
            {
                attrs |= MethodAttributes.Virtual;

                //去掉 NewSlot
                if (isBaseType && _attributes.HasFlag(MethodAttributes.NewSlot))
                {
                    attrs &= ~MethodAttributes.NewSlot;
                }
                else if (isInterface1)
                {
                    //如果没有传入 midifier，则加 Final 去除上面定义的 Virtual
                    if (modifier == Modifier.Standard)
                    {
                        attrs |= MethodAttributes.Final;
                    }

                    attrs |= MethodAttributes.NewSlot;
                }
            }
            else if (method != null)
            {
            }

            return attrs;
        }

        private void ProcessGenericMethod()
        {
            if (ParameterTypes?.Any(s => s is GtpType) == true)
            {
                var names = ParameterTypes.Where(s => s is GtpType).Where(s => !Context.TypeBuilder.TryGetGenericParameterType(s.Name, out _)).Cast<GtpType>().Select(s => s.Name).ToArray();
                Dictionary<string, GenericTypeParameterBuilder>? builders = null;
                
                if (names.Length > 0)
                {
                     builders = _methodBuilder.DefineGenericParameters(names).ToDictionary(s => s.Name);
                }

                for (var i = 0; i < ParameterTypes.Length; i++)
                {
                    if (ParameterTypes[i] is GtpType gt)
                    {
                        if (builders?.TryGetValue(gt.Name, out var parb) == true)
                        {
                            ParameterTypes[i] = gt.Initialize(parb);
                        }
                        else if (Context.TypeBuilder.TryGetGenericParameterType(gt.Name, out var gt1))
                        {
                            ParameterTypes[i] = gt1.GenericTypeParameterBuilder;
                        }
                    }
                }

                MethodBuilder.SetParameters(ParameterTypes);

                if (ReturnType is GtpType rgt)
                {
                    if (builders?.TryGetValue(rgt.Name, out var retb) == true)
                    {
                        ReturnType = rgt.Initialize(retb);
                    }
                    else if (Context.TypeBuilder.TryGetGenericParameterType(rgt.Name, out var gt1))
                    {
                        ReturnType = gt1.GenericTypeParameterBuilder;
                    }
                }
            }
        }

        private void InitBuilder()
        {
            var isOverride = Modifier == Modifier.ExplicitImpl && Context.BaseMethod != null;

            if (isOverride)
            {
                _methodBuilder = Context.TypeBuilder.TypeBuilder.DefineMethod(string.Concat(Context.BaseMethod.DeclaringType.Name, ".", Name), _attributes);
            }
            else
            {
                _methodBuilder = Context.TypeBuilder.TypeBuilder.DefineMethod(Name, _attributes);
            }

            Context.Emitter = new EmitHelper(_methodBuilder.GetILGenerator(), _methodBuilder);
            if (ParameterTypes != null)
            {
                _methodBuilder.SetParameters(ParameterTypes);
            }

            if (ReturnType != null)
            {
                _methodBuilder.SetReturnType(ReturnType);
            }

            if (_buildAction != null)
            {
                _buildAction(Context);
            }
            else
            {
                Context.Emitter.ret();
            }

            if (isOverride)
            {
                Context.TypeBuilder.TypeBuilder.DefineMethodOverride(_methodBuilder, Context.BaseMethod);
            }
        }

        private static bool IsEquals(ParameterInfo[] parameters, Type[] types2)
        {
            if (parameters.Length != types2.Length)
            {
                return false;
            }

            for (var i = 0; i < parameters.Length; i++)
            {
                if (parameters[i].ParameterType != types2[i])
                {
                    return false;
                }
            }

            return true;
        }

        private static bool IsEquals(ParameterInfo[] parameters, GtpType[] gtypes)
        {
            var types = parameters.Where(s => s.ParameterType.IsGenericParameter).Select(s => s.ParameterType).ToArray();

            if (types.Length != gtypes.Length)
            {
                return false;
            }

            for (var i = 0; i < types.Length; i++)
            {
                if (types[i].Name != gtypes[i].Name)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
