using Fireasy.Common.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Mysqlx.Expr;

namespace Fireasy.Data.Tests.DatabaseTest
{
    /// <summary>
    /// <see cref="IDatabase"/> 测试基类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class DatabaseTests<T> : DbInstanceBaseTests where T : IProvider
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        protected override void RegisterServices(IServiceCollection services)
        {
            base.RegisterServices(services);

            services.AddTransient<TestDIDatabase>();
        }

        /// <summary>
        /// 指定 <see cref="IProvider"/> 和连接串创建实例
        /// </summary>
        [TestMethod]
        public async Task TestCreateDatabase()
        {
            var factory = ServiceProvider.GetRequiredService<IDatabaseFactory>();

            await using var database = factory.CreateDatabase<T>(ConnectionString);
            Assert.IsNotNull(database);

            var exp = await database.TryConnectAsync();

            Assert.IsNull(exp);
        }

        /// <summary>
        /// 测试注入 <see cref="IDatabase"/>
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestDependencyInjectionDatabase()
        {
            await using var database = ServiceProvider.GetService<IDatabase>();
            Assert.IsNotNull(database);

            //配置里默认使用sqlserver
            Assert.IsInstanceOfType(database.Provider, typeof(SqlServerProvider));
        }

        /// <summary>
        /// 测试注入 <see cref="IProvider"/>
        /// </summary>
        [TestMethod]
        public async Task TestDependencyInjectionProvider()
        {
            var factory = ServiceProvider.GetRequiredService<IDatabaseFactory>();

            //全局中没有 IProvider
            var provider = ServiceProvider.GetService<IProvider>();
            Assert.IsNull(provider);

            //IProvider 是注册在 InternalServiceProvider 里面的，只有在 IDatabase 实例中可以获取
            await using var database = factory.CreateDatabase<T>(ConnectionString);
            provider = database.GetService<IProvider>();

            Assert.IsNotNull(provider);
            Assert.AreEqual(provider, database.Provider);
            Assert.IsInstanceOfType(provider, typeof(T));
        }

        /// <summary>
        /// 测试从其他类里注入 <see cref="IDatabase"/>
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestDependencyInjectionDatabaseByAnotherObject()
        {
            await using var database = ServiceProvider.GetService<IDatabase>();
            Assert.IsNotNull(database);

            var obj1 = ServiceProvider.GetService<TestDIDatabase>();
            var obj2 = ServiceProvider.GetService<TestDIDatabase>();

            Assert.IsTrue(obj1!.AreEqual(database));
            Assert.IsTrue(obj2!.AreEqual(database));
        }

        /// <summary>
        /// 从配置实例进行创建
        /// </summary>
        [TestMethod]
        public async Task TestCreateDatabaseFromConfig()
        {
            var factory = ServiceProvider.GetRequiredService<IDatabaseFactory>();

            await using var database = factory.CreateDatabase(InstanceName);
            Assert.IsNotNull(database);

            var exp = await database.TryConnectAsync();

            Assert.IsNull(exp);
        }

        /// <summary>
        /// 从提供者创建
        /// </summary>
        [TestMethod]
        public async Task TestCreateDatabaseByProviderName()
        {
            var factory = ServiceProvider.GetRequiredService<IDatabaseFactory>();

            await using var database = factory.CreateDatabase(ProviderName, ConnectionString);
            Assert.IsNotNull(database);

            var exp = await database.TryConnectAsync();

            Assert.IsNull(exp);
        }

        /// <summary>
        /// 测试事务
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestTransactionAsync()
        {
            var factory = ServiceProvider.GetRequiredService<IDatabaseFactory>();

            await using var database = factory.CreateDatabase<T>(ConnectionString);
            await database.BeginTransactionAsync();

            try
            {
                var syntax = database.GetService<ISyntaxProvider>();

                await database.ExecuteNonQueryAsync($"delete from customers");
                //表不存在，将抛错
                await database.ExecuteNonQueryAsync($"delete from testtable");

                await database.CommitTransactionAsync();
            }
            catch (Exception)
            {
                await database.RollbackTransactionAsync();
            }
        }

