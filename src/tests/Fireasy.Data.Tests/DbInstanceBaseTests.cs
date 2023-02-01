using Fireasy.Common.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Fireasy.Data.Tests
{
    public abstract class DbInstanceBaseTests : ConfigurationBaseTests
    {
        /// <summary>
        /// 连接字符串
        /// </summary>
        protected virtual string ConnectionString => string.Empty;

        /// <summary>
        /// 实例名称
        /// </summary>
        protected virtual string InstanceName => string.Empty;
    }
}
