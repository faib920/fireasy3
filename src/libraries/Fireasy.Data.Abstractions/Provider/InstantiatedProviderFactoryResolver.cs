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
    /// 指定 <see cref="DbProviderFactory"/> 实例。
    /// </summary>
    public class InstantiatedProviderFactoryResolver : IProviderFactoryResolver
    {
        private readonly DbProviderFactory _instance;

        /// <summary>
        /// 初始化类 <see cref="InstalledProviderFactoryResolver"/> 类的新实例。
        /// </summary>
        /// <param name="instance"><see cref="DbProviderFactory"/> 实例。</param>
        public InstantiatedProviderFactoryResolver(DbProviderFactory instance)
        {
            _instance = instance;
        }

        bool IProviderFactoryResolver.TryResolve(out DbProviderFactory? factory, out Exception? exception)
        {
            factory = _instance;
            exception = null;

            return true;
        }
    }
}
