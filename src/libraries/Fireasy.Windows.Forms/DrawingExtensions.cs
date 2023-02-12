// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Fireasy.Common;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Fireasy.Windows.Forms
{
    /// <summary>
    /// 绘制扩展方法。
    /// </summary>
    public static class DrawingExtensions
    {
        /// <summary>
        /// 使用新的 <see cref="Region"/> 限定绘画区域，结束后恢复原来的 <see cref="Region"/>。
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="region">新的限定绘画的区域。</param>
        /// <param name="action">要使用新 <see cref="Region"/> 限定绘制的方法。</param>
        /// <param name="set"></param>
        public static void KeepClip(this Graphics graphics, Region region, Action action, bool set = true)
        {
            Guard.ArgumentNull(graphics, nameof(graphics));
            Guard.ArgumentNull(region, nameof(region));
            Guard.ArgumentNull(action, nameof(action));

            Region? saved = null;
            if (set)
            {
                saved = graphics.Clip == null ? null : graphics.Clip.Clone();
                graphics.SetClip(region, CombineMode.Intersect);
            }

            action();

            if (saved != null)
            {
                graphics.SetClip(saved, CombineMode.Replace);
            }
        }

        /// <summary>
        /// 使用新的 <see cref="Rectangle"/> 限定绘画区域，结束后恢复原来的 <see cref="Rectangle"/>。
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="rect">新的限定绘画的矩形区域。</param>
        /// <param name="action">要使用新 <see cref="Region"/> 限定绘制的方法。</param>
        /// <param name="set"></param>
        public static void KeepClip(this Graphics graphics, Rectangle rect, Action action, bool set = true)
        {
            graphics.KeepClip(new Region(rect), action, set);
        }

        /// <summary>
        /// 将给定的矩形向右扩展一定的宽度，右边坐标保持不变。
        /// </summary>
        /// <param name="rect">给定的矩形。</param>
        /// <param name="width">往右扩展的宽度。</param>
        /// <returns></returns>
        public static Rectangle ReduceRight(this Rectangle rect, int width)
        {
            rect.X += width;
            rect.Width -= width;
            return rect;
        }

        /// <summary>
        /// 将给定的矩形向左扩展一定的宽度，右边坐标保持不变。
        /// </summary>
        /// <param name="rect">给定的矩形。</param>
        /// <param name="width">往左扩展的宽度。</param>
        /// <returns></returns>
        public static Rectangle ReduceLeft(this Rectangle rect, int width)
        {
            rect.X -= width;
            rect.Width += width;
            return rect;
        }

        /// <summary>
        /// 在给定的矩形中划出指定大小的矩形。
        /// </summary>
        /// <param name="rect">给定的矩形。</param>
        /// <param name="width">矩形的宽度。</param>
        /// <param name="height">矩形的调试。</param>
        /// <returns></returns>
        public static Rectangle Middle(this Rectangle rect, int width, int height)
        {
            return new Rectangle(rect.X + (rect.Width - width) / 2, rect.Y + (rect.Height - height) / 2, width, height);
        }

        /// <summary>
        /// 从定义接口中获取图像。
        /// </summary>
        /// <param name="definition"></param>
        /// <returns></returns>
        public static Image? GetImage(this IImageDefinition definition)
        {
            if (definition.Image != null)
            {
                return definition.Image;
            }

            if (definition.ImageList == null)
            {
                return null;
            }

            if (!string.IsNullOrEmpty(definition.ImageKey) &&
                definition.ImageList.Images.ContainsKey(definition.ImageKey))
            {
                return definition.ImageList.Images[definition.ImageKey];
            }

            if (definition.ImageIndex >= 0 && definition.ImageIndex <= definition.ImageList.Images.Count - 1)
            {
                return definition.ImageList.Images[definition.ImageIndex];
            }

            return null;
        }
    }
}
