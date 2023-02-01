// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;
using System.Dynamic;

namespace Fireasy.Common.Dynamic
{
    /// <summary>
    /// 为 <see cref="ExpandoObject"/> 类型提供信息补充。无法继承此类。
    /// </summary>
    public sealed class DynamicTypeDescriptor : ICustomTypeDescriptor
    {
        private readonly IDynamicMetaObjectProvider _instance;
        private readonly IDynamicExpansibility _dynamicManager;

        /// <summary>
        /// 初始化 <see cref="DynamicTypeDescriptor"/> 类的新实例。
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <param name="instance"></param>
        public DynamicTypeDescriptor(IServiceProvider serviceProvider, IDynamicMetaObjectProvider instance)
        {
            _dynamicManager = serviceProvider.GetRequiredService<IDynamicExpansibility>();
            _instance = instance;
        }

        string ICustomTypeDescriptor.GetComponentName()
        {
            return TypeDescriptor.GetComponentName(this, true);
        }

        EventDescriptor ICustomTypeDescriptor.GetDefaultEvent()
        {
            return TypeDescriptor.GetDefaultEvent(this, true);
        }

        string ICustomTypeDescriptor.GetClassName()
        {
            return TypeDescriptor.GetClassName(this, true);
        }

        EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes)
        {
            return TypeDescriptor.GetEvents(this, attributes, true);
        }

        EventDescriptorCollection ICustomTypeDescriptor.GetEvents()
        {
            return TypeDescriptor.GetEvents(this, true);
        }

        TypeConverter ICustomTypeDescriptor.GetConverter()
        {
            return TypeDescriptor.GetConverter(this, true);
        }

        object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd)
        {
            return _instance;
        }

        AttributeCollection ICustomTypeDescriptor.GetAttributes()
        {
            return TypeDescriptor.GetAttributes(this, true);
        }

        object ICustomTypeDescriptor.GetEditor(Type editorBaseType)
        {
            return TypeDescriptor.GetEditor(this, editorBaseType, true);
        }

        PropertyDescriptor? ICustomTypeDescriptor.GetDefaultProperty()
        {
            return null;
        }

        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
        {
            return ((ICustomTypeDescriptor)this).GetProperties(new Attribute[0]);
        }

        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes)
        {
            return new PropertyDescriptorCollection(
                ((IDictionary<string, object>)_instance).Keys
                .Select(x => new DynamicPropertyDescriptor(_dynamicManager!, _instance, x))
                .ToArray());
        }

        private class DynamicPropertyDescriptor : PropertyDescriptor
        {
            private readonly IDynamicExpansibility _dynamicManager;
            private readonly IDynamicMetaObjectProvider _instance;
            private readonly string _name;

            public DynamicPropertyDescriptor(IDynamicExpansibility dynamicManager, IDynamicMetaObjectProvider instance, string name)
                : base(name, null)
            {
                _dynamicManager = dynamicManager;
                _instance = instance;
                _name = name;
            }

            public override Type? PropertyType
            {
                get
                {
                    return _dynamicManager.TryGetMember(_instance, _name, out var value) ? value!.GetType() : null;
                }
            }

            public override void SetValue(object component, object value)
            {
                _dynamicManager.TrySetMember(_instance, _name, value);
            }

            public override object? GetValue(object component)
            {
                _dynamicManager.TryGetMember(_instance, _name, out var value);
                return value;
            }

            public override bool IsReadOnly
            {
                get
                {
                    return false;
                }
            }

            public override Type? ComponentType
            {
                get { return null; }
            }

            public override bool CanResetValue(object component)
            {
                return false;
            }

            public override void ResetValue(object component)
            {
            }

            public override bool ShouldSerializeValue(object component)
            {
                return false;
            }

            public override string Category
            {
                get { return string.Empty; }
            }

            public override string Description
            {
                get { return string.Empty; }
            }
        }
    }
}
