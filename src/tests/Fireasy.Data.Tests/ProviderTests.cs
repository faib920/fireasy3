using Fireasy.Data.Syntax;
using Fireasy.Data.Syntax.SqlServer;
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
            //sqlserver使用2012以下版本的语法
            builder.ConfigureData(s => s.AddProvider<TestProvider>("MySql").AddProivderService<SqlServerProvider, SqlServerSyntaxLessThan2012>());
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

            //通过配置 fireasy:dataProviders 自动注册了提供者
            Assert.IsTrue(names.Contains("myprovider"));

            //由于引用了 Fireasy.Data.OleDb，自动注册了 OleDbProvider
            Assert.IsTrue(names.Contains("OleDb"));
        }

        /// <summary>
        /// 测试获取所有定义的提供者名称
        /// </summary>
        [TestMethod]
        public void TestGetSupportedProviders()
        {
            var manager = ServiceProvider.GetRequiredService<IProviderManager>();

            var descriptors = manager.GetSupportedProviders();

            foreach (var item in descriptors)
            {
                Console.WriteLine($"{item.Alais} {item.Description}");
            }

            //通过配置 fireasy:dataProviders 自动注册了提供者
            Assert.IsTrue(descriptors.Any(s => s.Alais == "myprovider"));

            //由于引用了 Fireasy.Data.OleDb，自动注册了 OleDbProvider
            Assert.IsTrue(descriptors.Any(s => s.Alais == "OleDb"));
        }

        /// <summary>
        /// 测试获取指定名称的提供者
        /// </summary>
        [TestMethod]
        public void TestGetDefinedProvider()
        {
            var manager = ServiceProvider.GetRequiredService<IProviderManager>();

            var provider = manager.GetDefinedProvider("myprovider");

            Assert.IsInstanceOfType(provider, typeof(TestProvider));
        }

        /// <summary>
        /// 测试更换了 DbProviderFactory
        /// </summary>
        [TestMethod]
        public async Task TestChangeProviderFactory()
        {
            var factory = ServiceProvider.GetRequiredService<IDatabaseFactory>();

            await using var database = factory.CreateDatabase("sqlserver");
            Assert.IsNotNull(database);

            //被更换了DbProviderFactory
            Assert.AreEqual(System.Data.SqlClient.SqlClientFactory.Instance, database.Provider.DbProviderFactory);
        }

        /// <summary>
        /// 测试更换了提供者
        /// </summary>
        [TestMethod]
        public async Task TestChangeProvider()
        {
            var factory = ServiceProvider.GetRequiredService<IDatabaseFactory>();

            await using var database = factory.CreateDatabase("mysql");
            Assert.IsNotNull(database);

            //被更换了provider
            Assert.IsInstanceOfType(database.Provider, typeof(TestProvider));
        }

        /// <summary>
        /// 测试自定义提供者
        /// </summary>
        [TestMethod]
        public async Task TestCustomizedProvider()
        {
            var factory = ServiceProvider.GetRequiredService<IDatabaseFactory>();

            await using var database = factory.CreateDatabase("testdb");
            Assert.IsNotNull(database);

            //自定义provider
            Assert.IsInstanceOfType(database.Provider, typeof(TestProvider));
        }

        /// <summary>
        /// 测试自定义提供者插件服务
        /// </summary>
        [TestMethod]
        public async Task TestCustomizedProviderService()
        {
            var factory = ServiceProvider.GetRequiredService<IDatabaseFactory>();

            await using var database = factory.CreateDatabase<SqlServerProvider>(Constants.SqlServer_ConnectionString);
            Assert.IsNotNull(database);

            var syntax = database.Provider.GetService<ISyntaxProvider>();

            //切换使用2012以下版本的语法
            Assert.IsInstanceOfType(syntax, typeof(SqlServerSyntaxLessThan2012));

            //分页语法变为 ROW_NUMBER() OVER
            var pager = new DataPager(10, 0);
            var list = await database.ExecuteEnumerableAsync("select * from customers", pager);

            Assert.AreEqual(10, list.Count());
        }
    }
}
