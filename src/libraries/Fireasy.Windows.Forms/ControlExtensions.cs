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
    /// 控件扩展方法。
    /// </summary>
    public static class ControlExtensions
    {
        /// <summary>
        /// 多线程内对窗体的调用操作。
        /// </summary>
        /// <param name="control"></param>
        /// <param name="method"></param>
        public static void Invoke(this Control control, Action method)
        {
            if (control.InvokeRequired)
            {
                control.Invoke((Delegate)method);
            }
            else
            {
                method();
            }
        }
    }
}
