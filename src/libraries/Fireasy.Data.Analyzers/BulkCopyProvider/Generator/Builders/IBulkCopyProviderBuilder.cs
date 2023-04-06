// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Microsoft.CodeAnalysis.Text;

namespace Fireasy.Data.Analyzers.BulkCopyProvider.Generator.Builders
{
    /// <summary>
    /// BulkCopyProvider 类构造器。
    /// </summary>
    public interface IBulkCopyProviderBuilder
    {
        /// <summary>
        /// 获取构建的 BulkCopyProvider 类名。 
        /// </summary>
        string BulkCopyProviderTypeName { get; }

        /// <summary>
        /// 构建源代码。
        /// </summary>
        /// <returns></returns>
        SourceText BuildSource();
    }
}
