// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using System;
using System.Data.Common;
using System.IO;
using System.Linq;

namespace Fireasy.Data.Provider
{
    /// <summary>
    /// 从本地一组程序集中动态加载解析。
    /// </summary>
    public class AssemblyProviderFactoryResolver : IProviderFactoryResolver
    {
        private readonly string[] _typeNames;

        /// <summary>
        /// 初始化类 <see cref="AssemblyProviderFactoryResolver"/> 类的新实例。
        /// </summary>
        /// <param name="typeNames">一组程序集标识。</param>
        public AssemblyProviderFactoryResolver(params string[] typeNames)
        {
            _typeNames = typeNames;
        }

        bool IProviderFactoryResolver.TryResolve(out DbProviderFactory? factory, out Exception? exception)
        {
            foreach (var typeName in _typeNames)
            {
                if (AssemblyLoader.TryLoad(typeName, out factory))
                {
                    exception = null;
                    return true;
                }
            }

            var typeNames = string.Join("、", _typeNames.Select(s => s.Substring(s.LastIndexOf(",") + 1).Trim()).ToArray());
            var message = _typeNames.Count() == 1 ? $"必须从 Nuget 里安装 {typeNames} 组件。" : $"必须从 Nuget 里安装 {typeNames} 中的任意一个组件。";
            exception = new FileNotFoundException(message);
            factory = null;

            return false;
        }
    }
}
