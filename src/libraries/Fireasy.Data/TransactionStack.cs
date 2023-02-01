// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Fireasy.Data
{
    /// <summary>
    /// 一个堆栈，用于事务控制。
    /// </summary>
    internal sealed class TransactionStack
    {
        private int _pos;

        /// <summary>
        /// 入栈。
        /// </summary>
        internal void Push()
        {
            _pos++;
        }

        /// <summary>
        /// 出栈，并判断是否已至栈底。
        /// </summary>
        /// <returns></returns>
        internal bool Pop()
        {
            _pos--;
            return _pos == 0;
        }
    }
}
