// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using System.Reflection;
using System.Text.RegularExpressions;

namespace Fireasy.Common.DependencyInjection.Filters
{
    /// <summary>
    /// 默认的程序集过滤器。
    /// </summary>
    internal class DefaultAssemblyFilter : IAssemblyFilter
    {
        private readonly Regex _matchRegex = new Regex("Microsoft.*|System.*");

        /// <summary>
        /// 判断 <see cref="Assembly"/> 是否被过滤。
        /// </summary>
        /// <param name="assembly">指定的程序集。</param>
        /// <returns>为 true 表示被过滤。</returns>
        bool IAssemblyFilter.IsFilter(Assembly assembly)
        {
            return _matchRegex.IsMatch(assembly.GetName().Name);
        }
    }
}
