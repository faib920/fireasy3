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
    /// 用于创建一个动态的接口。
    /// </summary>
    public class DynamicInterfaceBuilder : DynamicTypeBuilder
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context">上下文对象。</param>
        /// <param name="typeName">接口名称。</param>
        /// <param name="accessibility">接口的访问修饰符。</param>
        internal DynamicInterfaceBuilder(BuildContext context, string typeName, Accessibility accessibility) :
            base(context, typeName, accessibility, Modifier.Standard, null)
        {
        }

        /// <summary>
        /// 获取或设置动态类型所继承的类型。
        /// </summary>
        public override Type? BaseType
        {
            get
            {
                return null;
            }
            set
            {
                throw new DynamicBuildException("接口不允许设置父类类型。");
            }
        }

        /// <summary>
        /// 获取 <see cref="TypeAttributes"/>。
        /// </summary>
        /// <returns></returns>
        protected override TypeAttributes GetTypeAttributes()
        {
            return TypeAttributes.Interface | TypeAttributes.Abstract;
        }

        internal override PropertyAttributes GetPropertyAttributes()
        {
            return PropertyAttributes.HasDefault;
        }

        internal override MethodAttributes GetMethodAttributes()
        {
            return MethodAttributes.Virtual | MethodAttributes.Abstract;
        }

        /// <summary>
        /// 定义字段返回为 null。
        /// </summary>
        /// <param name="fieldName">字段的名称。</param>
        /// <param name="fieldType">字段的类型。</param>
        /// <param name="defaultValue">缺省值。</param>
        /// <param name="accessibility">访问修饰符。</param>
        /// <param name="modifier">修饰符。</param>
        /// <returns></returns>
        public override DynamicFieldBuilder DefineField(string fieldName, Type fieldType, object defaultValue = null, Accessibility accessibility = Accessibility.Private, Modifier modifier = Modifier.Standard)
        {
            return null;
        }
    }
}
