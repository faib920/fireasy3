// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Fireasy.Common.Compiler
{
    /// <summary>
    /// 配置参数。
    /// </summary>
    public class ConfigureOptions
    {
        /// <summary>
        /// 获取或设置程序集名称。
        /// </summary>
        public string? AssemblyName { get; set; }

        /// <summary>
        /// 获取或设置输出的程序集。
        /// </summary>
        public string? OutputAssembly { get; set; }

        /// <summary>
        /// 获取或设置编译选项。
        /// </summary>
        public string? CompilerOptions { get; set; }

        /// <summary>
        /// 获取附加的程序集。
        /// </summary>
        public List<string> Assemblies { get; private set; } = new List<string>();
    }
}
