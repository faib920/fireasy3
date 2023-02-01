﻿using Fireasy.Tests.Base;

namespace Fireasy.Configuration.Tests
{
    public class ConfigurationBaseTests : ServiceProviderBaseTests
    {
        protected IConfiguration Configuration { get; private set; }

        protected override void RegisterServices(IServiceCollection services)
        {
            Configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();

            services.AddSingleton(Configuration);
        }
    }
}
