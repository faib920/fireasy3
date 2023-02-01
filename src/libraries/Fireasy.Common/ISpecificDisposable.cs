// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Fireasy.Common
{
    /// <summary>
    /// 对 <see cref="IDisposable"/> 的特殊化处理。
    /// </summary>
    internal interface ISpecificDisposable
    {
        void Dispose(bool disposing);
    }

    /// <summary>
    /// 对 <see cref="IAsyncDisposable"/> 的特殊化处理。
    /// </summary>
    internal interface ISpecificAsyncDisposable
    {
        ValueTask DisposeAsync(bool disposing);
    }
}
