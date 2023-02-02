// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Fireasy.Data.Syntax
{
    /// <summary>
    /// 无法提供语法解析时抛出的异常。
    /// </summary>
    [Serializable]
    public sealed class SyntaxNotSupportedException : Exception
    {
        /// <summary>
        /// 使用语句法初始化 <see cref="SyntaxNotSupportedException"/> 类的新实例。
        /// </summary>
        /// <param name="syntaxName">语法名称。</param>
        public SyntaxNotSupportedException(string syntaxName)
            : base($"没有提供 \"{syntaxName}\" 的语法解析。")
        {
        }
    }
}