        /// <summary>
        /// 使用 <see cref="DefaultRowMapper{T}"/>
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestExecuteEnumerableMapperAsync()
        {
            var factory = ServiceProvider.GetRequiredService<IDatabaseFactory>();

            await using var database = factory.CreateDatabase<T>(ConnectionString);
            var syntax = database.GetService<ISyntaxProvider>();

            var list = await database.ExecuteEnumerableAsync<Customers>($"select * from customers order by {syntax!.Delimit("CustomerID")}");

            Assert.AreEqual("ALFKI", list!.ElementAt(0).CustomerID);
        }

        /// <summary>
        /// 使用 <see cref="SingleValueRowMapper{T}"/>
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestExecuteEnumerableMapperSingleAsync()
        {
            var factory = ServiceProvider.GetRequiredService<IDatabaseFactory>();

            await using var database = factory.CreateDatabase<T>(ConnectionString);
            var syntax = database.GetService<ISyntaxProvider>();

            var list = await database.ExecuteEnumerableAsync<string>($"select {syntax!.Delimit("CustomerID")} from customers order by {syntax!.Delimit("CustomerID")}");

            Assert.AreEqual("ALFKI", list!.ElementAt(0));
        }

        /// <summary>
        /// 使用动态类型
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestExecuteEnumerableDynamicAsync()
        {
            var factory = ServiceProvider.GetRequiredService<IDatabaseFactory>();

            await using var database = factory.CreateDatabase<T>(ConnectionString);
            var syntax = database.GetService<ISyntaxProvider>();

            var list = await database.ExecuteEnumerableAsync($"select * from customers order by {syntax!.Delimit("CustomerID")}");

            Assert.AreEqual("ALFKI", list!.ElementAt(0).CustomerID);
        }

        /// <summary>
        /// 测试 <see cref="IAsyncEnumerable{T}"/>
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestExecuteAsyncEnumerable()
        {
            var factory = ServiceProvider.GetRequiredService<IDatabaseFactory>();

            await using var database = factory.CreateDatabase<T>(ConnectionString);
            var syntax = database.GetService<ISyntaxProvider>();

            await foreach (var item in database.ExecuteAsyncEnumerable($"select * from customers order by {syntax!.Delimit("CustomerID")}"))
            {
                Assert.AreEqual("ALFKI", item.CustomerID);
                break;
            }
        }

        /// <summary>
        /// 测试 <see cref="IAsyncEnumerable{T}"/> ToList
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestExecuteAsyncEnumerableToList()
        {
            var factory = ServiceProvider.GetRequiredService<IDatabaseFactory>();

            await using var database = factory.CreateDatabase<T>(ConnectionString);
            var syntax = database.GetService<ISyntaxProvider>();

            var list = await database.ExecuteAsyncEnumerable($"select * from customers order by {syntax!.Delimit("CustomerID")}").ToListAsync();
            Assert.AreEqual("ALFKI", list[0].CustomerID);
        }

        /// <summary>
        /// 测试 <see cref="IAsyncEnumerable{T}"/> FirstOrDefault
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestExecuteAsyncEnumerableFirstOrDefault()
        {
            var factory = ServiceProvider.GetRequiredService<IDatabaseFactory>();

            await using var database = factory.CreateDatabase<T>(ConnectionString);
            var syntax = database.GetService<ISyntaxProvider>();

            var item = await database.ExecuteAsyncEnumerable($"select * from customers order by {syntax!.Delimit("CustomerID")}").FirstOrDefaultAsync();
            Assert.AreEqual("ALFKI", item?.CustomerID);
        }

        /// <summary>
        /// 使用参数进行查询
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestExecuteEnumerableUseParameters()
        {
            var factory = ServiceProvider.GetRequiredService<IDatabaseFactory>();

            await using var database = factory.CreateDatabase<T>(ConnectionString);
            var syntax = database.GetService<ISyntaxProvider>();

            var parameters = new ParameterCollection();
            parameters.Add("customerId", "ALFKI");
            var list = await database.ExecuteEnumerableAsync($"select * from customers where {syntax!.Delimit("CustomerID")}=@customerId", parameters: parameters);

            Assert.AreEqual("ALFKI", list!.ElementAt(0).CustomerID);
        }

