﻿// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Fireasy.Common.DynamicProxy
{
    /// <summary>
    /// 表示 Aspect 类型的异常。
    /// </summary>
    [Serializable]
    public sealed class DynamicProxyException : Exception
    {
        /// <summary>
        /// 初始化 <see cref="DynamicProxyException"/> 类的新实例。
        /// </summary>
        /// <param name="message">异常信息。</param>
        public DynamicProxyException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// 初始化 <see cref="DynamicProxyException"/> 类的新实例。
        /// </summary>
        /// <param name="message">异常信息。</param>
        /// <param name="exception">内部异常对象。</param>
        public DynamicProxyException(string message, Exception exception)
            : base(message, exception)
        {
        }
    }
}
