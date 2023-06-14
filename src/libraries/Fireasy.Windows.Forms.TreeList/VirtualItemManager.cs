// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Linq;

namespace Fireasy.Windows.Forms
{
    internal class VirtualItemManager
    {
        private readonly VirtualTreeListItemCollection _virtualList = new VirtualTreeListItemCollection();
        private readonly TreeList _treeList;
        private int _rowNumberIndex;
        private int _y;

        public VirtualItemManager(TreeList treeList)
        {
            _treeList = treeList;
        }

        public VirtualTreeListItemCollection Items
        {
            get
            {
                return _virtualList;
            }
        }

        internal int GetClientHeight()
        {
            return _virtualList.Count == 0 ? 0 : _virtualList.Last().Bounds.Bottom;
        }

        internal void Recalc()
        {
            _y = 0;
            _rowNumberIndex = 0;
            _virtualList.Clear();
            if (_treeList.Groups.Count == 0)
            {
                GenerateVirtualListItems(_treeList.Items);
            }
            else
            {
                foreach (var g in _treeList.Groups)
                {
                    var vitem = new VirtualTreeListItem(g, _virtualList.Count);
                    vitem.Bounds = new Rectangle(0, _y, 0, _treeList.GroupHeight);
                    _virtualList.Add(vitem);

                    _y += _treeList.GroupHeight;

                    if (g.Expended)
                    {
                        GenerateVirtualListItems(g.Items);
                    }
                }
            }
        }

        private void GenerateVirtualListItems(TreeListItemCollection items)
        {
            for (var i = 0; i < items.Count; i++)
            {
                var vitem = new VirtualTreeListItem(items[i], _virtualList.Count);
                vitem.Bounds = new Rectangle(0, _y, 0, _treeList.ItemHeight);
                items[i].DataIndex = (++_rowNumberIndex);
                _virtualList.Add(vitem);

                _y += _treeList.ItemHeight;

                if (_treeList.ShowGridLines)
                {
                    _y += 1;
                }

                if (items[i].Selected)
                {
                    _treeList.SelectedItems.Add(items[i]);
                }

                if (items[i].Expended)
                {
                    GenerateVirtualListItems(items[i].Items);
                }
            }
        }

    }
}