        /// <summary>
        /// 使用参数进行查询
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestExecuteEnumerableUseObjectParameters()
        {
            var factory = ServiceProvider.GetRequiredService<IDatabaseFactory>();

            await using var database = factory.CreateDatabase<T>(ConnectionString);
            var syntax = database.GetService<ISyntaxProvider>();

            var parameters = ParameterCollection.With(new { customerId = "ALFKI" });
            var list = await database.ExecuteEnumerableAsync($"select * from customers where {syntax!.Delimit("CustomerID")}=@customerId", parameters: parameters);

            Assert.AreEqual("ALFKI", list!.ElementAt(0).CustomerID);
        }

        /// <summary>
        /// 使用分页进行查询
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestExecuteEnumerableUseDataPager()
        {
            var factory = ServiceProvider.GetRequiredService<IDatabaseFactory>();

            await using var database = factory.CreateDatabase<T>(ConnectionString);

            var pager = new DataPager(10, 0);
            var list = await database.ExecuteEnumerableAsync("select * from customers", pager);

            Assert.AreEqual(10, list.Count());
        }

        /// <summary>
        /// 使用分页排序进行查询
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestExecuteEnumerableUseDataPagerAndOrderby()
        {
            var factory = ServiceProvider.GetRequiredService<IDatabaseFactory>();

            await using var database = factory.CreateDatabase<T>(ConnectionString);
            var syntax = database.GetService<ISyntaxProvider>();

            var pager = new DataPager(10, 1);
            var list = await database.ExecuteEnumerableAsync($"select * from customers order by {syntax!.Delimit("CustomerID")}", pager);

            Assert.AreEqual(10, list.Count());
        }

        /// <summary>
        /// 执行删除
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestExecuteNonQueryForDeleteAsync()
        {
            var factory = ServiceProvider.GetRequiredService<IDatabaseFactory>();

            await using var database = factory.CreateDatabase<T>(ConnectionString);
            var syntax = database.GetService<ISyntaxProvider>();

            var parameters = new ParameterCollection();
            parameters.Add("customerId", "NNNN");
            var ret = await database.ExecuteNonQueryAsync($"delete from customers where {syntax!.Delimit("CustomerID")}=@customerId", parameters: parameters);

            Assert.AreEqual(0, ret);
        }

        /// <summary>
        /// 执行更新
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestExecuteNonQueryForUpdateAsync()
        {
            var factory = ServiceProvider.GetRequiredService<IDatabaseFactory>();

            await using var database = factory.CreateDatabase<T>(ConnectionString);
            var syntax = database.GetService<ISyntaxProvider>();

            var parameters = new ParameterCollection();
            parameters.Add("customerId", "ALFKI");
            parameters.Add("country", "Germany");
            var ret = await database.ExecuteNonQueryAsync($"update customers set {syntax!.Delimit("Country")}=@country where {syntax!.Delimit("CustomerID")}=@customerId", parameters: parameters);

            Assert.AreEqual(1, ret);
        }

        /// <summary>
        /// 执行 ExecuteScalarAsync
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestExecuteScalarAsync()
        {
            var factory = ServiceProvider.GetRequiredService<IDatabaseFactory>();

            await using var database = factory.CreateDatabase<T>(ConnectionString);
            var syntax = database.GetService<ISyntaxProvider>();

            var value = await database.ExecuteScalarAsync<string>($"select {syntax!.Delimit("CustomerID")} from customers order by {syntax!.Delimit("CustomerID")}");

            Assert.AreEqual("ALFKI", value);
        }

        /// <summary>
        /// 执行 ExecuteScalarAsync，返回字节数组
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestExecuteScalarForBytesAsync()
        {
            var factory = ServiceProvider.GetRequiredService<IDatabaseFactory>();

            await using var database = factory.CreateDatabase<T>(ConnectionString);
            var syntax = database.GetService<ISyntaxProvider>();

            var value = await database.ExecuteScalarAsync<byte[]>($"select {syntax!.Delimit("Picture")} from categories where {syntax!.Delimit("CategoryID")}=1");

            Assert.IsTrue(value?.Length > 1000);
        }

        /// <summary>
        /// 执行 ExecuteScalarAsync，字节数组转为字符串
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestExecuteScalarForBytesToStringAsync()
        {
            var factory = ServiceProvider.GetRequiredService<IDatabaseFactory>();

            await using var database = factory.CreateDatabase<T>(ConnectionString);
            var syntax = database.GetService<ISyntaxProvider>();

            await Assert.ThrowsExceptionAsync<InvalidCastException>(async () =>
            {
                await database.ExecuteScalarAsync<string>($"select {syntax!.Delimit("Picture")} from categories where {syntax!.Delimit("CategoryID")}=1");
            });
        }

