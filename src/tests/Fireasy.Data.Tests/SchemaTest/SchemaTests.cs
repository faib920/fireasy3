using Fireasy.Common.Extensions;
using Fireasy.Data.RecordWrapper;
using Microsoft.Extensions.DependencyInjection;
using System.Data.Entity;
using static Azure.Core.HttpHeader;

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

            await using var database = factory.CreateDatabase<T>(ConnectionString);
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

            await using var database = factory.CreateDatabase<T>(ConnectionString);
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

            await using var database = factory.CreateDatabase<T>(ConnectionString);
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

            await using var database = factory.CreateDatabase<T>(ConnectionString);
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
        public async Task TestGetTablesByNameAsync()
        {
            var factory = ServiceProvider.GetRequiredService<IDatabaseFactory>();

            await using var database = factory.CreateDatabase<T>(ConnectionString);
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
        /// 测试获取 Table
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestGetTablesByNamesAsync()
        {
            var factory = ServiceProvider.GetRequiredService<IDatabaseFactory>();

            await using var database = factory.CreateDatabase<T>(ConnectionString);
            var syntax = database.GetService<ISyntaxProvider>();
            var schema = database.GetService<ISchemaProvider>();

            var names = new[]
            {
                syntax!.ToggleCase("products"),
                syntax!.ToggleCase("customers"),
                syntax!.ToggleCase("orders")
            };

            var tables = await schema!.GetSchemasAsync<Data.Schema.Table>(database, s => names.Contains(s.Name)).ToListAsync();

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

            await using var database = factory.CreateDatabase<T>(ConnectionString);
            var syntax = database.GetService<ISyntaxProvider>();
            var schema = database.GetService<ISchemaProvider>();

            var columns = await schema!.GetSchemasAsync<Data.Schema.Column>(database).ToListAsync();

            var columns1 = await schema!.GetSchemasAsync<Data.Schema.Column>(database).ToListAsync();

            foreach (var item in columns)
            {
                Console.WriteLine($"{item.TableName}.{item.Name},pk:{item.IsPrimaryKey},type:{item.DataType},clrtype:{item.ClrType?.Name}");
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

            await using var database = factory.CreateDatabase<T>(ConnectionString);
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
        /// 测试获取 Column
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestGetColumnsByNamesAsync()
        {
            var factory = ServiceProvider.GetRequiredService<IDatabaseFactory>();

            await using var database = factory.CreateDatabase<T>(ConnectionString);
            var syntax = database.GetService<ISyntaxProvider>();
            var schema = database.GetService<ISchemaProvider>();

            var tames = new[]
            {
                syntax!.ToggleCase("products"),
                syntax!.ToggleCase("customers"),
                syntax!.ToggleCase("orders")
            };

            var cnames = new[]
            {
                "ProductID",
                "ProductName"
            };

            var columns = await schema!.GetSchemasAsync<Data.Schema.Column>(database, s => tames.Contains(s.TableName) && cnames.Contains(s.Name)).ToListAsync();

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

            await using var database = factory.CreateDatabase<T>(ConnectionString);
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

            await using var database = factory.CreateDatabase<T>(ConnectionString);
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
        /// 测试获取 View
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestGetViewsByNamesAsync()
        {
            var factory = ServiceProvider.GetRequiredService<IDatabaseFactory>();

            await using var database = factory.CreateDatabase<T>(ConnectionString);
            var syntax = database.GetService<ISyntaxProvider>();
            var schema = database.GetService<ISchemaProvider>();

            var names = new[]
            {
                syntax!.ToggleCase("invoices"),
                syntax!.ToggleCase("customers"),
                syntax!.ToggleCase("orders")
            };

            var views = await schema!.GetSchemasAsync<Data.Schema.View>(database, s => names.Contains(s.Name)).ToListAsync();

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

            await using var database = factory.CreateDatabase<T>(ConnectionString);
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

            await using var database = factory.CreateDatabase<T>(ConnectionString);
            var syntax = database.GetService<ISyntaxProvider>();
            var schema = database.GetService<ISchemaProvider>();

            var cnames = new[]
            {
                "OrderID",
                "CustomerID"
            };

            var columns = await schema!.GetSchemasAsync<Data.Schema.ViewColumn>(database, s => s.ViewName == syntax!.ToggleCase("invoices") && cnames.Contains(s.Name)).ToListAsync();

            foreach (var item in columns)
            {
                Console.WriteLine(item.Name);
            }

            Assert.IsTrue(columns.Any());
        }

        /// <summary>
        /// 测试获取 Column
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestGetAllDataTypesAsync()
        {
            var factory = ServiceProvider.GetRequiredService<IDatabaseFactory>();

            await using var database = factory.CreateDatabase<T>(ConnectionString);
            var syntax = database.GetService<ISyntaxProvider>();
            var schema = database.GetService<ISchemaProvider>();
            var recordWrapper = database.GetService<IRecordWrapper>();

            var columns = await schema!.GetSchemasAsync<Data.Schema.Column>(database, s => s.TableName == syntax!.ToggleCase("all_datatypes")).ToListAsync();

            foreach (var item in columns)
            {
                Console.WriteLine($"{item.TableName}.{item.Name},pk:{item.IsPrimaryKey},datatype:{item.DataType},dbtype:{item.DbType},clrtype:{item.ClrType}");
            }

            using var reader = await database.ExecuteReaderAsync("select * from all_datatypes");
            while (reader.Read())
            {
                for (var i = 0; i < reader.FieldCount; i++) 
                {
                    var fieldType = recordWrapper!.GetFieldType(reader, i);
                    Assert.AreEqual(columns[i].ClrType, fieldType, columns[i].DataType);
                    Console.WriteLine(columns[i].DataType + " " + recordWrapper.GetValue(reader, i));
                }
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

            await using var database = factory.CreateDatabase<T>(ConnectionString);
            var syntax = database.GetService<ISyntaxProvider>();
            var schema = database.GetService<ISchemaProvider>();

            var foreignKeys = await schema!.GetSchemasAsync<Data.Schema.ForeignKey>(database, s => s.TableName.Equals(syntax!.ToggleCase("orders"))).ToListAsync();

            foreach (var item in foreignKeys)
            {
                Console.WriteLine($"{item.Name} {item.TableName}.{item.ColumnName} -> {item.PKTable}.{item.PKColumn}");
            }

            Assert.IsTrue(foreignKeys.Any());
        }

        /// <summary>
        /// 测试获取 ForeignKey
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestGetForeignKeysByTableNamesAsync()
        {
            var factory = ServiceProvider.GetRequiredService<IDatabaseFactory>();

            await using var database = factory.CreateDatabase<T>(ConnectionString);
            var syntax = database.GetService<ISyntaxProvider>();
            var schema = database.GetService<ISchemaProvider>();

            var tames = new[]
            {
                syntax!.ToggleCase("products"),
                syntax!.ToggleCase("customers"),
                syntax!.ToggleCase("orders")
            };

            var foreignKeys = await schema!.GetSchemasAsync<Data.Schema.ForeignKey>(database, s => tames.Contains(s.TableName)).ToListAsync();

            foreach (var item in foreignKeys)
            {
                Console.WriteLine($"{item.Name} {item.TableName}.{item.ColumnName} -> {item.PKTable}.{item.PKColumn}");
            }

            Assert.IsTrue(foreignKeys.Any());
        }

        /// <summary>
        /// 测试获取 Procedure
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestGetProceduresAsync()
        {
            var factory = ServiceProvider.GetRequiredService<IDatabaseFactory>();

            await using var database = factory.CreateDatabase<T>(ConnectionString);
            var schema = database.GetService<ISchemaProvider>();

            var procedures = await schema!.GetSchemasAsync<Data.Schema.Procedure>(database).ToListAsync();

            foreach (var item in procedures)
            {
                Console.WriteLine($"{item.Name}");
            }

            Assert.IsTrue(procedures.Any());
        }

        /// <summary>
        /// 测试获取 Procedure
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestGetProceduresByNamesAsync()
        {
            var factory = ServiceProvider.GetRequiredService<IDatabaseFactory>();

            await using var database = factory.CreateDatabase<T>(ConnectionString);
            var syntax = database.GetService<ISyntaxProvider>();
            var schema = database.GetService<ISchemaProvider>();

            var tames = new[]
            {
                syntax!.ToggleCase("CustOrderHist"),
                syntax!.ToggleCase("CustOrdersOrders")
            };

            var procedures = await schema!.GetSchemasAsync<Data.Schema.Procedure>(database, s => tames.Contains(s.Name)).ToListAsync();

            foreach (var item in procedures)
            {
                Console.WriteLine($"{item.Name}");
            }

            Assert.IsTrue(procedures.Any());
        }

        /// <summary>
        /// 测试获取 ProcedureParameter
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestGetProcedureParametersByNamesAsync()
        {
            var factory = ServiceProvider.GetRequiredService<IDatabaseFactory>();

            await using var database = factory.CreateDatabase<T>(ConnectionString);
            var syntax = database.GetService<ISyntaxProvider>();
            var schema = database.GetService<ISchemaProvider>();

            var tames = new[]
            {
                syntax!.ToggleCase("CustOrderHist"),
                syntax!.ToggleCase("CustOrdersOrders")
            };

            var parameters = await schema!.GetSchemasAsync<Data.Schema.ProcedureParameter>(database, s => tames.Contains(s.ProcedureName)).ToListAsync();

            foreach (var item in parameters)
            {
                Console.WriteLine($"{item.ProcedureName} {item.Name}");
            }

            Assert.IsTrue(parameters.Any());
        }

        /// <summary>
        /// 测试获取 Index
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestGetIndexsAsync()
        {
            var factory = ServiceProvider.GetRequiredService<IDatabaseFactory>();

            await using var database = factory.CreateDatabase<T>(ConnectionString);
            var schema = database.GetService<ISchemaProvider>();

            var indexes = await schema!.GetSchemasAsync<Data.Schema.Index>(database).ToListAsync();

            foreach (var item in indexes)
            {
                Console.WriteLine($"{item.TableName} {item.Name}");
            }

            Assert.IsTrue(indexes.Any());
        }

        /// <summary>
        /// 测试获取 IndexColumn
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestGetIndexColumnsAsync()
        {
            var factory = ServiceProvider.GetRequiredService<IDatabaseFactory>();

            await using var database = factory.CreateDatabase<T>(ConnectionString);
            var syntax = database.GetService<ISyntaxProvider>();
            var schema = database.GetService<ISchemaProvider>();

            var tames = new[]
            {
                syntax!.ToggleCase("products"),
                syntax!.ToggleCase("employees")
            };

            var columns = await schema!.GetSchemasAsync<Data.Schema.IndexColumn>(database, s => tames.Contains(s.TableName) && new[] { "LastName" }.Contains(s.ColumnName)).ToListAsync();

            foreach (var item in columns)
            {
                Console.WriteLine($"{item.TableName} {item.IndexName} {item.ColumnName}");
            }

            Assert.IsTrue(columns.Any());
        }

    }
}
