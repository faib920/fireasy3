﻿// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Fireasy.Common.DependencyInjection
{
    /// <summary>
    /// 用于标识类型不被 <see cref="IServiceDiscoverer"/> 发现。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class DisableServerDiscoverAttribute : Attribute
    {
    }
}
