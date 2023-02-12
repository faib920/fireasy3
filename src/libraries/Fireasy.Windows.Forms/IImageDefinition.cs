// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using System.Drawing;

namespace Fireasy.Windows.Forms
{
    /// <summary>
    /// 提供图像支持的定义接口。
    /// </summary>
    public interface IImageDefinition
    {
        /// <summary>
        /// 获取一个 <see cref="ImageList"/> 实例。
        /// </summary>
        ImageList ImageList { get; }

        /// <summary>
        /// 获取图像 key。
        /// </summary>
        string ImageKey { get; set; }

        /// <summary>
        /// 获取图像索引。
        /// </summary>
        int ImageIndex { get; set; }

        /// <summary>
        /// 获取图像。
        /// </summary>
        Image Image { get; set; }
    }
}
