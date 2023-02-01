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
    /// 提供类型过滤器
    /// </summary>
    public interface ITypeFilter
    {
        /// <summary>
        /// 判断 <see cref="Type"/> 是否被过滤。
        /// </summary>
        /// <param name="assembly">指定的程序集。</param>
        /// <param name="type">指定的类型。</param>
        /// <returns>为 true 表示被过滤。</returns>
        bool IsFilter(Assembly assembly, Type type);
    }
}
