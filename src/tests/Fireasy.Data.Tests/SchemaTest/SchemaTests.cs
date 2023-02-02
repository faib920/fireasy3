/// 测试获取 ViewColumn
using Fireasy.Common.Extensions;
using Microsoft.Extensions.DependencyInjection;
using System.Data.Entity;

namespace Fireasy.Data.Tests.SchemaTest
{
    /// <summary>
    /// <see cref="ISchemaProvider"/> 测试类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class SchemaTests<T> : DbInstanceBaseTests where T : IProvider
    {
        /// <summary>
        /// 测试获取 Database
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestGetDatabaseAsync()
        {
            var factory = ServiceProvider.GetRequiredService<IDatabaseFactory>();

            using var database = factory.CreateDatabase<T>(ConnectionString);
            var schema = database.GetService<ISchemaProvider>();

            var databases = await schema!.GetSchemasAsync<Data.Schema.Database>(database).ToListAsync();

            foreach (var item in databases)
            {
                Console.WriteLine(item.Name);
            }

            Assert.IsTrue(databases.Any());
        }

        /// <summary>
        /// 测试获取 User
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestGetUsersAsync()
        {
            var factory = ServiceProvider.GetRequiredService<IDatabaseFactory>();

            using var database = factory.CreateDatabase<T>(ConnectionString);
            var schema = database.GetService<ISchemaProvider>();

            var users = await schema!.GetSchemasAsync<Data.Schema.User>(database).ToListAsync();

            foreach (var item in users)
            {
                Console.WriteLine(item.Name);
            }

            Assert.IsTrue(users.Any());
        }

        /// <summary>
        /// 测试获取 DataType
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestGetDataTypesAsync()
        {
            var factory = ServiceProvider.GetRequiredService<IDatabaseFactory>();

            using var database = factory.CreateDatabase<T>(ConnectionString);
            var schema = database.GetService<ISchemaProvider>();

            var tables = await schema!.GetSchemasAsync<Data.Schema.DataType>(database).ToListAsync();

            foreach (var item in tables)
            {
                Console.WriteLine($"{item.Name}->{item.DbType}");
            }

            Assert.IsTrue(tables.Any());
        }

        /// <summary>
        /// 测试获取 Table
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestGetTablesAsync()
        {
            var factory = ServiceProvider.GetRequiredService<IDatabaseFactory>();

            using var database = factory.CreateDatabase<T>(ConnectionString);
            var schema = database.GetService<ISchemaProvider>();

            var tables = await schema!.GetSchemasAsync<Data.Schema.Table>(database).ToListAsync();

            foreach (var item in tables)
            {
                Console.WriteLine($"{item.Name}");
            }

            Assert.IsTrue(tables.Any());
        }

        /// <summary>
        /// 测试获取 Table
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestGetTablesPredicateAsync()
        {
            var factory = ServiceProvider.GetRequiredService<IDatabaseFactory>();

            using var database = factory.CreateDatabase<T>(ConnectionString);
            var syntax = database.GetService<ISyntaxProvider>();
            var schema = database.GetService<ISchemaProvider>();

            var tables = await schema!.GetSchemasAsync<Data.Schema.Table>(database, s => s.Name == syntax!.ToggleCase("products")).ToListAsync();

            foreach (var item in tables)
            {
                Console.WriteLine(item.Name);
            }

            Assert.IsTrue(tables.Any());
        }

        /// <summary>
        /// 测试获取 Column
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestGetColumnsAsync()
        {
            var factory = ServiceProvider.GetRequiredService<IDatabaseFactory>();

            using var database = factory.CreateDatabase<T>(ConnectionString);
            var syntax = database.GetService<ISyntaxProvider>();
            var schema = database.GetService<ISchemaProvider>();

            var columns = await schema!.GetSchemasAsync<Data.Schema.Column>(database, s => s.TableName == syntax!.ToggleCase("products")).ToListAsync();

            foreach (var item in columns)
            {
                Console.WriteLine($"{item.TableName}.{item.Name},pk:{item.IsPrimaryKey},type:{item.DataType}");
            }

            Assert.IsTrue(columns.Any());
        }

        /// <summary>
        /// 测试获取 Column
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestGetColumnsByNameAsync()
        {
            var factory = ServiceProvider.GetRequiredService<IDatabaseFactory>();

            using var database = factory.CreateDatabase<T>(ConnectionString);
            var syntax = database.GetService<ISyntaxProvider>();
            var schema = database.GetService<ISchemaProvider>();

            var columns = await schema!.GetSchemasAsync<Data.Schema.Column>(database, s => s.TableName == syntax!.ToggleCase("products") && s.Name == "ProductID").ToListAsync();

            foreach (var item in columns)
            {
                Console.WriteLine($"{item.TableName}.{item.Name},pk:{item.IsPrimaryKey},type:{item.DataType}");
            }

            Assert.IsTrue(columns.Any());
        }

        /// <summary>
        /// 测试获取 View
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestGetViewsAsync()
        {
            var factory = ServiceProvider.GetRequiredService<IDatabaseFactory>();

            using var database = factory.CreateDatabase<T>(ConnectionString);
            var schema = database.GetService<ISchemaProvider>();

            var views = await schema!.GetSchemasAsync<Data.Schema.View>(database).ToListAsync();

            foreach (var item in views)
            {
                Console.WriteLine(item.Name);
            }

            Assert.IsTrue(views.Any());
        }

        /// <summary>
        /// 测试获取 View
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestGetViewsByNameAsync()
        {
            var factory = ServiceProvider.GetRequiredService<IDatabaseFactory>();

            using var database = factory.CreateDatabase<T>(ConnectionString);
            var syntax = database.GetService<ISyntaxProvider>();
            var schema = database.GetService<ISchemaProvider>();

            var views = await schema!.GetSchemasAsync<Data.Schema.View>(database, s => s.Name == syntax!.ToggleCase("invoices")).ToListAsync();

            foreach (var item in views)
            {
                Console.WriteLine(item.Name);
            }

            Assert.IsTrue(views.Any());
        }

        /// <summary>
        /// 测试获取 ViewColumn
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestGetViewsColumnsAsync()
        {
            var factory = ServiceProvider.GetRequiredService<IDatabaseFactory>();

            using var database = factory.CreateDatabase<T>(ConnectionString);
            var syntax = database.GetService<ISyntaxProvider>();
            var schema = database.GetService<ISchemaProvider>();

            var columns = await schema!.GetSchemasAsync<Data.Schema.ViewColumn>(database, s => s.ViewName == syntax!.ToggleCase("invoices")).ToListAsync();

            foreach (var item in columns)
            {
                Console.WriteLine(item.Name);
            }

            Assert.IsTrue(columns.Any());
        }

        /// <summary>
        /// 测试获取 ViewColumn
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestGetViewsColumnsByNameAsync()
        {
            var factory = ServiceProvider.GetRequiredService<IDatabaseFactory>();

            using var database = factory.CreateDatabase<T>(ConnectionString);
            var syntax = database.GetService<ISyntaxProvider>();
            var schema = database.GetService<ISchemaProvider>();

            var columns = await schema!.GetSchemasAsync<Data.Schema.ViewColumn>(database, s => s.ViewName == syntax!.ToggleCase("invoices") && s.Name == "OrderID").ToListAsync();

            foreach (var item in columns)
            {
                Console.WriteLine(item.Name);
            }

            Assert.IsTrue(columns.Any());
        }

        /// <summary>
        /// 测试获取 ForeignKey
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestGetForeignKeysAsync()
        {
            var factory = ServiceProvider.GetRequiredService<IDatabaseFactory>();

            using var database = factory.CreateDatabase<T>(ConnectionString);
            var syntax = database.GetService<ISyntaxProvider>();
            var schema = database.GetService<ISchemaProvider>();

            var foreignKeys = await schema!.GetSchemasAsync<Data.Schema.ForeignKey>(database, s => s.TableName == syntax!.ToggleCase("orders")).ToListAsync();

            foreach (var item in foreignKeys)
            {
                Console.WriteLine($"{item.Name} {item.TableName}.{item.ColumnName} -> {item.PKTable}.{item.PKColumn}");
            }

            Assert.IsTrue(foreignKeys.Any());
        }

    }
}
