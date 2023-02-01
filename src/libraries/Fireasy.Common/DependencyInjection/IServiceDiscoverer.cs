// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.Reflection;

namespace Fireasy.Common.DependencyInjection
{
    /// <summary>
    /// 提供用于服务发现的接口。
    /// </summary>
    public interface IServiceDiscoverer
    {
        /// <summary>
        /// 获取被发现的 <see cref="Assembly"/> 集合。
        /// </summary>
        ReadOnlyCollection<Assembly> Assemblies { get; }

        /// <summary>
        /// 获取注册的 <see cref="ServiceDescriptor"/> 集合。
        /// </summary>
        ReadOnlyCollection<ServiceDescriptor> Descriptors { get; }

    }
}
