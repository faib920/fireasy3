// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Fireasy.Windows.Forms
{
    public partial class TreeList
    {
        /// <summary>
        /// 测试鼠标经过的位置的对象。
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="eventType"></param>
        /// <returns></returns>
        protected TreeListHitTestInfo HitTest(int x, int y, TreeListHitTestEventType eventType)
        {
            if (_bound.ColumnBound.Contains(x, y))
            {
                return HitTestColumn(x, y);
            }

            if (_bound.AvlieBound.Contains(x, y))
            {
                return HitTestItem(x, y);
            }

            return null;
        }

        /// <summary>
        /// 检测鼠标经过的地方的 <see cref="TreeListColumn"/>，如果是两个 <see cref="TreeListColumn"/> 的中间经，则为宽度调整线。
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private TreeListHitTestInfo HitTestColumn(int x, int y)
        {
            const int SIZE_WIDTH = 4;
            var workRect = _bound.ColumnBound;
            var x1 = workRect.X - GetOffsetLeft();
            foreach (var column in Columns)
            {
                if (column.Hidden)
                {
                    continue;
                }

                var rect = new Rectangle(x1, workRect.Top, column.Width, HeaderHeight);
                if (column.Index == 0 && ShowCheckAllBoxOnHeader)
                {
                    var h = (rect.Height - 15) / 2;
                    var checkRect = new Rectangle(rect.X + 2, rect.Top + h, 15, 15);
                    if (checkRect.Contains(x, y))
                    {
                        return new TreeListHitTestInfo(TreeListHitTestType.ColumnCheckAll, column, checkRect);
                    }
                }

                //如果列的宽度可调，则矩形框的宽度需要缩小4个像素，以便检测调整线
                if (!column.Fixed)
                {
                    rect.Inflate(-SIZE_WIDTH, 0);
                }

                if (rect.Contains(x, y))
                {
                    //恢复矩形框的宽度
                    if (!column.Fixed)
                    {
                        rect.Inflate(SIZE_WIDTH, 0);
                    }

                    return new TreeListHitTestInfo(TreeListHitTestType.Column, column, rect);
                }

                //检测是不是两列中间的宽度调整线
                var sizeRect = new Rectangle(rect.Right - SIZE_WIDTH, workRect.Top, SIZE_WIDTH * 2, HeaderHeight);
                if (sizeRect.Contains(x, y) && !column.Fixed)
                {
                    return new TreeListHitTestInfo(TreeListHitTestType.ColumnSize, column, rect);
                }

                x1 += column.Width;
            }

            return new TreeListHitTestInfo(TreeListHitTestType.Column);
        }

        private TreeListHitTestInfo HitTestGroup(int x, int y)
        {
            return null;
        }

        /// <summary>
        /// 提供座标 x 和 y 处的 <see cref="TreeListItem"/> 信息。
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private TreeListHitTestInfo HitTestItem(int x, int y)
        {
            var itemHeight = GetAdjustItemHeight();
            var totalWidth = GetColumnTotalWidth();
            if (ShowRowNumber)
            {
                totalWidth += RowNumberWidth;
            }

            var columnHeight = ShowHeader ? HeaderHeight : 0;

            //取得当前行的索引号
            var index = (y - columnHeight + GetOffsetTop()) / itemHeight;

            //判断索引是否有效，以及是否超出右边
            if (index < 0 || index > _virMgr.Items.Count - 1 || x > totalWidth - GetOffsetLeft())
            {
                return new TreeListHitTestInfo(TreeListHitTestType.Item);
            }

            var item = _virMgr.Items[index];
            if (item.ItemType == ItemType.Group)
            {
                return HitTestGroup(x, y);
            }

            var x1 = _bound.ItemBound.X - GetOffsetLeft();

            //修正y座标
            var y1 = _bound.ItemBound.Y + index * itemHeight - GetOffsetTop();
            var tw = GetColumnTotalWidth();

            var rect = new Rectangle(x1, y1, tw, ItemHeight);

            if (rect.Contains(x, y))
            {
                //测试是否是 +/-
                var info = HitTestItemPlusMinus(item, rect, x, y);
                if (info != null)
                {
                    return info;
                }

                //测试是否为复选框
                info = HitTestItemCheckbox(item, rect, x, y);
                if (info != null)
                {
                    return info;
                }

                //测试是否是Cell
                info = HitTestCell(item, rect, x, y);
                if (info != null)
                {
                    return info;
                }

                return new TreeListHitTestInfo(TreeListHitTestType.Item, item, rect);
            }

            if (ShowRowNumber && new Rectangle(_bound.RowNumberBound.X, y1, RowNumberWidth, ItemHeight).Contains(x, y))
            {
                return new TreeListHitTestInfo(TreeListHitTestType.Item, item, rect);
            }

            return null;
        }

        /// <summary>
        /// 提供座标 x 和 y 处的 +/- 信息。
        /// </summary>
        /// <param name="item"></param>
        /// <param name="rect"><see cref="TreeListItem"/> 的区域。</param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private TreeListHitTestInfo HitTestItemPlusMinus(VirtualTreeListItem item, Rectangle rect, int x, int y)
        {
            var sitem = (TreeListItem)item.Item;

            //是否可以显示 +/-
            if (ShowPlusMinus && (sitem.ShowExpanded || sitem.Items.Count > 0))
            {
                var x1 = rect.Left + sitem.Level * Indent;

                //+/-符号的区域
                var mrect = new Rectangle(x1, rect.Top, 16, ItemHeight).Middle(12, 12);

                //获取第一列的区域
                var firstColumn = GetFirstColumn();
                var crect = GetColumnBound(firstColumn);

                //获取当前Cell的区域
                var srect = new Rectangle(crect.X, rect.Top, crect.Width, ItemHeight);

                if (mrect.Contains(x, y) && srect.IntersectsWith(mrect))
                {
                    return new TreeListHitTestInfo(TreeListHitTestType.PlusMinus, item, mrect);
                }
            }

            return null;
        }

        /// <summary>
        /// 提供座标 x 和 y 处的复选框信息。
        /// </summary>
        /// <param name="item"></param>
        /// <param name="rect"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private TreeListHitTestInfo HitTestItemCheckbox(VirtualTreeListItem item, Rectangle rect, int x, int y)
        {
            if (ShowCheckBoxes)
            {
                var sitem = (TreeListItem)item.Item;

                //如果项不可用，则单选按钮不可用
                if (!sitem.Enabled || !sitem.ShowBox)
                {
                    return null;
                }

                var x1 = rect.Left + sitem.Level * Indent;

                //复选框区域
                var mrect = new Rectangle(x1, rect.Top, 15, ItemHeight).Middle(15, 15);
                if (ShowPlusMinus)
                {
                    mrect.Offset(16, 0);
                }

                //获取第一列的区域
                var firstColumn = GetFirstColumn();
                var crect = GetColumnBound(firstColumn);

                //获取当前Cell的区域
                var srect = new Rectangle(crect.X, rect.Top, crect.Width, ItemHeight);

                if (mrect.Contains(x, y) && srect.IntersectsWith(mrect))
                {
                    return new TreeListHitTestInfo(TreeListHitTestType.Checkbox, item, mrect);
                }
            }

            return null;
        }

        private TreeListHitTestInfo HitTestCell(VirtualTreeListItem vitem, Rectangle rect, int x, int y)
        {
            var workRect = _bound.ItemBound;
            var x1 = workRect.X - GetOffsetLeft();
            var item = (TreeListItem)vitem.Item;
            foreach (var column in Columns)
            {
                if (column.Index > item.Cells.Count)
                {
                    return null;
                }

                if (column.Hidden)
                {
                    continue;
                }

                var rect1 = new Rectangle(x1, rect.Y, column.Width, ItemHeight);

                if (rect1.Contains(x, y))
                {
                    var owner = new TreeListHitTestInfo(TreeListHitTestType.Item, vitem, rect);
                    if (column.Index <= item.Cells.Count - 1)
                    {
                        return new TreeListHitTestInfo(TreeListHitTestType.Cell, item.Cells[column.Index], rect1) { Owner = owner };
                    }
                    else
                    {
                        return owner;
                    }
                }

                x1 += column.Width;
            }

            return null;
        }

        /// <summary>
        /// 处理当前鼠标经过的对象。
        /// </summary>
        /// <param name="info"></param>
        /// <param name="drawState"></param>
        private void ProcessHitTestInfo(TreeListHitTestInfo info, DrawState drawState)
        {
            if (info.Bounds.IsEmpty)
            {
                return;
            }

            switch (info.HitTestType)
            {
                case TreeListHitTestType.Column:
                    var column = (TreeListColumn)info.Element;
                    using (var graphics = CreateGraphics())
                    {
                        var drawArgs = new TreeListColumnRenderEventArgs(column, graphics, info.Bounds)
                        {
                            DrawState = drawState
                        };
                        graphics.KeepClip(_bound.ColumnBound, () => Renderer.DrawColumnHeader(drawArgs));
                    }

                    break;
                case TreeListHitTestType.ColumnSize:
                    Cursor = Cursors.VSplit;
                    break;
                case TreeListHitTestType.Cell:

                    var cell = (TreeListCell)info.Element;
                    var rect = GetCellTextRectangle(cell, info.Bounds);
                    if (drawState == DrawState.Hot)
                    {
                        ShowToolTip(cell, rect);
                    }
                    else if (drawState == DrawState.Pressed)
                    {
                        HideToolTip();
                    }

                    if (info.Owner != null)
                    {
                        ProcessHitTestInfo(info.Owner, drawState);
                    }
                    break;
                case TreeListHitTestType.Item:
                    if (HandCursor)
                    {
                        Cursor = Cursors.Hand;
                    }

                    if (drawState == DrawState.Pressed || !HotTracking)
                    {
                        return;
                    }

                    var vitem = (VirtualTreeListItem)info.Element;
                    var item = (TreeListItem)vitem.Item;
                    if (item.Selected)
                    {
                        return;
                    }

                    using (var graphics = CreateGraphics())
                    {
                        var drawArgs = new TreeListItemRenderEventArgs(item, graphics, info.Bounds)
                        {
                            DrawState = drawState,
                            Alternate = vitem.Index % 2 != 0
                        };
                        DrawItem(drawArgs);
                    }

                    break;
            }
        }

        /// <summary>
        /// 处理鼠标单击时 <see cref="TreeListHitTestInfo"/> 的处理。
        /// </summary>
        /// <param name="info"></param>
        /// <param name="eventType"></param>
        private void ProcessHitTestInfoClick(TreeListHitTestInfo info, TreeListHitTestEventType eventType)
        {
            if (info.Bounds.IsEmpty)
            {
                return;
            }

            if (eventType == TreeListHitTestEventType.MouseUp &&
                info.HitTestType == TreeListHitTestType.Column)
            {
                ProcessColumnClick(info);
            }
            else if (eventType == TreeListHitTestEventType.MouseUp &&
                info.HitTestType == TreeListHitTestType.ColumnCheckAll)
            {
                ProcessColumnCheckAllClick(info);
            }
            else if (eventType == TreeListHitTestEventType.MouseDown)
            {
                switch (info.HitTestType)
                {
                    case TreeListHitTestType.Cell:
                        ProcessCellClick(info);
                        break;
                    case TreeListHitTestType.Item:
                        ProcessItemClick(info);
                        break;
                    case TreeListHitTestType.PlusMinus:
                        ProcessPlusMinusClick(info);
                        break;
                    case TreeListHitTestType.Checkbox:
                        ProcessItemCheckedChange(info);
                        break;
                }
            }
        }

        private void ProcessHitTestInfoDragOver(TreeListHitTestInfo info, Point point)
        {
            if (info == null || info.Element == null ||
                info.HitTestType != TreeListHitTestType.Cell ||
                (_lastHoverHitInfo != null && _lastHoverHitInfo.Equals(info)))
            {
                return;
            }

            if (!info.Equals(_lastDragingHitInfo))
            {
                if (_lastDragingHitInfo != null)
                {
                    DrawItem((_lastDragingHitInfo.Element as TreeListCell).Item, DrawState.Normal, _lastDragingHitInfo.Owner.Bounds);
                }

                _lastDragingHitInfo = info;

                ProcessHitTestInfoDragOver(point);
            }
            else
            {
                ProcessHitTestInfoDragOver(point);
            }
        }

        private void ProcessHitTestInfoDragOver(Point point)
        {
            var current = (_lastHoverHitInfo.Element as TreeListCell).Item;
            var target = (_lastDragingHitInfo.Element as TreeListCell).Item;

            var drawStatus = GetDrawDragStatus(_lastDragingHitInfo, point);
            var dragdownable = GetDragdownable(current, target, drawStatus);

            if (_lastDragStatus != drawStatus)
            {
                var eventArgs = new TreeListItemDragOverEventArgs { Item = target, Source = current, Dragable = dragdownable };

                eventArgs.Position = drawStatus == DrawState.Drag ? DragPosition.Children : (drawStatus == DrawState.DragTop ? DragPosition.Before : DragPosition.After);
                OnItemDragOver(eventArgs);
                Cursor = eventArgs.Dragable ? Cursors.SizeAll : Cursors.No;
                _lastDragdownable = eventArgs.Dragable;

                DrawItem(target, drawStatus, _lastDragingHitInfo.Owner.Bounds);
                _lastDragStatus = drawStatus;
            }
        }

        private void ProcessItemDragDown()
        {
            var current = (_lastHoverHitInfo.Element as TreeListCell).Item;
            var target = (_lastDragingHitInfo.Element as TreeListCell).Item;

            DrawItem(target, DrawState.Normal, _lastDragingHitInfo.Owner.Bounds);

            _lastDragingHitInfo = null;
            Cursor = Cursors.Default;

            if (!_lastDragdownable)
            {
                return;
            }

            var eventArgs = new TreeListBeforeItemDragDownEventArgs { Item = target, Source = current };
            eventArgs.Position = _lastDragStatus == DrawState.Drag ? DragPosition.Children : (_lastDragStatus == DrawState.DragTop ? DragPosition.Before : DragPosition.After);
            OnBeforeItemDragDown(eventArgs);

            var currentItems = current.Parent == null ? Items : current.Parent.Items;
            var targetItems = target.Parent == null ? Items : target.Parent.Items;

            if (eventArgs.Cancel)
            {
                return;
            }

            if (eventArgs.Position == DragPosition.Children)
            {
                currentItems.Remove(current);
                target.Items.Add(current);
                target.Expended = true;
                current.Parent = target;
                current.SetLevel(target.Level + 1);
            }
            else if (eventArgs.Position == DragPosition.Before)
            {
                if (current.Parent != target.Parent)
                {
                    currentItems.Remove(current);
                    targetItems.Insert(target.Index, current);
                    current.Parent = target.Parent;
                    current.SetLevel(target.Parent == null ? 0 : target.Level);
                }
                else if (current.Index + 1 != target.Index)
                {
                    currentItems.RemoveAt(current.Index);
                    currentItems.Insert(target.Index, current);
                }
            }
            else if (eventArgs.Position == DragPosition.After)
            {
                if (current.Parent != target.Parent)
                {
                    currentItems.Remove(current);
                    targetItems.Insert(target.Index + 1, current);
                    current.Parent = target.Parent;
                    current.SetLevel(target.Parent == null ? 0 : target.Level);
                }
                else if (current.Index != target.Index + 1)
                {
                    currentItems.RemoveAt(current.Index);
                    currentItems.Insert(target.Index + 1, current);
                }
            }

            current.Selected = true;

            UpdateItems();

            var eventArgs1 = new TreeListAfterItemDragDownEventArgs { Item = target, Source = current };
            eventArgs1.Position = eventArgs1.Position;
            OnAfterItemDragDown(eventArgs1);
        }

        private bool GetDragdownable(TreeListItem current, TreeListItem target, DrawState drawStatus)
        {
            var dragdownable = current.Parent == target ? drawStatus != DrawState.Drag : true;
            if (dragdownable)
            {
                var parent = target.Parent;
                while (parent != null)
                {
                    if (parent == current)
                    {
                        dragdownable = false;
                        break;
                    }

                    parent = parent.Parent;
                }
            }

            return dragdownable;
        }

        private DrawState GetDrawDragStatus(TreeListHitTestInfo info, Point point)
        {
            var target = (info.Element as TreeListCell);

            var rect = info.Owner.Bounds;
            var offset = ItemHeight / 4;

            if (point.Y >= rect.Bottom - offset)
            {
                return DrawState.DragBottom;
            }
            else if (point.Y <= rect.Top + offset)
            {
                return DrawState.DragTop;
            }
            else
            {
                return DrawState.Drag;
            }
        }

        /// <summary>
        /// 处理列头单击。
        /// </summary>
        /// <param name="info"></param>
        private void ProcessColumnClick(TreeListHitTestInfo info)
        {
            if (_lastHoverHitInfo != null &&
                _lastHoverHitInfo.HitTestType == TreeListHitTestType.Column &&
                _lastHoverHitInfo.Element != null)
            {
                var column = (TreeListColumn)info.Element;

                HideEditor();

                if (Sortable && column.Sortable)
                {
                    if (_sortedColumn != column)
                    {
                        _sortedOrder = SortOrder.Ascending;
                    }
                    else
                    {
                        if (_sortedOrder == SortOrder.Ascending)
                        {
                            _sortedOrder = SortOrder.Descending;
                        }
                        else
                        {
                            _sortedOrder = SortOrder.Ascending;
                        }
                    }

                    _sortedColumn = column;

                    if (RaiseColumnClickEvent(column, _sortedOrder))
                    {
                        if (Groups.Count == 0)
                        {
                            Items.Sort(++_sortVersion, column, _sortedOrder);
                        }
                        else
                        {
                            Groups.Sort(++_sortVersion, column, _sortedOrder);
                        }
                        UpdateItems();
                    }
                }
            }
        }

        /// <summary>
        /// 处理列头单击。
        /// </summary>
        /// <param name="info"></param>
        private void ProcessColumnCheckAllClick(TreeListHitTestInfo info)
        {
            if (_lastHoverHitInfo != null &&
                _lastHoverHitInfo.HitTestType == TreeListHitTestType.ColumnCheckAll &&
                _lastHoverHitInfo.Element != null)
            {
                var column = (TreeListColumn)info.Element;
                column.TreeList.CheckAllChecked = !column.TreeList.CheckAllChecked;
                Invalidate(_bound.ColumnBound);
                RaiseCheckAllChangedEvent(column, column.TreeList.CheckAllChecked);
            }
        }

        /// <summary>
        /// 处理行单击。
        /// </summary>
        /// <param name="info"></param>
        private void ProcessItemClick(TreeListHitTestInfo info)
        {
            var item = (TreeListItem)((VirtualTreeListItem)info.Element).Item;

            //按着ctrol切换选中状态
            if (_controlPressed)
            {
                SelectItem(item, !item.Selected, false);
            }
            else if (_shiftPressed && _lastRowIndex != -1)
            {
                if (_lastRowIndex > _virMgr.Items.Count - 1)
                {
                    return;
                }

                for (var i = SelectedItems.Count - 1; i >= 0; i--)
                {
                    SelectedItems[i].SetSelected(false);
                }

                SelectedItems.InternalClear();
                Invalidate(_bound.ItemBound);

                var start = _lastRowIndex;
                var end = item.Index;
                if (start > end)
                {
                    start = end;
                    end = _lastRowIndex;
                }

                for (var i = start; i <= end; i++)
                {
                    if (!(_virMgr.Items[i].Item is TreeListItem t))
                    {
                        continue;
                    }
                    t.SetSelected(true);
                    SelectedItems.InternalAdd(t);
                }
                Invalidate();
                RaiseItemSelectionChangedEvent();
            }
            //选中且不是多选时，只触发单击事件
            else if (item.Selected && SelectedItems.Count == 1)
            {
                RaiseItemClickEvent(item);
                return;
            }
            //选中当前行
            else
            {
                SelectItem(item, true);
                HideEditor();
                _lastRowIndex = item.Index;
            }

            RaiseItemClickEvent(item);
        }

        /// <summary>
        /// 调整数据行的座标。当选中的行显示在顶处或底处，但是只显示一部份时，调整座标以显示整行的内容。
        /// </summary>
        /// <param name="info"></param>
        private void AdjustItemPosistion(TreeListHitTestInfo info)
        {
            var y = info.Bounds.Y - _bound.ItemBound.Y;
            if (y < 0 || (y = info.Bounds.Bottom - _bound.ItemBound.Bottom) > 0)
            {
                var barValue = _vbar.Value + y;
                if (y > _vbar.Maximum)
                {
                    barValue = _vbar.Maximum;
                }
                else if (y < _vbar.Minimum)
                {
                    barValue = _vbar.Minimum;
                }

                _vbar.Value = barValue;
                var r = info.Bounds;
                r.Offset(0, -y);
                info.Bounds = r;

                if (info.Owner != null)
                {
                    r = info.Owner.Bounds;
                    r.Offset(0, -y);
                    info.Owner.Bounds = r;
                }
            }
        }

        /// <summary>
        /// 处理单元格单击。
        /// </summary>
        /// <param name="info"></param>
        private void ProcessCellClick(TreeListHitTestInfo info)
        {
            AdjustItemPosistion(info);

            var cell = (TreeListCell)info.Element;
            RaiseCellClickEvent(cell);

            switch (cell.BoxType)
            {
                case BoxType.CheckBox:
                    if (!RaiseBeforeCellCheckChangeEvent(cell))
                    {
                        cell.Checked = !cell.Checked;
                        RaiseAfterCellCheckChangeEvent(cell);
                    }
                    break;
                case BoxType.RadioButton:
                    var items = cell.Item.Parent == null ? cell.Item.TreeList.Items : cell.Item.Parent.Items;
                    foreach (var item in items)
                    {
                        if (item == cell.Item)
                        {
                            continue;
                        }

                        var c1 = item.Cells[cell.Column];
                        if (!RaiseBeforeCellCheckChangeEvent(c1))
                        {
                            c1.Checked = false;
                            RaiseAfterCellCheckChangeEvent(c1);
                        }
                    }

                    if (!RaiseBeforeCellCheckChangeEvent(cell))
                    {
                        cell.Checked = true;
                        RaiseAfterCellCheckChangeEvent(cell);
                    }
                    break;
            }

            cell.Item.InvalidateItem();

            if (cell.Item.Enabled && cell.Item.Selected && !RaiseBeforeCellEditingEvent(cell))
            {
                var rect = GetCellTextRectangle(cell, info.Bounds);
                _editor.BeginEdit(cell, rect);
            }

            if (info.Owner != null)
            {
                ProcessItemClick(info.Owner);
            }
        }

        /// <summary>
        /// 处理 +/- 符号单击。
        /// </summary>
        /// <param name="info"></param>
        private void ProcessPlusMinusClick(TreeListHitTestInfo info)
        {
            var item = (TreeListItem)((VirtualTreeListItem)info.Element).Item;
            var expended = item.Expended;

            var cancel = expended ? RaiseBeforeItemCollapseEvent(item) : RaiseBeforeItemExpandEvent(item);
            if (cancel)
            {
                return;
            }

            HideEditor();

            item.Expended = !item.Expended;
        }

        private void ProcessItemCheckedChange(TreeListHitTestInfo info)
        {
            var vitem = (VirtualTreeListItem)info.Element;
            var item = (TreeListItem)vitem.Item;

            if (RaiseBeforeItemCheckChangeEvent(item))
            {
                return;
            }

            item.Checked = !item.Checked;
            if (item.Mixed)
            {
                item.Mixed = false;
            }

            RaiseAfterItemCheckChangeEvent(item);

            InvalidateItem(vitem);
        }

        internal void ProcessItemExpand(TreeListItem item)
        {
            if (item.Expended)
            {
                RaiseAfterItemExpandEvent(item);

                if (_sortedColumn != null)
                {
                    item.Items.Sort(_sortVersion, _sortedColumn, _sortedOrder);
                }
            }
            else
            {
                RaiseAfterItemCollapseEvent(item);
            }

            UpdateItems();
        }

        internal void ProcessItemCheck(TreeListItem item)
        {
            RaiseAfterItemCheckChangeEvent(item);

            InvalidateItem(item);
        }

        /// <summary>
        /// 处理鼠标双击时 <see cref="TreeListHitTestInfo"/> 的处理。
        /// </summary>
        /// <param name="info"></param>
        private void ProcessHitTestInfoDbClick(TreeListHitTestInfo info)
        {
            HideEditor();
            switch (info.HitTestType)
            {
                case TreeListHitTestType.Cell:
                    if (info.Owner != null)
                    {
                        ProcessHitTestInfoDbClick(info.Owner);
                    }

                    break;
                case TreeListHitTestType.Item:
                    if (info.Element != null)
                    {
                        var item = (TreeListItem)((VirtualTreeListItem)info.Element).Item;
                        RaiseItemDoubleClickEvent(item);
                    }

                    break;
            }
        }
    }
}
