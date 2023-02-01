using Microsoft.Extensions.DependencyInjection;

namespace Fireasy.Data.Tests
{
    /// <summary>
    /// 数据库提供者测试
    /// </summary>
    [TestClass]
    public class ProviderTests : DbInstanceBaseTests
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder"></param>
        protected override void ConfigureBuilder(SetupBuilder builder)
        {
            base.ConfigureBuilder(builder);

            //用于替换 SqlServerProvider 中的 DbProviderFactory
            builder.ConfigureData(s => s.AddProivderFactory<SqlServerProvider>(System.Data.SqlClient.SqlClientFactory.Instance));

            //用于替换数据库提供者
            builder.ConfigureData(s => s.AddProvider<TestProvider>("MySql"));
        }

        /// <summary>
        /// 测试获取所有定义的提供者名称
        /// </summary>
        [TestMethod]
        public void TestGetSupportedProviderNames()
        {
            var manager = ServiceProvider.GetRequiredService<IProviderManager>();

            var names = manager.GetSupportedProviderNames();

            foreach (var name in names)
            {
                Console.WriteLine(name);
            }

            Assert.IsTrue(names.Contains("myprovider"));
        }

        /// <summary>
        /// 测试获取指定名称的提供者
        /// </summary>
        [TestMethod]
        public void TestGetDefinedProviderInstance()
        {
            var manager = ServiceProvider.GetRequiredService<IProviderManager>();

            var provider = manager.GetDefinedProviderInstance("myprovider");

            Assert.IsInstanceOfType(provider, typeof(TestProvider));
        }

        /// <summary>
        /// 测试更换了 DbProviderFactory
        /// </summary>
        [TestMethod]
        public void TestChangeProviderFactory()
        {
            var factory = ServiceProvider.GetRequiredService<IDatabaseFactory>();

            using var database = factory.CreateDatabase("sqlserver");
            Assert.IsNotNull(database);

            //被更换了DbProviderFactory
            Assert.AreEqual(System.Data.SqlClient.SqlClientFactory.Instance, database.Provider.DbProviderFactory);
        }

        /// <summary>
        /// 测试更换了提供者
        /// </summary>
        [TestMethod]
        public void TestChangeProvider()
        {
            var factory = ServiceProvider.GetRequiredService<IDatabaseFactory>();

            using var database = factory.CreateDatabase("mysql");
            Assert.IsNotNull(database);

            //被更换了provider
            Assert.IsInstanceOfType(database.Provider, typeof(TestProvider));
        }

        /// <summary>
        /// 测试自定义提供者
        /// </summary>
        [TestMethod]
        public void TestCustomeProvider()
        {
            var factory = ServiceProvider.GetRequiredService<IDatabaseFactory>();

            using var database = factory.CreateDatabase("testdb");
            Assert.IsNotNull(database);

            //自定义provider
            Assert.IsInstanceOfType(database.Provider, typeof(TestProvider));
        }
    }
}
