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
    /// 
    /// </summary>
    public sealed class DynamicDescriptionSupporter
    {
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// 初始化 <see cref="DynamicDescriptionSupporter"/> 的新实例。
        /// </summary>
        /// <param name="serviceProvider"></param>
        public DynamicDescriptionSupporter(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;

            AddDefaultDynamicProvider();
        }

        /// <summary>
        /// 添加一个动态类型的自定义类型说明。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void AddDynamicProvider<T>() where T : IDynamicMetaObjectProvider
        {
            TypeDescriptor.AddProvider(new DynamicObjectTypeDescriptionProvider(_serviceProvider), typeof(T));
        }

        /// <summary>
        /// 添加默认的动态类型的自定义类型说明。
        /// </summary>
        private void AddDefaultDynamicProvider()
        {
            AddDynamicProvider<DynamicObject>();
            AddDynamicProvider<ExpandoObject>();
            AddDynamicProvider<DynamicExpandoObject>();
        }
    }
}
