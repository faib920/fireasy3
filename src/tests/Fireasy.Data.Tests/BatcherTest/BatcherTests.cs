using Fireasy.Data.Batcher;
using Microsoft.Extensions.DependencyInjection;
using System.Data;

namespace Fireasy.Data.Tests.BatcherTest
{
    public abstract class BatcherTests<T> : DbInstanceBaseTests where T : IProvider
    {
        protected override void ConfigureBuilder(SetupBuilder builder)
        {
            base.ConfigureBuilder(builder);

            builder.ConfigureData(s => s.AddProivderFactory<MySqlProvider>(MySqlConnector.MySqlConnectorFactory.Instance));
        }

        /// <summary>
        /// 测试获取 <see cref="IBulkCopyProvider"/> 实例。
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestGetBulkCopyProvider()
        {
            var factory = ServiceProvider.GetRequiredService<IDatabaseFactory>();

            await using var database = factory.CreateDatabase<T>(ConnectionString);
            var bulkCopyProvider = database.GetService<IBulkCopyProvider>();

            Assert.IsNotNull(bulkCopyProvider);
        }

        [TestMethod]
        public async Task TestBatchInsertWithDataTable_10000()
        {
            var factory = ServiceProvider.GetRequiredService<IDatabaseFactory>();

            await using var database = factory.CreateDatabase<T>(ConnectionString);
            var batcher = database.GetService<IBatcherProvider>();
            var syntax = database.GetService<ISyntaxProvider>();

            await database.ExecuteNonQueryAsync($"delete from {syntax!.ToggleCase("batchers")}");

            var table = NewDataTable();

            for (var i = 0; i < 10000; i++)
            {
                table.Rows.Add(NewDataRow(i));
            }

            await batcher.InsertAsync(database, table);
        }

        [TestMethod]
        public async Task TestBatchInsertWithList_10000()
        {
            var factory = ServiceProvider.GetRequiredService<IDatabaseFactory>();

            await using var database = factory.CreateDatabase<T>(ConnectionString);
            var batcher = database.GetService<IBatcherProvider>();
            var syntax = database.GetService<ISyntaxProvider>();

            await database.ExecuteNonQueryAsync($"delete from {syntax!.ToggleCase("batchers")}");

            var list = new List<BatcherData>();

            for (var i = 0; i < 10000; i++)
            {
                list.Add(NewData(i));
            }

            await batcher.InsertAsync(database, list, "batchers");
        }

        [TestMethod]
        public async Task TestBatchInsertWithDataTable_100000()
        {
            var factory = ServiceProvider.GetRequiredService<IDatabaseFactory>();

            await using var database = factory.CreateDatabase<T>(ConnectionString);
            var batcher = database.GetService<IBatcherProvider>();
            var syntax = database.GetService<ISyntaxProvider>();

            await database.ExecuteNonQueryAsync($"delete from {syntax!.ToggleCase("batchers")}");

            var table = NewDataTable();

            for (var i = 0; i < 100000; i++)
            {
                table.Rows.Add(NewDataRow(i));
            }

            await batcher.InsertAsync(database, table);
        }

        [TestMethod]
        public async Task TestBatchInsertWithList_100000()
        {
            var factory = ServiceProvider.GetRequiredService<IDatabaseFactory>();

            await using var database = factory.CreateDatabase<T>(ConnectionString);
            var batcher = database.GetService<IBatcherProvider>();
            var syntax = database.GetService<ISyntaxProvider>();

            await database.ExecuteNonQueryAsync($"delete from {syntax!.ToggleCase("batchers")}");

            var list = new List<BatcherData>();

            for (var i = 0; i < 100000; i++)
            {
                list.Add(NewData(i));
            }

            await batcher.InsertAsync(database, list, "batchers");
        }

        private DataTable NewDataTable()
        {
            var table = new DataTable { TableName = "batchers" };
            table.Columns.Add("Id", typeof(int));
            table.Columns.Add("Name", typeof(string));
            table.Columns.Add("Address", typeof(string));
            table.Columns.Add("Ext1", typeof(string));
            table.Columns.Add("Ext2", typeof(string));
            table.Columns.Add("Ext3", typeof(string));
            table.Columns.Add("Ext4", typeof(string));
            table.Columns.Add("Ext5", typeof(string));
            table.Columns.Add("Ext6", typeof(string));
            table.Columns.Add("Ext7", typeof(string));
            table.Columns.Add("Ext8", typeof(string));
            table.Columns.Add("Ext9", typeof(string));
            table.Columns.Add("Ext10", typeof(string));
            table.Columns.Add("ExtB1", typeof(byte[]));
            table.Columns.Add("ExtB2", typeof(byte[]));
            table.Columns.Add("ExtB3", typeof(byte[]));
            table.Columns.Add("ExtB4", typeof(byte[]));
            table.Columns.Add("ExtB5", typeof(byte[]));

            return table;
        }

        private object[] NewDataRow(int i)
        {
            return new object[] { i + 1, "Name" + i, "Address" + i, "ext1", "ext2", "ext3", "ext4", "ext5", "ext6", "ext7", "ext8", "ext9", "ext10", NewBytes(), NewBytes(), NewBytes(), NewBytes(), NewBytes() };
        }

        private class BatcherData
        {
            public BatcherData(int id, string name, string address, string ext1, string ext2, string ext3, string ext4, string ext5, string ext6, string ext7, string ext8, string ext9, string ext10, byte[] extB1, byte[] extB2, byte[] extB3, byte[] extB4, byte[] extB5)
            {
                Id = id;
                Name = name;
                Address = address;
                Ext1 = ext1;
                Ext2 = ext2;
                Ext3 = ext3;
                Ext4 = ext4;
                Ext5 = ext5;
                Ext6 = ext6;
                Ext7 = ext7;
                Ext8 = ext8;
                Ext9 = ext9;
                Ext10 = ext10;
                ExtB1 = extB1;
                ExtB2 = extB2;
                ExtB3 = extB3;
                ExtB4 = extB4;
                ExtB5 = extB5;
            }

            public int Id { get; set; }
            public string Name { get; set; }
            public string Address { get; set; }
            public string Ext1 { get; set; }
            public string Ext2 { get; set; }
            public string Ext3 { get; set; }
            public string Ext4 { get; set; }
            public string Ext5 { get; set; }
            public string Ext6 { get; set; }
            public string Ext7 { get; set; }
            public string Ext8 { get; set; }
            public string Ext9 { get; set; }
            public string Ext10 { get; set; }
            public byte[] ExtB1 { get; set; }
            public byte[] ExtB2 { get; set; }
            public byte[] ExtB3 { get; set; }
            public byte[] ExtB4 { get; set; }
            public byte[] ExtB5 { get; set; }
        }

        private BatcherData NewData(int i)
        {
            return new BatcherData(i + 1, "Name" + i, "Address" + i, "ext1", "ext2", "ext3", "ext4", "ext5", "ext6", "ext7", "ext8", "ext9", "ext10", NewBytes(), NewBytes(), NewBytes(), NewBytes(), NewBytes());
        }

        private byte[] NewBytes()
        {
            var r = new Random();
            byte[] buffer = new byte[128];
            r.NextBytes(buffer);

            return buffer;
        }
    }
}