        /// <summary>
        /// 执行 ExecuteScalarAsync，整型转为长整型
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestExecuteScalarForIntegerToLongAsync()
        {
            var factory = ServiceProvider.GetRequiredService<IDatabaseFactory>();

            await using var database = factory.CreateDatabase<T>(ConnectionString);
            var syntax = database.GetService<ISyntaxProvider>();

            var value = await database.ExecuteScalarAsync<long>($"select {syntax!.Delimit("CategoryID")} from categories where {syntax!.Delimit("CategoryID")}=1");

            Assert.AreEqual(1L, value);
        }

        /// <summary>
        /// 执行 ExecuteScalarAsync，返回可空整型
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestExecuteScalarForNullableAsync()
        {
            var factory = ServiceProvider.GetRequiredService<IDatabaseFactory>();

            await using var database = factory.CreateDatabase<T>(ConnectionString);
            var syntax = database.GetService<ISyntaxProvider>();

            var value = await database.ExecuteScalarAsync<int?>($"select {syntax!.Delimit("CategoryID")} from categories where {syntax!.Delimit("CategoryID")}=1");

            Assert.AreEqual(1, value);
        }

        /// <summary>
        /// 执行 ExecuteScalarAsync，返回枚举值
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestExecuteScalarForEnumAsync()
        {
            var factory = ServiceProvider.GetRequiredService<IDatabaseFactory>();

            await using var database = factory.CreateDatabase<T>(ConnectionString);
            var syntax = database.GetService<ISyntaxProvider>();

            var value = await database.ExecuteScalarAsync<ShipVia?>($"select {syntax!.Delimit("ShipVia")} from orders where {syntax!.Delimit("OrderID")}=10248");

            Assert.AreEqual(ShipVia.C, value);
        }

        /// <summary>
        /// 测试加密完整的字符串
        /// </summary>
        [TestMethod]
        public void TestEncryptFullConnectionString()
        {
            var encryptor = ServiceProvider.GetService<IConnectionStringProtector>();
            var connStr = encryptor.Encrypt(ConnectionString, ConnectionStringProtectMode.Full);

            Console.WriteLine(connStr);

            connStr = encryptor.Decrypt(connStr);

            Console.WriteLine(connStr);

            Assert.IsTrue(connStr == ConnectionString);
        }

        /// <summary>
        /// 测试加密部分参数
        /// </summary>
        [TestMethod]
        public void TestEncryptConnectionString()
        {
            var encryptor = ServiceProvider.GetService<IConnectionStringProtector>();
            var connStr = encryptor.Encrypt(ConnectionString, ConnectionStringProtectMode.Server | ConnectionStringProtectMode.Password);

            Console.WriteLine(connStr);

            connStr = encryptor.Decrypt(connStr);

            Console.WriteLine(connStr);

            Assert.IsTrue(connStr == ConnectionString);
        }

        /// <summary>
        /// 测试加密部分参数并尝试打开
        /// </summary>
        [TestMethod]
        public async Task TestEncryptConnectionStringTryOpen()
        {
            var encryptor = ServiceProvider.GetService<IConnectionStringProtector>();
            var connStr = encryptor.Encrypt(ConnectionString, ConnectionStringProtectMode.Server | ConnectionStringProtectMode.Password);

            var factory = ServiceProvider.GetRequiredService<IDatabaseFactory>();

            await using var database = factory.CreateDatabase<T>((string)connStr);
            var exp = await database.TryConnectAsync();

            Assert.IsNull(exp);
        }

        public class Customers
        {
            public string CustomerID { get; set; }

            public string CompanyName { get; set; }
        }

        public enum ShipVia
        {
            A = 1,
            B = 2,
            C = 3
        }

        public class TestDIDatabase
        {
            private readonly IDatabase _database;

            public TestDIDatabase(IDatabase database)
            {
                _database = database;
            }

            public bool AreEqual(IDatabase database)
            {
                return database == _database;
            }
        }
    }
}
