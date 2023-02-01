// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using System.ComponentModel;
using System.Dynamic;

namespace Fireasy.Common.Dynamic
{
    /// <summary>
    /// 为动态类型类型提供补充元数据。无法继承此类。
    /// </summary>
    public sealed class DynamicObjectTypeDescriptionProvider : TypeDescriptionProvider
    {
        private static readonly TypeDescriptionProvider Default = TypeDescriptor.GetProvider(typeof(IDynamicMetaObjectProvider));
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// 初始化 <see cref="DynamicObjectTypeDescriptionProvider"/> 类的新实例。
        /// </summary>
        public DynamicObjectTypeDescriptionProvider(IServiceProvider serviceProvider)
            : base(Default)
        {
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// 获取指定类型和对象的自定义类型说明符。
        /// </summary>
        /// <param name="objectType">对象的类型。</param>
        /// <param name="instance">该类型的实例。</param>
        /// <returns></returns>
        public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance)
        {
            var defaultDescriptor = base.GetTypeDescriptor(objectType, instance);

            return instance == null ? defaultDescriptor :
                new DynamicTypeDescriptor(_serviceProvider, (IDynamicMetaObjectProvider)instance);
        }
    }
}
