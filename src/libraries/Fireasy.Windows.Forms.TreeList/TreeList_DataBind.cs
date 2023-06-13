// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Fireasy.Common.Extensions;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;

namespace Fireasy.Windows.Forms
{
    public partial class TreeList
    {
        private object _dataSource;
        private Dictionary<Type, ObjectBindingDefinition>? _bindingCache = null;

        /// <summary>
        /// 将一个数据对象绑定到 <see cref="TreeList"/>
        /// </summary>
        /// <param name="dataSource"></param>
        public void DataBind(object dataSource)
        {
            if (dataSource == null)
            {
                return;
            }

            _dataSource = dataSource;
            _bindingCache = null;

            BeginUpdate();

            var selectKeyValues = GetSelectedItems();

            Items.Clear();
            Groups.Clear();
            SelectedItems.InternalClear();
            CheckedItems.InternalClear();

            if (dataSource == null)
            {
                EndUpdate();
                return;
            }

            if (dataSource is DataSet dataSet)
            {
                if (dataSet.Tables.Count > 0)
                {
                    BindDataTable(dataSet.Tables[0]);
                }
            }
            if (dataSource is DataTable dataTable)
            {
                BindDataTable(dataTable);
            }
            else if (dataSource is IEnumerable enumerable)
            {
                BindEnumerable(enumerable);
            }

            EndUpdate();

            ReSelectItems(selectKeyValues);
        }

        /// <summary>
        /// 获取或设置数据源。
        /// </summary>
        public object DataSource
        {
            get { return _dataSource; }
            set
            {
                _dataSource = value;
                DataBind(_dataSource);
            }
        }

        /// <summary>
        /// 把选中项记录下来。
        /// </summary>
        /// <returns></returns>
        private object[] GetSelectedItems()
        {
            if (KeepSelectedItems && !string.IsNullOrEmpty(KeyField))
            {
                return SelectedItems.Where(s => s.KeyValue != null).Select(s => s.KeyValue).ToArray();
            }

            return null;
        }

        /// <summary>
        /// 重新选中选择项。
        /// </summary>
        /// <param name="selectKeyValues"></param>
        private void ReSelectItems(object[] selectKeyValues)
        {
            if (selectKeyValues != null)
            {
                foreach (var vitem in _virMgr.Items)
                {
                    if (vitem.Item is TreeListItem item &&
                        item.KeyValue != null &&
                        selectKeyValues.FirstOrDefault(s => s.Equals(item.KeyValue)) != null)
                    {
                        item.Selected = true;
                    }
                }
            }
        }

        /// <summary>
        /// 绑定DataTable对象。
        /// </summary>
        /// <param name="table"></param>
        private void BindDataTable(DataTable table)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// 绑定枚举类型的数据源。
        /// </summary>
        /// <param name="enumerable"></param>
        private void BindEnumerable(IEnumerable enumerable)
        {
            var index = 0;
            foreach (var item in enumerable)
            {
                if (item == null)
                {
                    continue;
                }

                var definition = GetBindingDefinition(item);

                var listitem = new TreeListItem();
                listitem.DataItem = item;
                Items.Add(listitem);

                BindListItem(listitem, definition, index, true);

                RaiseItemDataBoundEvent(listitem, item, index++);
            }
        }

        internal void BindListItem(TreeListItem listitem, bool initialize)
        {
            if (listitem.DataItem == null)
            {
                return;
            }

            var definition = GetBindingDefinition(listitem.DataItem);
            BindListItem(listitem, definition, -1, initialize);
        }

        internal void UpdateListCellValue(TreeListCell cell, object? value)
        {
            if (!AllowUpdateDataItem)
            {
                return;
            }

            if (cell.Item.DataItem == null || string.IsNullOrEmpty(cell.Column.DataKey))
            {
                return;
            }

            var definition = GetBindingDefinition(cell.Item.DataItem);
            if (definition.Properties.TryGetValue(cell.Column.DataKey, out var property) && property != null)
            {
                property.SetValue(cell.Item.DataItem, value.To(property.PropertyType));

                RaiseCellDataUpdatedEvent(cell, cell.Item.DataItem, value);
            }
        }

        private void BindListItem(TreeListItem listitem, ObjectBindingDefinition definition, int index, bool initialize)
        {
            if (definition.PrimaryProperty != null)
            {
                listitem.KeyValue = definition.PrimaryProperty.GetValue(listitem.DataItem);
            }

            if (initialize && listitem.DataItem is INotifyPropertyChanged npc)
            {
                npc.PropertyChanged += (o, e) =>
                {
                    if (definition.Properties.TryGetValue(e.PropertyName, out var property))
                    {
                        var value = property.GetValue(listitem.DataItem);
                        listitem.Cells[property.Name].Value = value;
                        RaiseCellDataUpdatedEvent(listitem.Cells[property.Name], listitem.DataItem, value);
                    }
                };
            }

            foreach (var property in definition.Properties)
            {
                if (property.Value == null)
                {
                    listitem.Cells[property.Key].Value = null;
                    continue;
                }
                else
                {
                    var value = property.Value.GetValue(listitem.DataItem);
                    listitem.Cells[property.Key].Value = value;
                    RaiseCellDataBoundEvent(listitem.Cells[property.Key], listitem.DataItem!, value);
                }
            }
        }

        /// <summary>
        /// 获取能够绑定到 <see cref="TreeList"/> 上的数据对象的属性集合。
        /// </summary>
        /// <param name="element">序列中的元素。</param>
        /// <returns></returns>
        internal ObjectBindingDefinition GetBindingDefinition(object element)
        {
            if (_bindingCache == null)
            {
                _bindingCache = new Dictionary<Type, ObjectBindingDefinition>();
            }

            if (!_bindingCache.TryGetValue(element.GetType(), out var definition))
            {
                var binding = new Dictionary<string, PropertyDescriptor>();
                var properties = new List<PropertyDescriptor>();

                foreach (PropertyDescriptor pd in TypeDescriptor.GetProperties(element))
                {
                    properties.Add(pd);
                }

                var primary = FindProperty(properties, KeyField);

                foreach (var column in Columns.Where(s => !string.IsNullOrEmpty(s.DataKey)))
                {
                    binding.Add(column.DataKey, FindProperty(properties, column.DataKey));
                }

                definition = new ObjectBindingDefinition(primary, binding);

                _bindingCache.Add(element.GetType(), definition);
            }

            return definition;
        }

        private PropertyDescriptor FindProperty(List<PropertyDescriptor> properties, string name)
        {
            return string.IsNullOrEmpty(name) ? null :
                properties.Find(s => s.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        internal class ObjectBindingDefinition
        {
            public ObjectBindingDefinition(PropertyDescriptor primary, Dictionary<string, PropertyDescriptor> properties)
            {
                PrimaryProperty = primary;
                Properties = properties;
            }

            public PropertyDescriptor PrimaryProperty { get; set; }

            public Dictionary<string, PropertyDescriptor> Properties { get; set; }
        }
    }
}
