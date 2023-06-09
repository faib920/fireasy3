﻿// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fireasy.Windows.Forms
{
    /// <summary>
    /// 一个下拉框，使用 <see cref="TreeList"/> 作为下拉组件。
    /// </summary>
    public class ComplexComboBox : PopupComboBox
    {
        /// <summary>
        /// 初始化 <see cref="ComplexComboBox"/> 类的新实例。
        /// </summary>
        public ComplexComboBox()
        {
            InitializeComponent();
            DropDownControl = TreeList;
        }

        /// <summary>
        /// 获取或设置作为下拉内容的 <see cref="TreeList"/> 对象。
        /// </summary>
        public TreeList TreeList { get; private set; }

        /// <summary>
        /// 获取选中的项。
        /// </summary>
        public new object? SelectedItem
        {
            get
            {
                if (TreeList.SelectedItems.Count == 0)
                {
                    return null;
                }

                return TreeList.SelectedItems[0];
            }
        }

        /// <summary>
        /// 获取选中项的文本。对应到 <see cref="TreeListItem"/> 的 Text。
        /// </summary>
        public override string SelectedText
        {
            get
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 获取选中项的值。对应到 <see cref="TreeListCell"/> 的 Value。ValueMember 与 <see cref="TreeListColumn"/> 的 DataKey 值匹配。
        /// </summary>
        public override object? SelectedValue
        {
            get
            {
                return base.SelectedValue;
            }
            set
            {
                if (TreeList.ShowCheckBoxes)
                {
                    base.SelectedValue = value;

                    if (value != null)
                    {
                        var texts = new List<string>();
                        if (value is object[] values)
                        {
                            CheckItem(TreeList.Items, values, texts);
                        }

                        SetText(string.Join(",", texts));
                    }
                    else
                    {
                        SetText(string.Empty);
                    }
                }
                else
                {
                    TreeList.SelectedItems.Clear();
                    if (!FindItem(TreeList.Items, value))
                    {
                        SetText(string.Empty);
                    }
                }
            }
        }

        protected override void OnPopupOpened()
        {
            TreeList.Focus();
        }

        /// <summary>
        /// 设置需要下拉显示的 <see cref="TreeList"/> 控件。
        /// </summary>
        /// <param name="treelist"></param>
        public void SetTreeList(TreeList treelist)
        {
            TreeList = treelist;
            DropDownControl = treelist;
        }

        private bool FindItem(TreeListItemCollection items, object? value)
        {
            if (value == null)
            {
                return false;
            }

            foreach (var item in items)
            {
                if (Equals(GetValue(item), value))
                {
                    item.Selected = true;
                    base.SelectedValue = value;
                    base.SelectedItem = item;
                    SetText(GetDisplayText(item));
                    ExpendAllParent(item);
                    return true;
                }

                if (FindItem(item.Items, value))
                {
                    return true;
                }
            }

            return false;
        }

        private void ExpendAllParent(TreeListItem item)
        {
            var parent = item.Parent;
            while (parent != null)
            {
                parent.Expended = true;
                parent = parent.Parent;
            }
        }

        private void CheckItem(TreeListItemCollection items, object[] values, List<string> texts)
        {
            foreach (var item in items)
            {
                if (values.Contains(GetValue(item)))
                {
                    item.Checked = true;
                    texts.Add(GetDisplayText(item));
                }

                CheckItem(item.Items, values, texts);
            }
        }

        private void InitializeComponent()
        {
            TreeList = new TreeList
            {
                Dock = DockStyle.Fill,
                HotTracking = true,
                ShowHeader = false
            };

            TreeList.Columns.Add(new TreeListColumn { Spring = true });
            TreeList.ItemClick += TreeList_ItemClick;
            TreeList.ItemCheckChanged += TreeList_ItemCheckChanged;
            TreeList.ShowGridLines = false;
        }

        void TreeList_ItemClick(object sender, TreeListItemEventArgs e)
        {
            if (TreeList.ShowCheckBoxes)
            {
                return;
            }

            SetItem(GetDisplayText(e.Item), GetValue(e.Item));

            HideDropDown();
        }

        void TreeList_ItemCheckChanged(object sender, TreeListItemEventArgs e)
        {
            if (!TreeList.ShowCheckBoxes)
            {
                return;
            }

            var sb = new StringBuilder();
            var array = new List<object>();

            foreach (var item in TreeList.CheckedItems)
            {
                if (sb.Length > 0)
                {
                    sb.Append(",");
                }

                sb.Append(GetDisplayText(item));
                array.Add(GetValue(item));
            }

            SetItem(sb.ToString(), array.Count == 0 ? null : array.ToArray());
        }

        private string GetDisplayText(TreeListItem item)
        {
            if (!string.IsNullOrEmpty(DisplayMember))
            {
                return item.Cells[DisplayMember].Text;
            }

            return item.Text;
        }

        private object GetValue(TreeListItem item)
        {
            if (!string.IsNullOrEmpty(ValueMember))
            {
                return item.Cells[ValueMember].Value;
            }

            return item.KeyValue;
        }
    }
}
