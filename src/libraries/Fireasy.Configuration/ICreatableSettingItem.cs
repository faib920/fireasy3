// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace Fireasy.Configuration
{

    /// <summary>
    /// 指定了用于创建实例的类型的配置项。
    /// </summary>
    public interface ICreatableSettingItem
    {
        /// <summary>
        /// 获取用于创建实例的类型。
        /// </summary>
        Type CreationType { get; }
    }
}
