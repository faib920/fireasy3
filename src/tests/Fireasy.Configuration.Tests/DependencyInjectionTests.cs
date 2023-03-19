using Fireasy.Tests.Base;
using Microsoft.Extensions.Options;

namespace Fireasy.Configuration.Tests
{
    [TestClass]
    public class DependencyInjectionTests : ConfigurationBaseTests
    {
        protected override void RegisterServices(IServiceCollection services)
        {
            base.RegisterServices(services);

            services.Configure<Options>(Configuration.GetSection("fireasy:dddd"));
        }

        [TestMethod]
        public void TestGetConfigurationUnity()
        {
            var unity = ServiceProvider.GetRequiredService<ConfigurationUnity>();
            var options = ServiceProvider.GetRequiredService<IOptions<Options>>();

            Assert.IsNotNull(unity);
        }
    }

    public class Options
    {
        public List<OptionSetting> Settings { get; set; }
    }

    public class OptionSetting
    {
        public Type Type { get; set; }
    }

    public class Test1
    {

    }
}