// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Fireasy.Common.Emit
{
    /// <summary>
    /// 动态构建时发生的异常。
    /// </summary>
    public class DynamicBuildException : Exception
    {
        /// <summary>
        /// 初始化 <see cref="DynamicBuildException"/> 类的新实例。
        /// </summary>
        /// <param name="message"></param>
        public DynamicBuildException(string message) : base(message) { }

        /// <summary>
        /// 初始化 <see cref="DynamicBuildException"/> 类的新实例。
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public DynamicBuildException(string message, Exception innerException) : base(message, innerException) { }
    }
}
