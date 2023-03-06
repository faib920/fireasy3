// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using System.Reflection;

namespace Fireasy.Common.Emit
{
    /// <summary>
    /// 泛型类型参数。
    /// </summary>
    public class GenericTypeParameter
    {
        /// <summary>
        /// 初始化 <see cref="GenericTypeParameter"/> 类的新实例。
        /// </summary>
        /// <param name="name">参数名称。</param>
        /// <param name="baseType">所继承的类型</param>
        /// <param name="attr">特性。</param>
        /// <param name="contraintTypes">约束类型数组。</param>
        public GenericTypeParameter(string name, Type? baseType = null, GenericParameterAttributes attr = GenericParameterAttributes.None, params Type[] contraintTypes)
        {
            Name = name;
            BaseType = baseType;
            Attribute = attr;
            ConstraintTypes = contraintTypes;
        }

        /// <summary>
        /// 从指定的类型来创建泛型参数。
        /// </summary>
        /// <param name="type">指定的类型。</param>
        /// <returns></returns>
        public static GenericTypeParameter From(Type type)
        {
            return new GenericTypeParameter(type.Name, type.BaseType, type.GenericParameterAttributes, type.GetGenericParameterConstraints());
        }

        /// <summary>
        /// 获取参数名称。
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 获取所继承的类型。
        /// </summary>
        public Type? BaseType { get; }

        /// <summary>
        /// 获取约束类型数组。
        /// </summary>
        public Type[] ConstraintTypes { get; }

        /// <summary>
        /// 获取参数特性。
        /// </summary>
        public GenericParameterAttributes Attribute { get; }
    }
}
