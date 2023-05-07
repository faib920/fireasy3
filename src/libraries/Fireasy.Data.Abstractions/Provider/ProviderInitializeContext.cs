// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Microsoft.Extensions.DependencyInjection;

namespace Fireasy.Data.Provider
{
    public class ProviderInitializeContext
    {
        public ProviderInitializeContext(IServiceCollection services, ConnectionString connectionString)
        {
            Services = services;
            ConnectionString = connectionString;
        }

        public IServiceCollection Services { get; }

        public ConnectionString ConnectionString { get; }
    }
}
