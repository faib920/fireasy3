// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using System.Data.Common;

namespace Fireasy.Data.Provider
{
    /// <summary>
    /// 定义 <see cref="DbProviderFactory"/> 的解析来源。
    /// </summary>
    public interface IProviderFactoryResolver
    {
        /// <summary>
        /// 解析返回 <see cref="DbProviderFactory"/> 对象。
        /// </summary>
        /// <param name="factory"><see cref="DbProviderFactory"/> 对象。</param>
        /// <param name="exception">异常信息。</param>
        /// <returns></returns>
        bool TryResolve(out DbProviderFactory? factory, out Exception? exception);
    }
}
