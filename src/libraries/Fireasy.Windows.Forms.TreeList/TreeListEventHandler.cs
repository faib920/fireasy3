// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Fireasy.Windows.Forms
{
    public delegate void TreeListColumnClickEventHandler(object sender, TreeListColumnClickEventArgs e);

    public delegate void TreeListCheckAllEventHandler(object sender, TreeListCheckAllEventArgs e);

    public delegate void TreeListItemDataBoundEventHandler(object sender, TreeListItemDataBoundEventArgs e);

    public delegate void TreeListCellDataBoundEventHandler(object sender, TreeListCellDataBoundEventArgs e);

    public delegate void TreeListItemClickEventHandler(object sender, TreeListItemEventArgs e);

    public delegate void TreeListItemDoubleClickEventHandler(object sender, TreeListItemEventArgs e);

    public delegate void TreeListItemSelectionChangedEventHandler(object sender, TreeListItemSelectionEventArgs e);

    public delegate void TreeListItemBeforeCollapseEventHandler(object sender, TreeListItemCancelEventArgs e);

    public delegate void TreeListItemBeforeExpandEventHandler(object sender, TreeListItemCancelEventArgs e);

    public delegate void TreeListItemAfterCollapseEventHandler(object sender, TreeListItemEventArgs e);

    public delegate void TreeListItemAfterExpandEventHandler(object sender, TreeListItemEventArgs e);

    public delegate void TreeListDemandLoadEventHandler(object sender, TreeListItemEventArgs e);

    public delegate void TreeListCellClickEventHandler(object sender, TreeListCellEventArgs e);

    public delegate void TreeListBeforeCellEditingEventHandler(object sender, TreeListBeforeCellEditingEventArgs e);

    public delegate void TreeListAfterCellEditedEventHandler(object sender, TreeListAfterCellEditedEventArgs e);

    public delegate void TreeListBeforeCellUpdatingEventHandler(object sender, TreeListBeforeCellUpdatingEventArgs e);

    public delegate void TreeListAfterCellUpdatedEventHandler(object sender, TreeListAfterCellUpdatedEventArgs e);

    public delegate void TreeListItemBeforeCheckedEventHandler(object sender, TreeListItemCancelEventArgs e);

    public delegate void TreeListItemAfterCheckedEventHandler(object sender, TreeListItemEventArgs e);

    public delegate void TreeListCellBeforeCheckedEventHandler(object sender, TreeListCellCancelEventArgs e);

    public delegate void TreeListCellAfterCheckedEventHandler(object sender, TreeListCellEventArgs e);

    public delegate void TreeListItemCheckChangeEventHandler(object sender, TreeListItemEventArgs e);

    public delegate void TreeListItemDragOverEventHandler(object sender, TreeListItemDragOverEventArgs e);

    public delegate void TreeListBeforeItemDragDownEventHandler(object sender, TreeListBeforeItemDragDownEventArgs e);

    public delegate void TreeListAfterItemDragDownEventHandler(object sender, TreeListAfterItemDragDownEventArgs e);

    public class TreeListItemSelectionEventArgs
    {
    }

    public class TreeListItemEventArgs
    {
        public TreeListItem Item { get; internal set; }
    }

    public class TreeListCellEventArgs
    {
        public TreeListCell Cell { get; internal set; }
    }

    public class TreeListColumnClickEventArgs
    {
        /// <summary>
        /// 获取所单击的列。
        /// </summary>
        public TreeListColumn Column { get; internal set; }

        /// <summary>
        /// 获取或设置是否允许排序。
        /// </summary>
        public bool Sortable { get; set; }

        /// <summary>
        /// 获取排序方式。
        /// </summary>
        public SortOrder Sorting { get; internal set; }
    }

    public class TreeListCheckAllEventArgs
    {
        /// <summary>
        /// 获取所单击的列。
        /// </summary>
        public TreeListColumn Column { get; internal set; }

        /// <summary>
        /// 获取或设置是否选中。
        /// </summary>
        public bool Checked { get; set; }
    }

    public class TreeListItemDataBoundEventArgs : TreeListItemEventArgs
    {
        public int Index { get; internal set; }

        public object ItemData { get; internal set; }
    }

    public class TreeListItemCancelEventArgs : TreeListItemEventArgs
    {
        public bool Cancel { get; set; }
    }

    public class TreeListCellCancelEventArgs : TreeListCellEventArgs
    {
        public bool Cancel { get; set; }
    }

    /// <summary>
    /// 单元格数据绑定的事件参数。
    /// </summary>
    public class TreeListCellDataBoundEventArgs : TreeListCellEventArgs
    {
        /// <summary>
        /// 获取绑定到行上的对象。
        /// </summary>
        public object ItemData { get; internal set; }

        /// <summary>
        /// 获取绑定到单元格的数据。
        /// </summary>
        public object Value { get; internal set; }
    }

    /// <summary>
    /// 单元格编辑时的事件参数。
    /// </summary>
    public class TreeListBeforeCellEditingEventArgs : TreeListCellEventArgs
    {
        /// <summary>
        /// 获取或设置是否取消编辑。
        /// </summary>
        public bool Cancel { get; set; }
    }

    /// <summary>
    /// 单元格编辑后的事件参数。
    /// </summary>
    public class TreeListAfterCellEditedEventArgs : TreeListCellEventArgs
    {
        /// <summary>
        /// 获取是否是按下了回车键结束的编辑。
        /// </summary>
        public bool EnterKey { get; internal set; }
    }

    /// <summary>
    /// 单元格更新时的事件参数。
    /// </summary>
    public class TreeListBeforeCellUpdatingEventArgs : TreeListCellEventArgs
    {
        /// <summary>
        /// 获取原来的值。
        /// </summary>
        public object OldValue { get; internal set; }

        /// <summary>
        /// 获取或设置新值。
        /// </summary>
        public object NewValue { get; set; }

        /// <summary>
        /// 获取或设置是否取消更新。
        /// </summary>
        public bool Cancel { get; set; }
    }

    /// <summary>
    /// 单元格更新后的事件参数。
    /// </summary>
    public class TreeListAfterCellUpdatedEventArgs : TreeListCellEventArgs
    {
        /// <summary>
        /// 获取原来的值。
        /// </summary>
        public object OldValue { get; internal set; }

        /// <summary>
        /// 获取或设置新值。
        /// </summary>
        public object NewValue { get; set; }

        /// <summary>
        /// 获取或设置是否是按下了回车键结束的编辑。
        /// </summary>
        public bool EnterKey { get; set; }
    }

    /// <summary>
    /// 行拖拽中的事件参数。
    /// </summary>
    public class TreeListItemDragOverEventArgs : TreeListItemEventArgs
    {
        /// <summary>
        /// 获取拖拽的项。
        /// </summary>
        public TreeListItem Source { get; internal set; }

        /// <summary>
        /// 获取拖拽的位置
        /// </summary>
        public DragPosition Position { get; internal set; }

        /// <summary>
        /// 获取或设置是否可拖拽。
        /// </summary>
        public bool Dragable { get; set; }
    }

    /// <summary>
    /// 行拖拽落下的事件参数。
    /// </summary>
    public class TreeListBeforeItemDragDownEventArgs : TreeListItemEventArgs
    {
        /// <summary>
        /// 获取拖拽的项。
        /// </summary>
        public TreeListItem Source { get; internal set; }

        /// <summary>
        /// 获取拖拽的位置
        /// </summary>
        public DragPosition Position { get; internal set; }

        /// <summary>
        /// 获取或设置是否取消拖拽。
        /// </summary>
        public bool Cancel { get; set; }
    }

    /// <summary>
    /// 行拖拽落下的事件参数。
    /// </summary>
    public class TreeListAfterItemDragDownEventArgs : TreeListItemEventArgs
    {
        /// <summary>
        /// 获取拖拽的项。
        /// </summary>
        public TreeListItem Source { get; internal set; }

        /// <summary>
        /// 获取拖拽的位置
        /// </summary>
        public DragPosition Position { get; internal set; }
    }
}
