using Fireasy.Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Fireasy.Tests.Base
{
    public abstract class ServiceProviderBaseTests
    {
        public IServiceProvider ServiceProvider { get; }

        public ServiceProviderBaseTests()
        {
            var services = new ServiceCollection();

            services.AddLogging(s => s.AddConsole());

            var builder = services.AddFireasy();
            RegisterServices(services);
            ConfigureBuilder(builder);

            ServiceProvider = services.BuildServiceProvider();
        }

        protected virtual void RegisterServices(IServiceCollection services)
        {
        }

        protected virtual void ConfigureBuilder(SetupBuilder builder)
        {
        }
    }
}
