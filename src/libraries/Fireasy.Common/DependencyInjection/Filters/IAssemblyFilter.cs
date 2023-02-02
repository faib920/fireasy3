// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using System.Reflection;

namespace Fireasy.Common.DependencyInjection.Filters
{
    /// <summary>
    /// 提供程序集过滤器。
    /// </summary>
    public interface IAssemblyFilter
    {
        /// <summary>
        /// 判断 <see cref="Assembly"/> 是否被过滤。
        /// </summary>
        /// <param name="assembly">指定的程序集。</param>
        /// <returns>为 true 表示被过滤。</returns>
        bool IsFilter(Assembly assembly);
    }
}
