﻿// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace Fireasy.Common.Modularity
{
    public interface IModuleStartup
    {
        void Initialize(IServiceProvider serviceProvider);
    }
}