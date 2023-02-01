// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace Fireasy.Data.Syntax
{
    /// <summary>
    /// 语法解析错误时抛出的异常。
    /// </summary>
    [Serializable]
    public class SyntaxParseErrorException : Exception
    {
        /// <summary>
        /// 初始化 <see cref="SyntaxParseErrorException"/> 类的新实例。
        /// </summary>
        /// <param name="message">异常信息。</param>
        public SyntaxParseErrorException(string message)
            : base(message) 
        { 
        }
    }
}
