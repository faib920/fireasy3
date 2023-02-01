// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Fireasy.Common.Emit;
using Fireasy.Common.Threading;

namespace Fireasy.Common.DynamicProxy
{
    /// <summary>
    /// 用于在当前线程内共享一个 <see cref="DynamicAssemblyBuilder"/> 实例。无法继承此类。
    /// </summary>
    public sealed class AssemblyScope : Scope<AssemblyScope>
    {
        /// <summary>
        /// 初始化 <see cref="AssemblyScope"/> 类的新实例。
        /// </summary>
        /// <param name="assemblyName">程序集名称。</param>
        public AssemblyScope(string assemblyName)
        {
            AssemblyBuilder = new DynamicAssemblyBuilder(assemblyName);
        }

        internal DynamicAssemblyBuilder AssemblyBuilder { get; }
    }
}
