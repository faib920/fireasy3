// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

namespace Fireasy.Common.Emit
{
    /// <summary>
    /// 一个抽象类，动态构造器。
    /// </summary>
    public abstract class DynamicBuilder
    {
        /// <summary>
        /// 初始化 <see cref="DynamicBuilder"/> 类的新实例。
        /// </summary>
        protected DynamicBuilder()
        {
        }

        /// <summary>
        /// 初始化 <see cref="DynamicBuilder"/> 类的新实例。
        /// </summary>
        /// <param name="accessibility">访问修饰符。</param>
        /// <param name="modifier">类或成员修饰符。</param>
        protected DynamicBuilder(Accessibility accessibility, Modifier modifier)
        {
            Accessibility = accessibility;
            Modifier = modifier;
        }

        /// <summary>
        /// 获取访问修饰符。
        /// </summary>
        public Accessibility Accessibility { get; }

        /// <summary>
        /// 获取类或成员修饰符。
        /// </summary>
        public Modifier Modifier { get; }

        /// <summary>
        /// 获取或设置构造器的上下文对象。
        /// </summary>
        public BuildContext Context { get; protected set; }

        /// <summary>
        /// 使用自定义特性生成器设置此程序集的自定义特性。
        /// </summary>
        /// <typeparam name="T">自定义属性的类型。</typeparam>
        /// <param name="constructorArgs">自定义属性的构造函数的参数。</param>
        public void SetCustomAttribute<T>(params object[] constructorArgs) where T : Attribute
        {
            var types = constructorArgs == null ? new Type[0] : (from s in constructorArgs select s.GetType()).ToArray();
            var constructor = typeof(T).GetConstructor(types);
            if (constructor == null)
            {
                throw new DynamicBuildException($"未找到类型 {typeof(T)} 缺省的构造函数。");
            }

            SetCustomAttribute(new CustomAttributeBuilder(constructor, constructorArgs));
        }

        /// <summary>
        /// 使用自定义特性生成器设置此程序集的自定义特性。
        /// </summary>
        /// <param name="expression"></param>
        public void SetCustomAttribute(Expression<Func<Attribute>> expression)
        {
            SetCustomAttribute(CustomAttributeConstructorVisitor.Build(expression));
        }

        /// <summary>
        /// 使用自定义特性生成器设置此程序集的自定义特性。
        /// </summary>
        /// <param name="expression">一个 <see cref="Attribute"/> 的构造表达式。</param>
        public void SetCustomAttribute(Expression expression)
        {
            SetCustomAttribute(CustomAttributeConstructorVisitor.Build(expression));
        }

        /// <summary>
        /// 设置一个 <see cref="CustomAttributeBuilder"/> 对象到当前实例中。
        /// </summary>
        /// <param name="customBuilder">一个 <see cref="CustomAttributeBuilder"/> 对象。</param>
        protected abstract void SetCustomAttribute(CustomAttributeBuilder customBuilder);

        private class CustomAttributeConstructorVisitor : ExpressionVisitor
        {
            private ConstructorInfo? _constructor;
            private readonly List<object?> _constructorArgs = new();
            private readonly List<PropertyInfo> _properties = new();
            private readonly List<object?> _values = new();
            private VisitType _flags = VisitType.None;

            private enum VisitType
            {
                None,
                ConstructorArgument,
                MemberInit
            }

            public static CustomAttributeBuilder Build(Expression expression)
            {
                var visitor = new CustomAttributeConstructorVisitor();
                visitor.Visit(expression);

                if (visitor._constructor == null)
                {
                    throw new DynamicBuildException("未找到任何构造函数。");
                }
                if (visitor._properties.Count != visitor._values.Count)
                {
                    throw new DynamicBuildException("names 和 values 数组的长度不匹配。");
                }

                return new CustomAttributeBuilder(
                    visitor._constructor,
                    visitor._constructorArgs.ToArray(),
                    visitor._properties.ToArray(),
                    visitor._values.ToArray());
            }

            protected override Expression VisitMember(MemberExpression memberExp)
            {
                //值或引用
                var exp = (Expression)memberExp;
                if (memberExp.Type.IsValueType)
                {
                    exp = Expression.Convert(memberExp, typeof(object));
                }

                var lambda = Expression.Lambda<Func<object>>(exp);
                var fn = lambda.Compile();

                //转换为常量表达式
                return Visit(Expression.Constant(fn(), memberExp.Type));
            }

            protected override Expression VisitMemberInit(MemberInitExpression memberInitExp)
            {
                _flags = VisitType.MemberInit;
                var count = memberInitExp.Bindings.Count;
                for (var i = 0; i < count; i++)
                {
                    var pro = memberInitExp.Bindings[i].Member as PropertyInfo;
                    if (pro == null)
                    {
                        continue;
                    }

                    _properties.Add(pro);

                    switch (memberInitExp.Bindings[i].BindingType)
                    {
                        case MemberBindingType.Assignment:
                            if (memberInitExp.Bindings[i] is MemberAssignment assign &&
                                CheckArgumentExpression(assign.Expression))
                            {
                                Visit(assign.Expression);
                            }

                            break;
                    }
                }

                Visit(memberInitExp.NewExpression);
                return memberInitExp;
            }

            protected override Expression VisitNew(NewExpression newExp)
            {
                _flags = VisitType.ConstructorArgument;
                _constructor = newExp.Constructor;
                foreach (var arg in newExp.Arguments)
                {
                    if (CheckArgumentExpression(arg))
                    {
                        Visit(arg);
                    }
                }

                return newExp;
            }

            protected override Expression VisitConstant(ConstantExpression constExp)
            {
                if (_flags == VisitType.MemberInit)
                {
                    _values.Add(constExp.Value);
                }
                else if (_flags == VisitType.ConstructorArgument)
                {
                    _constructorArgs.Add(constExp.Value);
                }

                return constExp;
            }

            /// <summary>
            /// 检查表达式能否被正确解析，只有构造里的参数以及属性初始化表达式可以使用。
            /// </summary>
            /// <param name="expression">要检查的表达式。</param>
            /// <returns>能够被解析，则为 true。</returns>
            private bool CheckArgumentExpression(Expression expression)
            {
                if (expression.NodeType == ExpressionType.Constant ||
                    expression.NodeType == ExpressionType.MemberAccess)
                {
                    return true;
                }

                throw new DynamicBuildException($"表达式 {expression} 不能用来构造 NewExpression。");
            }
        }
    }
}
