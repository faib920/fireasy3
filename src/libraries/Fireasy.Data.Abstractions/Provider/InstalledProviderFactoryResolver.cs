// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using System.Data.Common;
using System.IO;

namespace Fireasy.Data.Provider
{
#if !NETSTANDARD2_0
    /// <summary>
    /// 通过 DbProviderFactories.GetFactory 从系统环境中安装的驱动中解析。
    /// </summary>
    public class InstalledProviderFactoryResolver : IProviderFactoryResolver
    {
        private readonly string _providerName;

        /// <summary>
        /// 初始化类 <see cref="InstalledProviderFactoryResolver"/> 类的新实例。
        /// </summary>
        /// <param name="providerName">固定的驱动名称。</param>
        public InstalledProviderFactoryResolver(string providerName)
        {
            _providerName = providerName;
        }

        bool IProviderFactoryResolver.TryResolve(out DbProviderFactory? factory, out Exception? exception)
        {
            factory = DbProviderFactories.GetFactory(_providerName);
            if (factory != null)
            {
                exception = null;
                return true;
            }

            exception = new FileNotFoundException($"{_providerName} 未找到，请下载相关的程序进行安装。");
            factory = null;

            return false;
        }
    }
#endif
}
