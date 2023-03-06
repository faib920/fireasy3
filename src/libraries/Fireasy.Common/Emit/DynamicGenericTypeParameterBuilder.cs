// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using System.Reflection.Emit;

namespace Fireasy.Common.Emit
{
    /// <summary>
    /// 动态泛型参数构造器。
    /// </summary>
    public class DynamicGenericTypeParameterBuilder
    {
        /// <summary>
        /// 初始化 <see cref="DynamicGenericTypeParameterBuilder"/> 类的新实例。
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="builder"></param>
        public DynamicGenericTypeParameterBuilder(GenericTypeParameter parameter, GenericTypeParameterBuilder builder)
        {
            if (parameter.BaseType != null && parameter.BaseType != typeof(object))
            {
                builder.SetBaseTypeConstraint(parameter.BaseType);
            }

            if (parameter.ConstraintTypes != null)
            {
                builder.SetInterfaceConstraints(parameter.ConstraintTypes);
            }

            builder.SetGenericParameterAttributes(parameter.Attribute);

            GenerateTypeParameterBuilder = builder;
        }

        /// <summary>
        /// 获取当前的 <see cref="GenericTypeParameterBuilder"/>。
        /// </summary>
        /// <returns></returns>
        public GenericTypeParameterBuilder GenerateTypeParameterBuilder { get; }
    }
}
