using Microsoft.Extensions.DependencyInjection;

namespace Fireasy.Data.Tests
{
    /// <summary>
    /// 测试 Provider
    /// </summary>
    public class TestProvider : MySqlProvider
    {
        public TestProvider(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
        }

        public override void Initialize(ProviderInitializeContext context)
        {
            base.Initialize(context);
        }
    }
}
