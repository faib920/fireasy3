// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using System.Reflection;

namespace Fireasy.Common.Compiler
{
    /// <summary>
    /// 代码编译器接口。
    /// </summary>
    public interface ICodeCompiler
    {
        /// <summary>
        /// 编译代码生成一个程序集。
        /// </summary>
        /// <param name="sources">程序源代码。</param>
        /// <param name="options">配置选项。</param>
        /// <returns>由代码编译成的程序集。</returns>
        Assembly? CompileAssembly(IEnumerable<string> sources, ConfigureOptions? options = null);
    }
}
