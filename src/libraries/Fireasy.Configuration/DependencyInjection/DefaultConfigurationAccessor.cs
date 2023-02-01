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
