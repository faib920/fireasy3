// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Fireasy.Common.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Fireasy.Configuration.DependencyInjection
{
    public class DefaultConfigurationAccessor : IConfigurationAccessor
    {
        public IConfiguration? GetConfiguration(IServiceCollection services)
        {
            return services.GetSingletonInstance<IConfiguration>();
        }
    }
}
