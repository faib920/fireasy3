// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Fireasy.Common.Extensions
{
    /// <summary>
    /// 序列拆分方式。
    /// </summary>
    public enum SequenceSplitMode
    {
        /// <summary>
        /// 最后一个序列为剩余的数量。
        /// </summary>
        Normal,
        /// <summary>
        /// 最后两个序列的数量均衡。
        /// </summary>
        Equationally
    }
}
