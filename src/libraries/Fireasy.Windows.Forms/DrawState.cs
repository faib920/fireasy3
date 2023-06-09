﻿// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Fireasy.Windows.Forms
{
    /// <summary>
    /// 绘制时的状态。
    /// </summary>
    public enum DrawState
    {
        /// <summary>
        /// 正常。
        /// </summary>
        Normal,
        /// <summary>
        /// 项被选中。
        /// </summary>
        Selected,
        /// <summary>
        /// 项被按下。
        /// </summary>
        Pressed,
        /// <summary>
        /// 鼠标移上项。
        /// </summary>
        Hot,
        /// <summary>
        /// 鼠标拖拽项。
        /// </summary>
        Drag,
        /// <summary>
        /// 鼠标拖拽到顶部。
        /// </summary>
        DragTop,
        /// <summary>
        /// 鼠标拖拽到底部。
        /// </summary>
        DragBottom
    }
}
