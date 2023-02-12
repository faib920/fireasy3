// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Fireasy.Windows.Forms
{
    /// <summary>
    /// 拖拽的位置。
    /// </summary>
    public enum DragPosition
    {
        /// <summary>
        /// 拖拽到中间位置，作为目标的孩子。
        /// </summary>
        Children,
        /// <summary>
        /// 拖拽到顶部位置，移动到目标的前面。
        /// </summary>
        After,
        /// <summary>
        /// 拖拽到底部位置，移动到目标的后面。
        /// </summary>
        Before
    }
}
