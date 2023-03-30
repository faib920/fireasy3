using Microsoft.Extensions.DependencyInjection;
using System.Data;
using System.Text;

namespace Fireasy.Data.Tests.SyntaxTest
{
    /// <summary>
    /// <see cref="ISyntaxProvider"/> 测试类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class SyntaxTests<T> : DbInstanceBaseTests where T : IProvider
    {
        /// <summary>
        /// 测试 IdentityValue
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestIdentityValueAsync()
        {
            var factory = ServiceProvider.GetRequiredService<IDatabaseFactory>();

            await using var database = factory.CreateDatabase<T>(ConnectionString);
            var syntax = database.GetService<ISyntaxProvider>();

            if (!string.IsNullOrEmpty(syntax!.IdentityValue))
            {
                var sql = $"{syntax.IdentityValue}";

                var ret = await database.ExecuteScalarAsync<int>(sql);
                Assert.AreEqual(0, ret);
            }
            else
            {
                throw new SyntaxNotSupportedException("IdentityValue");
            }
        }

        /// <summary>
        /// 测试 NewGuid
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestNewGuidAsync()
        {
            var factory = ServiceProvider.GetRequiredService<IDatabaseFactory>();

            await using var database = factory.CreateDatabase<T>(ConnectionString);
            var syntax = database.GetService<ISyntaxProvider>();

            if (!string.IsNullOrEmpty(syntax!.NewGuid))
            {
                var sql = $"{syntax.NewGuid}";

                var ret = await database.ExecuteScalarAsync<Guid>(sql);
                Assert.IsTrue(ret != Guid.Empty);
            }
            else
            {
                throw new SyntaxNotSupportedException("NewGuid");
            }
        }

        /// <summary>
        /// 测试 FakeTable
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestFakeSelectAsync()
        {
            var factory = ServiceProvider.GetRequiredService<IDatabaseFactory>();

            await using var database = factory.CreateDatabase<T>(ConnectionString);
            var syntax = database.GetService<ISyntaxProvider>();

            var sql = $"select 120{syntax!.FakeTable}";

            var ret = await database.ExecuteScalarAsync<int>(sql);
            Assert.AreEqual(120, ret);
        }

        /// <summary>
        /// 测试 Segment
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestSegmentAsync()
        {
            var factory = ServiceProvider.GetRequiredService<IDatabaseFactory>();

            await using var database = factory.CreateDatabase<T>(ConnectionString);
            var syntax = database.GetService<ISyntaxProvider>();

            var sql = syntax!.Segment($"select * from customers", new DataPager(10, 0));

            var list = await database.ExecuteEnumerableAsync(sql);
        }

        /// <summary>
        /// 测试 Coalesce
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestCoalesceAsync()
        {
            var factory = ServiceProvider.GetRequiredService<IDatabaseFactory>();

            await using var database = factory.CreateDatabase<T>(ConnectionString);
            var syntax = database.GetService<ISyntaxProvider>();

            var sql = $"select {syntax!.Coalesce(syntax.Delimit("Region"), syntax.Delimit("City"))} from customers";

            var list = await database.ExecuteEnumerableAsync(sql);
        }

        /// <summary>
        /// 测试 ToggleCase
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestToggleCaseAsync()
        {
            var factory = ServiceProvider.GetRequiredService<IDatabaseFactory>();

            await using var database = factory.CreateDatabase<T>(ConnectionString);
            var syntax = database.GetService<ISyntaxProvider>();

            var sql = $"select {syntax!.Delimit("Quantity")} from {syntax.Delimit(syntax.ToggleCase("Order Details"))}";

            var _ = await database.ExecuteScalarAsync<decimal>(sql);
        }

        /// <summary>
        /// 测试 ParameterPrefix
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestParameterPrefixAsync()
        {
            var factory = ServiceProvider.GetRequiredService<IDatabaseFactory>();

            await using var database = factory.CreateDatabase<T>(ConnectionString);
            var syntax = database.GetService<ISyntaxProvider>();

            var parameters = new ParameterCollection();
            parameters.Add("customerId", "ALFKI");

            var sql = $"select count(1) from customers where {syntax!.Delimit("CustomerID")} = {syntax!.ParameterPrefix}customerId";

            var _ = await database.ExecuteScalarAsync<int>(sql, parameters);
        }

        /// <summary>
        /// 测试 FormatParameter
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestFormatParameterPrefixAsync()
        {
            var factory = ServiceProvider.GetRequiredService<IDatabaseFactory>();

            await using var database = factory.CreateDatabase<T>(ConnectionString);
            var syntax = database.GetService<ISyntaxProvider>();

            var parameters = new ParameterCollection();
            parameters.Add("customerId", "ALFKI");

            var sql = $"select count(1) from customers where {syntax!.Delimit("CustomerID")} = {syntax.FormatParameter("customerId")}";

            var _ = await database.ExecuteScalarAsync<int>(sql, parameters);
        }

        /// <summary>
        /// 测试 Delimiter
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestDelimiterAsync()
        {
            var factory = ServiceProvider.GetRequiredService<IDatabaseFactory>();

            await using var database = factory.CreateDatabase<T>(ConnectionString);
            var syntax = database.GetService<ISyntaxProvider>();

            var sql = $"select {syntax!.Delimiter[0] + "CustomerID" + syntax!.Delimiter[1]} from customers";

            var _ = await database.ExecuteScalarAsync<string>(sql);
        }

        /// <summary>
        /// 测试 ExistsTable
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestExistsTableAsync()
        {
            var factory = ServiceProvider.GetRequiredService<IDatabaseFactory>();

            await using var database = factory.CreateDatabase<T>(ConnectionString);
            var syntax = database.GetService<ISyntaxProvider>();

            var sql = syntax!.ExistsTable("customers");

            var count = await database.ExecuteScalarAsync<int>(sql);

            Assert.AreEqual(1, count);
        }

        /// <summary>
        /// 测试转换为string
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestConvertToStringAsync()
        {
            var factory = ServiceProvider.GetRequiredService<IDatabaseFactory>();

            await using var database = factory.CreateDatabase<T>(ConnectionString);
            var syntax = database.GetService<ISyntaxProvider>();

            var sql = $"select {syntax!.Convert(syntax.Delimit("ShipVia"), DbType.String)} from orders where {syntax.Delimit("OrderID")}=10248";

            var ret = await database.ExecuteScalarAsync<string>(sql);

            Assert.AreEqual("3", ret);
        }

        /// <summary>
        /// 测试转换为boolean
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestConvertToBooleanAsync()
        {
            var factory = ServiceProvider.GetRequiredService<IDatabaseFactory>();

            await using var database = factory.CreateDatabase<T>(ConnectionString);
            var syntax = database.GetService<ISyntaxProvider>();

            var sql = $"select {syntax!.Convert(syntax.Delimit("Discontinued"), DbType.Boolean)} from products where {syntax.Delimit("ProductID")}=5";

            var ret = await database.ExecuteScalarAsync<bool>(sql);

            Assert.AreEqual(true, ret);
        }

        /// <summary>
        /// 测试转换为byte
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestConvertToByteAsync()
        {
            var factory = ServiceProvider.GetRequiredService<IDatabaseFactory>();

            await using var database = factory.CreateDatabase<T>(ConnectionString);
            var syntax = database.GetService<ISyntaxProvider>();

            var sql = $"select {syntax!.Convert(syntax.Delimit("Freight"), DbType.Byte)} from orders where {syntax.Delimit("OrderID")}=10248";

            var ret = await database.ExecuteScalarAsync<byte>(sql);

            Assert.AreEqual((byte)32, ret);
        }

        /// <summary>
        /// 测试转换为short
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestConvertToShortAsync()
        {
            var factory = ServiceProvider.GetRequiredService<IDatabaseFactory>();

            await using var database = factory.CreateDatabase<T>(ConnectionString);
            var syntax = database.GetService<ISyntaxProvider>();

            var sql = $"select {syntax!.Convert(syntax.Delimit("Freight"), DbType.Int16)} from orders where {syntax.Delimit("OrderID")} =10248";

            var ret = await database.ExecuteScalarAsync<short>(sql);

            Assert.AreEqual((short)32, ret);
        }

        /// <summary>
        /// 测试转换为decimal
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestConvertToDecimalAsync()
        {
            var factory = ServiceProvider.GetRequiredService<IDatabaseFactory>();

            await using var database = factory.CreateDatabase<T>(ConnectionString);
            var syntax = database.GetService<ISyntaxProvider>();

            var sql = $"select {syntax!.Convert("'34.6'", DbType.Decimal)}{syntax.FakeTable}";

            var ret = await database.ExecuteScalarAsync<decimal>(sql);

            Assert.AreEqual(34.6m, ret);
        }

        /// <summary>
        /// 测试转换为long
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestConvertToLongAsync()
        {
            var factory = ServiceProvider.GetRequiredService<IDatabaseFactory>();

            await using var database = factory.CreateDatabase<T>(ConnectionString);
            var syntax = database.GetService<ISyntaxProvider>();

            var sql = $"select {syntax!.Convert(syntax.Delimit("Freight"), DbType.Int64)} from orders where {syntax.Delimit("OrderID")}=10248";

            var ret = await database.ExecuteScalarAsync<long>(sql);

            Assert.AreEqual(32L, ret);
        }

        /// <summary>
        /// 测试转换为float
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestConvertToSingleAsync()
        {
            var factory = ServiceProvider.GetRequiredService<IDatabaseFactory>();

            await using var database = factory.CreateDatabase<T>(ConnectionString);
            var syntax = database.GetService<ISyntaxProvider>();

            var sql = $"select {syntax!.Convert(syntax.Delimit("ShipVia"), DbType.Single)} from orders where {syntax.Delimit("OrderID")}=10248";

            var ret = await database.ExecuteScalarAsync<float>(sql);

            Assert.AreEqual(3f, ret);
        }

        /// <summary>
        /// 测试转换为float
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestConvertToDoubleAsync()
        {
            var factory = ServiceProvider.GetRequiredService<IDatabaseFactory>();

            await using var database = factory.CreateDatabase<T>(ConnectionString);
            var syntax = database.GetService<ISyntaxProvider>();

            var sql = $"select {syntax!.Convert(syntax.Delimit("ShipVia"), DbType.Double)} from orders where {syntax.Delimit("OrderID")}=10248";

            var ret = await database.ExecuteScalarAsync<double>(sql);

            Assert.AreEqual(3d, ret);
        }

        /// <summary>
        /// 测试创建列
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestCreateColumnsAsync()
        {
            var factory = ServiceProvider.GetRequiredService<IDatabaseFactory>();

            await using var database = factory.CreateDatabase<T>(ConnectionString);
            var syntax = database.GetService<ISyntaxProvider>();

            var sb = new StringBuilder();
            sb.AppendLine("create table tmp_1(");
            sb.AppendLine($"c1 {syntax!.Column(DbType.Int16)},");
            sb.AppendLine($"c2 {syntax.Column(DbType.Int32)} NOT NULL PRIMARY KEY {syntax.IdentityColumn},");
            sb.AppendLine($"c3 {syntax.Column(DbType.Int64)},");
            sb.AppendLine($"c4 {syntax.Column(DbType.Boolean)},");
            sb.AppendLine($"c5 {syntax.Column(DbType.Byte)},");
            sb.AppendLine($"c6 {syntax.Column(DbType.Date)},");
            sb.AppendLine($"c7 {syntax.Column(DbType.DateTime)},");
            sb.AppendLine($"c8 {syntax.Column(DbType.Time)},");
            sb.AppendLine($"c9 {syntax.Column(DbType.DateTimeOffset)},");
            sb.AppendLine($"c10 {syntax.Column(DbType.Single)},");
            sb.AppendLine($"c11 {syntax.Column(DbType.Double)},");
            sb.AppendLine($"c12 {syntax.Column(DbType.Decimal)},");
            sb.AppendLine($"c13 {syntax.Column(DbType.Currency)},");
            sb.AppendLine($"c14 {syntax.Column(DbType.Guid)},");
            sb.AppendLine($"c15 {syntax.Column(DbType.StringFixedLength, 200)},");
            sb.AppendLine($"c16 {syntax.Column(DbType.Binary)},");
            sb.AppendLine($"c17 {syntax.Column(DbType.Xml)}");
            sb.AppendLine(")");

            var ret = await database.ExecuteNonQueryAsync(sb.ToString());

            await database.ExecuteNonQueryAsync("drop table tmp_1");
        }

        /// <summary>
        /// 测试 String.Length
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestStringLengthAsync()
        {
            var factory = ServiceProvider.GetRequiredService<IDatabaseFactory>();

            await using var database = factory.CreateDatabase<T>(ConnectionString);
            var syntax = database.GetService<ISyntaxProvider>();

            var sql = $"select {syntax!.String.Length(syntax.Delimit("CustomerID"))} from customers where {syntax.Delimit("CustomerID")}='ALFKI'";

            var ret = await database.ExecuteScalarAsync<int>(sql);

            Assert.AreEqual(5, ret);
        }

        /// <summary>
        /// 测试 String.Substring
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestStringSubstringAsync()
        {
            var factory = ServiceProvider.GetRequiredService<IDatabaseFactory>();

            await using var database = factory.CreateDatabase<T>(ConnectionString);
            var syntax = database.GetService<ISyntaxProvider>();

            var sql = $"select {syntax!.String.Substring(syntax.Delimit("CustomerID"), 2, 2)} from customers where {syntax.Delimit("CustomerID")}='ALFKI'";

            var ret = await database.ExecuteScalarAsync<string>(sql);

            Assert.AreEqual("LF", ret);
        }

        /// <summary>
        /// 测试 String.Substring
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestStringSubstringToEndAsync()
        {
            var factory = ServiceProvider.GetRequiredService<IDatabaseFactory>();

            await using var database = factory.CreateDatabase<T>(ConnectionString);
            var syntax = database.GetService<ISyntaxProvider>();

            var sql = $"select {syntax!.String.Substring(syntax.Delimit("CustomerID"), 2)} from customers where {syntax.Delimit("CustomerID")}='ALFKI'";

            var ret = await database.ExecuteScalarAsync<string>(sql);

            Assert.AreEqual("LFKI", ret);
        }

        /// <summary>
        /// 测试 String.Concat
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestStringConcatAsync()
        {
            var factory = ServiceProvider.GetRequiredService<IDatabaseFactory>();

            await using var database = factory.CreateDatabase<T>(ConnectionString);
            var syntax = database.GetService<ISyntaxProvider>();

            var sql = $"select {syntax!.String.Concat(syntax.Delimit("CustomerID"), "'-'", syntax.Delimit("City"))} from customers where {syntax.Delimit("CustomerID")}='ALFKI'";

            var ret = await database.ExecuteScalarAsync<string>(sql);

            Assert.AreEqual("ALFKI-Berlin", ret);
        }

        /// <summary>
        /// 测试 String.PadLeft
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestStringPadLeftAsync()
        {
            var factory = ServiceProvider.GetRequiredService<IDatabaseFactory>();

            await using var database = factory.CreateDatabase<T>(ConnectionString);
            var syntax = database.GetService<ISyntaxProvider>();

            var sql = $"select {syntax!.String.PadLeft(syntax.Delimit("CustomerID"), 7, "'-'")} from customers where {syntax.Delimit("CustomerID")}='ALFKI'";

            var ret = await database.ExecuteScalarAsync<string>(sql);

            Assert.AreEqual("--ALFKI", ret);
        }

        /// <summary>
        /// 测试 String.PadLeft
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestStringPadLeftLessAsync()
        {
            var factory = ServiceProvider.GetRequiredService<IDatabaseFactory>();

            await using var database = factory.CreateDatabase<T>(ConnectionString);
            var syntax = database.GetService<ISyntaxProvider>();

            var sql = $"select {syntax!.String.PadLeft(syntax.Delimit("CustomerID"), 2, "'-'")} from customers where {syntax.Delimit("CustomerID")}='ALFKI'";

            var ret = await database.ExecuteScalarAsync<string>(sql);

            Assert.AreEqual("AL", ret);
        }

        /// <summary>
        /// 测试 String.PadRight
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestStringPadRightAsync()
        {
            var factory = ServiceProvider.GetRequiredService<IDatabaseFactory>();

            await using var database = factory.CreateDatabase<T>(ConnectionString);
            var syntax = database.GetService<ISyntaxProvider>();

            var sql = $"select {syntax!.String.PadRight(syntax.Delimit("CustomerID"), 7, "'-'")} from customers where {syntax.Delimit("CustomerID")}='ALFKI'";

            var ret = await database.ExecuteScalarAsync<string>(sql);

            Assert.AreEqual("ALFKI--", ret);
        }

        /// <summary>
        /// 测试 String.PadRight
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestStringPadRightLessAsync()
        {
            var factory = ServiceProvider.GetRequiredService<IDatabaseFactory>();

            await using var database = factory.CreateDatabase<T>(ConnectionString);
            var syntax = database.GetService<ISyntaxProvider>();

            var sql = $"select {syntax!.String.PadRight(syntax.Delimit("CustomerID"), 2, "'-'")} from customers where {syntax.Delimit("CustomerID")}='ALFKI'";

            var ret = await database.ExecuteScalarAsync<string>(sql);

            Assert.AreEqual("AL", ret);
        }

        /// <summary>
        /// 测试 String.ToUpper
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestStringToUpperAsync()
        {
            var factory = ServiceProvider.GetRequiredService<IDatabaseFactory>();

            await using var database = factory.CreateDatabase<T>(ConnectionString);
            var syntax = database.GetService<ISyntaxProvider>();

            var sql = $"select {syntax!.String.ToUpper(syntax.Delimit("City"))} from customers where {syntax.Delimit("CustomerID")}='ALFKI'";

            var ret = await database.ExecuteScalarAsync<string>(sql);

            Assert.AreEqual("BERLIN", ret);
        }

        /// <summary>
        /// 测试 String.ToLower
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestStringToLowerAsync()
        {
            var factory = ServiceProvider.GetRequiredService<IDatabaseFactory>();

            await using var database = factory.CreateDatabase<T>(ConnectionString);
            var syntax = database.GetService<ISyntaxProvider>();

            var sql = $"select {syntax!.String.ToLower(syntax.Delimit("City"))} from customers where {syntax.Delimit("CustomerID")}='ALFKI'";

            var ret = await database.ExecuteScalarAsync<string>(sql);

            Assert.AreEqual("berlin", ret);
        }

        /// <summary>
        /// 测试 String.IndexOf
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestStringIndexOfAsync()
        {
            var factory = ServiceProvider.GetRequiredService<IDatabaseFactory>();

            await using var database = factory.CreateDatabase<T>(ConnectionString);
            var syntax = database.GetService<ISyntaxProvider>();

            var sql = $"select {syntax!.String.IndexOf(syntax.Delimit("ContactName"), "'i'")} from customers where {syntax.Delimit("CustomerID")}='CACTU'";

            var ret = await database.ExecuteScalarAsync<int>(sql);

            Assert.AreEqual(5, ret);
        }

        /// <summary>
        /// 测试 String.IndexOf
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestStringIndexOfWithStartAsync()
        {
            var factory = ServiceProvider.GetRequiredService<IDatabaseFactory>();

            await using var database = factory.CreateDatabase<T>(ConnectionString);
            var syntax = database.GetService<ISyntaxProvider>();

            var sql = $"select {syntax!.String.IndexOf(syntax.Delimit("ContactName"), "'i'", 6)} from customers where {syntax.Delimit("CustomerID")}='CACTU'";

            var ret = await database.ExecuteScalarAsync<int>(sql);

            Assert.AreEqual(7, ret);
        }

        /// <summary>
        /// 测试 String.Reverse
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestStringReverseAsync()
        {
            var factory = ServiceProvider.GetRequiredService<IDatabaseFactory>();

            await using var database = factory.CreateDatabase<T>(ConnectionString);
            var syntax = database.GetService<ISyntaxProvider>();

            var sql = $"select {syntax!.String.Reverse(syntax.Delimit("CustomerID"))} from customers where {syntax.Delimit("CustomerID")}='ALFKI'";

            var ret = await database.ExecuteScalarAsync<string>(sql);

            Assert.AreEqual("IKFLA", ret);
        }

        /// <summary>
        /// 测试 String.Replace
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestStringReplaceAsync()
        {
            var factory = ServiceProvider.GetRequiredService<IDatabaseFactory>();

            await using var database = factory.CreateDatabase<T>(ConnectionString);
            var syntax = database.GetService<ISyntaxProvider>();

            var sql = $"select {syntax!.String.Replace(syntax.Delimit("CustomerID"), "'I'", "'a'")} from customers where {syntax.Delimit("CustomerID")}='ALFKI'";

            var ret = await database.ExecuteScalarAsync<string>(sql);

            Assert.AreEqual("ALFKa", ret);
        }

        /// <summary>
        /// 测试 String.TrimStart
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestStringTrimStartAsync()
        {
            var factory = ServiceProvider.GetRequiredService<IDatabaseFactory>();

            await using var database = factory.CreateDatabase<T>(ConnectionString);
            var syntax = database.GetService<ISyntaxProvider>();

            var sql = $"select {syntax!.String.TrimStart(syntax.String.Concat("'    '", syntax.Delimit("CustomerID")))} from customers where {syntax.Delimit("CustomerID")}='ALFKI'";

            var ret = await database.ExecuteScalarAsync<string>(sql);

            Assert.AreEqual("ALFKI", ret);
        }

        /// <summary>
        /// 测试 String.TrimEnd
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestStringTrimEndAsync()
        {
            var factory = ServiceProvider.GetRequiredService<IDatabaseFactory>();

            await using var database = factory.CreateDatabase<T>(ConnectionString);
            var syntax = database.GetService<ISyntaxProvider>();

            var sql = $"select {syntax!.String.TrimEnd(syntax.String.Concat(syntax.Delimit("CustomerID"), "'    '"))} from customers where {syntax.Delimit("CustomerID")}='ALFKI'";

            var ret = await database.ExecuteScalarAsync<string>(sql);

            Assert.AreEqual("ALFKI", ret);
        }

        /// <summary>
        /// 测试 String.Trim
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestStringTrimAsync()
        {
            var factory = ServiceProvider.GetRequiredService<IDatabaseFactory>();

            await using var database = factory.CreateDatabase<T>(ConnectionString);
            var syntax = database.GetService<ISyntaxProvider>();

            var sql = $"select {syntax!.String.Trim(syntax.String.Concat("'    '", syntax.Delimit("CustomerID"), "'    '"))} from customers where {syntax.Delimit("CustomerID")}='ALFKI'";

            var ret = await database.ExecuteScalarAsync<string>(sql);

            Assert.AreEqual("ALFKI", ret);
        }

        /// <summary>
        /// 测试 String.GroupConcat
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestStringGroupConcatAsync()
        {
            var factory = ServiceProvider.GetRequiredService<IDatabaseFactory>();

            await using var database = factory.CreateDatabase<T>(ConnectionString);
            var syntax = database.GetService<ISyntaxProvider>();

            var sql = $"select {syntax!.String.GroupConcat(syntax.Delimit("ProductName"), "','")} from products where {syntax.Delimit("CategoryID")}=6 group by {syntax.Delimit("CategoryID")}";

            var ret = await database.ExecuteScalarAsync<string>(sql);

            Assert.AreEqual(6, ret?.Split(',').Length);
        }

        /// <summary>
        /// 测试 String.GroupConcat
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestStringGroupConcatOrderbyAsync()
        {
            var factory = ServiceProvider.GetRequiredService<IDatabaseFactory>();

            await using var database = factory.CreateDatabase<T>(ConnectionString);
            var syntax = database.GetService<ISyntaxProvider>();

            var sql = $"select {syntax!.String.GroupConcat(syntax.Delimit("ProductName"), "','", syntax.Delimit("ProductName") + " ASC")} from products where {syntax.Delimit("CategoryID")}=6 group by {syntax.Delimit("CategoryID")}";

            var ret = await database.ExecuteScalarAsync<string>(sql);

            Assert.AreEqual("Alice Mutton,Mishi Kobe Niku,Perth Pasties,Pt chinois,Thringer Rostbratwurst,Tourtire", ret);
        }

        /// <summary>
        /// 测试 String.IsMatch
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestStringIsMatchAsync()
        {
            var factory = ServiceProvider.GetRequiredService<IDatabaseFactory>();

            await using var database = factory.CreateDatabase<T>(ConnectionString);
            var syntax = database.GetService<ISyntaxProvider>();

            var sql = $"select {syntax!.String.IsMatch(syntax.Delimit("CustomerID"), "'\\w*'")} from customers where {syntax.Delimit("CustomerID")}='ALFKI'";

            var ret = await database.ExecuteScalarAsync<bool>(sql);

            Assert.AreEqual(true, ret);
        }

        /// <summary>
        /// 测试 Math.Abs
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestMathAbsAsync()
        {
            var factory = ServiceProvider.GetRequiredService<IDatabaseFactory>();

            await using var database = factory.CreateDatabase<T>(ConnectionString);
            var syntax = database.GetService<ISyntaxProvider>();

            var sql = $"select {syntax!.Math.Abs("0 - " + syntax.Delimit("Freight"))} from orders where {syntax.Delimit("OrderID")}=10249";

            var ret = await database.ExecuteScalarAsync<decimal>(sql);

            Assert.IsTrue(ret > 0);
        }

        /// <summary>
        /// 测试 Math.Negate
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestMathNegateAsync()
        {
            var factory = ServiceProvider.GetRequiredService<IDatabaseFactory>();

            await using var database = factory.CreateDatabase<T>(ConnectionString);
            var syntax = database.GetService<ISyntaxProvider>();

            var sql = $"select {syntax!.Math.Negate(syntax.Delimit("Freight"))} from orders where {syntax.Delimit("OrderID")}=10252";

            var ret = await database.ExecuteScalarAsync<decimal>(sql);

            Assert.IsTrue(ret < 0);
        }

        /// <summary>
        /// 测试 Math.Truncate
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestMathTruncateAsync()
        {
            var factory = ServiceProvider.GetRequiredService<IDatabaseFactory>();

            await using var database = factory.CreateDatabase<T>(ConnectionString);
            var syntax = database.GetService<ISyntaxProvider>();

            var sql = $"select {syntax!.Math.Truncate(syntax.Delimit("Freight"))} from orders where {syntax.Delimit("OrderID")}=10252";

            var ret = await database.ExecuteScalarAsync<decimal>(sql);

            Assert.AreEqual(51, ret);
        }

        /// <summary>
        /// 测试 Math.Ceiling
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestMathCeilingAsync()
        {
            var factory = ServiceProvider.GetRequiredService<IDatabaseFactory>();

            await using var database = factory.CreateDatabase<T>(ConnectionString);
            var syntax = database.GetService<ISyntaxProvider>();

            var sql = $"select {syntax!.Math.Ceiling(syntax.Delimit("Freight"))} from orders where {syntax.Delimit("OrderID")}=10252";

            var ret = await database.ExecuteScalarAsync<decimal>(sql);

            Assert.AreEqual(52, ret);
        }

        /// <summary>
        /// 测试 Math.Floor
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestMathFloorAsync()
        {
            var factory = ServiceProvider.GetRequiredService<IDatabaseFactory>();

            await using var database = factory.CreateDatabase<T>(ConnectionString);
            var syntax = database.GetService<ISyntaxProvider>();

            var sql = $"select {syntax!.Math.Floor(syntax.Delimit("Freight"))} from orders where {syntax.Delimit("OrderID")}=10252";

            var ret = await database.ExecuteScalarAsync<decimal>(sql);

            Assert.AreEqual(51, ret);
        }

        /// <summary>
        /// 测试 Math.Round
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestMathRoundAsync()
        {
            var factory = ServiceProvider.GetRequiredService<IDatabaseFactory>();

            await using var database = factory.CreateDatabase<T>(ConnectionString);
            var syntax = database.GetService<ISyntaxProvider>();

            var sql = $"select {syntax!.Math.Round(syntax.Delimit("Freight"), 1)} from orders where {syntax.Delimit("OrderID")}=10252";

            var ret = await database.ExecuteScalarAsync<decimal>(sql);

            Assert.AreEqual(51.3m, ret);
        }

        /// <summary>
        /// 测试 Math.Modulo
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestMathModuloAsync()
        {
            var factory = ServiceProvider.GetRequiredService<IDatabaseFactory>();

            await using var database = factory.CreateDatabase<T>(ConnectionString);
            var syntax = database.GetService<ISyntaxProvider>();

            var sql = $"select {syntax!.Math.Modulo(syntax.Delimit("Freight"), 2)} from orders where {syntax.Delimit("OrderID")}=10252";

            var ret = await database.ExecuteScalarAsync<int>(sql);

            Assert.AreEqual(1, ret);
        }

        /// <summary>
        /// 测试 Math.BitAnd
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestMathBitAndAsync()
        {
            var factory = ServiceProvider.GetRequiredService<IDatabaseFactory>();

            await using var database = factory.CreateDatabase<T>(ConnectionString);
            var syntax = database.GetService<ISyntaxProvider>();

            var sql = $"select {syntax!.Math.BitAnd(syntax.Delimit("UnitsInStock"), 2)} from products where {syntax.Delimit("ProductID")}=1";

            var ret = await database.ExecuteScalarAsync<int>(sql);

            Assert.AreEqual(2, ret);
        }

        /// <summary>
        /// 测试 Math.BitOr
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestMathBitOrAsync()
        {
            var factory = ServiceProvider.GetRequiredService<IDatabaseFactory>();

            await using var database = factory.CreateDatabase<T>(ConnectionString);
            var syntax = database.GetService<ISyntaxProvider>();

            var sql = $"select {syntax!.Math.BitOr(syntax.Delimit("UnitsInStock"), 2)} from products where {syntax.Delimit("ProductID")}=1";

            var ret = await database.ExecuteScalarAsync<int>(sql);

            Assert.AreEqual(39, ret);
        }

        /// <summary>
        /// 测试 Math.BitNot
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestMathBitNotAsync()
        {
            var factory = ServiceProvider.GetRequiredService<IDatabaseFactory>();

            await using var database = factory.CreateDatabase<T>(ConnectionString);
            var syntax = database.GetService<ISyntaxProvider>();

            var sql = $"select {syntax!.Math.BitNot(syntax.Delimit("SupplierID"))} from products where {syntax.Delimit("ProductID")}=1";

            var ret = await database.ExecuteScalarAsync<int>(sql);

            Assert.AreEqual(-2, ret);
        }

        /// <summary>
        /// 测试 Math.BitNot
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestMathExclusiveOrAsync()
        {
            var factory = ServiceProvider.GetRequiredService<IDatabaseFactory>();

            await using var database = factory.CreateDatabase<T>(ConnectionString);
            var syntax = database.GetService<ISyntaxProvider>();

            var sql = $"select {syntax!.Math.ExclusiveOr(syntax.Delimit("UnitsInStock"), 20)} from products where {syntax.Delimit("ProductID")}=1";

            var ret = await database.ExecuteScalarAsync<int>(sql);

            Assert.AreEqual(51, ret);
        }

        /// <summary>
        /// 测试 Math.LeftShift
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestMathLeftShiftAsync()
        {
            var factory = ServiceProvider.GetRequiredService<IDatabaseFactory>();

            await using var database = factory.CreateDatabase<T>(ConnectionString);
            var syntax = database.GetService<ISyntaxProvider>();

            var sql = $"select {syntax!.Math.LeftShift(syntax.Delimit("UnitsInStock"), 2)} from products where {syntax.Delimit("ProductID")}=1";

            var ret = await database.ExecuteScalarAsync<int>(sql);

            Assert.AreEqual(156, ret);
        }

        /// <summary>
        /// 测试 Math.RightShift
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestMathRightShiftAsync()
        {
            var factory = ServiceProvider.GetRequiredService<IDatabaseFactory>();

            await using var database = factory.CreateDatabase<T>(ConnectionString);
            var syntax = database.GetService<ISyntaxProvider>();

            var sql = $"select {syntax!.Math.RightShift(syntax.Delimit("UnitsInStock"), 2)} from products where {syntax.Delimit("ProductID")}=1";

            var ret = await database.ExecuteScalarAsync<int>(sql);

            Assert.AreEqual(9, ret);
        }

        /// <summary>
        /// 测试 Math.Random
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestMathRandomAsync()
        {
            var factory = ServiceProvider.GetRequiredService<IDatabaseFactory>();

            await using var database = factory.CreateDatabase<T>(ConnectionString);
            var syntax = database.GetService<ISyntaxProvider>();

            var sql = $"select {syntax!.Math.Random()}{syntax.FakeTable}";

            var ret = await database.ExecuteScalarAsync<decimal>(sql);

            Assert.AreNotEqual(0m, ret);
        }

        /// <summary>
        /// 测试 Math.Pow
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestMathPowerAsync()
        {
            var factory = ServiceProvider.GetRequiredService<IDatabaseFactory>();

            await using var database = factory.CreateDatabase<T>(ConnectionString);
            var syntax = database.GetService<ISyntaxProvider>();

            var sql = $"select {syntax!.Math.Power(syntax.Delimit("UnitsInStock"), 2)} from products where {syntax.Delimit("ProductID")}=1";

            var ret = await database.ExecuteScalarAsync<decimal>(sql);

            Assert.AreEqual(1521, ret);
        }

        /// <summary>
        /// 测试 Math.Sqrt
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestMathSqrtAsync()
        {
            var factory = ServiceProvider.GetRequiredService<IDatabaseFactory>();

            await using var database = factory.CreateDatabase<T>(ConnectionString);
            var syntax = database.GetService<ISyntaxProvider>();

            var sql = $"select {syntax!.Math.Sqrt(syntax.Delimit("UnitsInStock"))} from products where {syntax.Delimit("ProductID")}=1";

            var ret = await database.ExecuteScalarAsync<decimal>(sql);

            Assert.AreEqual(6.245m, Math.Round(ret, 3));
        }

        /// <summary>
        /// 测试 DateTime.Now
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestDateTimeNowAsync()
        {
            var factory = ServiceProvider.GetRequiredService<IDatabaseFactory>();

            await using var database = factory.CreateDatabase<T>(ConnectionString);
            var syntax = database.GetService<ISyntaxProvider>();

            var sql = $"select {syntax!.DateTime.Now()}{syntax.FakeTable}";

            var ret = await database.ExecuteScalarAsync<DateTime>(sql);

            Assert.AreEqual(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), ret.ToString("yyyy-MM-dd HH:mm:ss"));
        }

        /// <summary>
        /// 测试 DateTime.UtcNow
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestDateTimeUtcNowAsync()
        {
            var factory = ServiceProvider.GetRequiredService<IDatabaseFactory>();

            await using var database = factory.CreateDatabase<T>(ConnectionString);
            var syntax = database.GetService<ISyntaxProvider>();

            var sql = $"select {syntax!.DateTime.UtcNow()}{syntax.FakeTable}";

            var ret = await database.ExecuteScalarAsync<DateTime>(sql);

            Assert.AreEqual(DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"), ret.ToString("yyyy-MM-dd HH:mm:ss"));
        }

        /// <summary>
        /// 测试 DateTime.New
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestDateTimeNewAsync()
        {
            var factory = ServiceProvider.GetRequiredService<IDatabaseFactory>();

            await using var database = factory.CreateDatabase<T>(ConnectionString);
            var syntax = database.GetService<ISyntaxProvider>();

            var sql = $"select {syntax!.DateTime.New(2023, 1, 1)}{syntax.FakeTable}";

            var ret = await database.ExecuteScalarAsync<DateTime>(sql);

            Assert.AreEqual("2023-01-01", ret.ToString("yyyy-MM-dd"));
        }

        /// <summary>
        /// 测试 DateTime.New
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestDateTimeNewFullAsync()
        {
            var factory = ServiceProvider.GetRequiredService<IDatabaseFactory>();

            await using var database = factory.CreateDatabase<T>(ConnectionString);
            var syntax = database.GetService<ISyntaxProvider>();

            var sql = $"select {syntax!.DateTime.New(2023, 1, 1, 12, 20, 33)}{syntax.FakeTable}";

            var ret = await database.ExecuteScalarAsync<DateTime>(sql);

            Assert.AreEqual("2023-01-01 12:20:33", ret.ToString("yyyy-MM-dd HH:mm:ss"));
        }

        /// <summary>
        /// 测试 DateTime.Year
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestDateTimeYearAsync()
        {
            var factory = ServiceProvider.GetRequiredService<IDatabaseFactory>();

            await using var database = factory.CreateDatabase<T>(ConnectionString);
            var syntax = database.GetService<ISyntaxProvider>();

            var sql = $"select {syntax!.DateTime.Year(syntax.Delimit("OrderDate"))} from orders where {syntax.Delimit("OrderID")}=10248";

            var ret = await database.ExecuteScalarAsync<int>(sql);

            Assert.AreEqual(1996, ret);
        }

        /// <summary>
        /// 测试 DateTime.Month
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestDateTimeMonthAsync()
        {
            var factory = ServiceProvider.GetRequiredService<IDatabaseFactory>();

            await using var database = factory.CreateDatabase<T>(ConnectionString);
            var syntax = database.GetService<ISyntaxProvider>();

            var sql = $"select {syntax!.DateTime.Month(syntax.Delimit("OrderDate"))} from orders where {syntax.Delimit("OrderID")}=10248";

            var ret = await database.ExecuteScalarAsync<int>(sql);

            Assert.AreEqual(7, ret);
        }

        /// <summary>
        /// 测试 DateTime.Day
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestDateTimeDayAsync()
        {
            var factory = ServiceProvider.GetRequiredService<IDatabaseFactory>();

            await using var database = factory.CreateDatabase<T>(ConnectionString);
            var syntax = database.GetService<ISyntaxProvider>();

            var sql = $"select {syntax!.DateTime.Day(syntax.Delimit("OrderDate"))} from orders where {syntax.Delimit("OrderID")}=10248";

            var ret = await database.ExecuteScalarAsync<int>(sql);

            Assert.AreEqual(4, ret);
        }

        /// <summary>
        /// 测试 DateTime.DayOfWeek
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestDateTimeDayOfWeekAsync()
        {
            var factory = ServiceProvider.GetRequiredService<IDatabaseFactory>();

            await using var database = factory.CreateDatabase<T>(ConnectionString);
            var syntax = database.GetService<ISyntaxProvider>();

            var sql = $"select {syntax!.DateTime.DayOfWeek(syntax.Delimit("OrderDate"))} from orders where {syntax.Delimit("OrderID")}=10248";

            var ret = await database.ExecuteScalarAsync<int>(sql);

            Assert.AreEqual(4, ret);
        }

        /// <summary>
        /// 测试 DateTime.DayOfYear
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestDateTimeDayOfYearAsync()
        {
            var factory = ServiceProvider.GetRequiredService<IDatabaseFactory>();

            await using var database = factory.CreateDatabase<T>(ConnectionString);
            var syntax = database.GetService<ISyntaxProvider>();

            var sql = $"select {syntax!.DateTime.DayOfYear(syntax.Delimit("OrderDate"))} from orders where {syntax.Delimit("OrderID")}=10248";

            var ret = await database.ExecuteScalarAsync<int>(sql);

            Assert.AreEqual(186, ret);
        }

        /// <summary>
        /// 测试 DateTime.WeekOfYear
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestDateTimeWeekOfYearAsync()
        {
            var factory = ServiceProvider.GetRequiredService<IDatabaseFactory>();

            await using var database = factory.CreateDatabase<T>(ConnectionString);
            var syntax = database.GetService<ISyntaxProvider>();

            var sql = $"select {syntax!.DateTime.WeekOfYear(syntax.Delimit("OrderDate"))} from orders where {syntax.Delimit("OrderID")}=10248";

            var ret = await database.ExecuteScalarAsync<int>(sql);

            Assert.AreEqual(27, ret);
        }

        /// <summary>
        /// 测试 DateTime.Hour
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestDateTimeHourAsync()
        {
            var factory = ServiceProvider.GetRequiredService<IDatabaseFactory>();

            await using var database = factory.CreateDatabase<T>(ConnectionString);
            var syntax = database.GetService<ISyntaxProvider>();

            var sql = $"select {syntax!.DateTime.Hour(syntax.DateTime.Now())}{syntax.FakeTable}";

            var ret = await database.ExecuteScalarAsync<int>(sql);

            Assert.AreEqual(DateTime.Now.Hour, ret);
        }

        /// <summary>
        /// 测试 DateTime.Minute
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestDateTimeMinuteAsync()
        {
            var factory = ServiceProvider.GetRequiredService<IDatabaseFactory>();

            await using var database = factory.CreateDatabase<T>(ConnectionString);
            var syntax = database.GetService<ISyntaxProvider>();

            var sql = $"select {syntax!.DateTime.Minute(syntax.DateTime.Now())}{syntax.FakeTable}";

            var ret = await database.ExecuteScalarAsync<int>(sql);

            Assert.AreEqual(DateTime.Now.Minute, ret);
        }

        /// <summary>
        /// 测试 DateTime.Second
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestDateTimeSecondAsync()
        {
            var factory = ServiceProvider.GetRequiredService<IDatabaseFactory>();

            await using var database = factory.CreateDatabase<T>(ConnectionString);
            var syntax = database.GetService<ISyntaxProvider>();

            var sql = $"select {syntax!.DateTime.Second("'2023-01-01 12:22:45'")}{syntax.FakeTable}";

            var ret = await database.ExecuteScalarAsync<int>(sql);

            Assert.AreEqual(45, ret);
        }

        /// <summary>
        /// 测试 DateTime.Millisecond
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestDateTimeMillisecondAsync()
        {
            var factory = ServiceProvider.GetRequiredService<IDatabaseFactory>();

            await using var database = factory.CreateDatabase<T>(ConnectionString);
            var syntax = database.GetService<ISyntaxProvider>();

            var sql = $"select {syntax!.DateTime.Millisecond(syntax.DateTime.Now())}{syntax.FakeTable}";

            var ret = await database.ExecuteScalarAsync<int>(sql);

            Assert.IsTrue(ret > 0);
        }

        /// <summary>
        /// 测试 DateTime.AddYears
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestDateTimeAddYearsAsync()
        {
            var factory = ServiceProvider.GetRequiredService<IDatabaseFactory>();

            await using var database = factory.CreateDatabase<T>(ConnectionString);
            var syntax = database.GetService<ISyntaxProvider>();

            var sql = $"select {syntax!.DateTime.AddYears(syntax.Delimit("OrderDate"), 10)} from orders where {syntax.Delimit("OrderID")}=10248";

            var ret = await database.ExecuteScalarAsync<DateTime>(sql);

            Assert.AreEqual("2006-07-04", ret.ToString("yyyy-MM-dd"));
        }

        /// <summary>
        /// 测试 DateTime.AddMonths
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestDateTimeAddMonthsAsync()
        {
            var factory = ServiceProvider.GetRequiredService<IDatabaseFactory>();

            await using var database = factory.CreateDatabase<T>(ConnectionString);
            var syntax = database.GetService<ISyntaxProvider>();

            var sql = $"select {syntax!.DateTime.AddMonths(syntax.Delimit("OrderDate"), 10)} from orders where {syntax.Delimit("OrderID")}=10248";

            var ret = await database.ExecuteScalarAsync<DateTime>(sql);

            Assert.AreEqual("1997-05-04", ret.ToString("yyyy-MM-dd"));
        }

        /// <summary>
        /// 测试 DateTime.AddDays
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestDateTimeAddDaysAsync()
        {
            var factory = ServiceProvider.GetRequiredService<IDatabaseFactory>();

            await using var database = factory.CreateDatabase<T>(ConnectionString);
            var syntax = database.GetService<ISyntaxProvider>();

            var sql = $"select {syntax!.DateTime.AddDays(syntax.Delimit("OrderDate"), 10)} from orders where {syntax.Delimit("OrderID")}=10248";

            var ret = await database.ExecuteScalarAsync<DateTime>(sql);

            Assert.AreEqual("1996-07-14", ret.ToString("yyyy-MM-dd"));
        }

        /// <summary>
        /// 测试 DateTime.AddHours
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestDateTimeAddHoursAsync()
        {
            var factory = ServiceProvider.GetRequiredService<IDatabaseFactory>();

            await using var database = factory.CreateDatabase<T>(ConnectionString);
            var syntax = database.GetService<ISyntaxProvider>();

            var sql = $"select {syntax!.DateTime.AddHours(syntax.Delimit("OrderDate"), 10)} from orders where {syntax.Delimit("OrderID")}=10248";

            var ret = await database.ExecuteScalarAsync<DateTime>(sql);

            Assert.AreEqual("1996-07-04 10:00:00", ret.ToString("yyyy-MM-dd HH:mm:ss"));
        }

        /// <summary>
        /// 测试 DateTime.AddMinutes
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestDateTimeAddMinutesAsync()
        {
            var factory = ServiceProvider.GetRequiredService<IDatabaseFactory>();

            await using var database = factory.CreateDatabase<T>(ConnectionString);
            var syntax = database.GetService<ISyntaxProvider>();

            var sql = $"select {syntax!.DateTime.AddMinutes(syntax.Delimit("OrderDate"), 10)} from orders where {syntax.Delimit("OrderID")}=10248";

            var ret = await database.ExecuteScalarAsync<DateTime>(sql);

            Assert.AreEqual("1996-07-04 00:10:00", ret.ToString("yyyy-MM-dd HH:mm:ss"));
        }

        /// <summary>
        /// 测试 DateTime.AddSeconds
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestDateTimeAddSecondsAsync()
        {
            var factory = ServiceProvider.GetRequiredService<IDatabaseFactory>();

            await using var database = factory.CreateDatabase<T>(ConnectionString);
            var syntax = database.GetService<ISyntaxProvider>();

            var sql = $"select {syntax!.DateTime.AddSeconds(syntax.Delimit("OrderDate"), 10)} from orders where {syntax.Delimit("OrderID")}=10248";

            var ret = await database.ExecuteScalarAsync<DateTime>(sql);

            Assert.AreEqual("1996-07-04 00:00:10", ret.ToString("yyyy-MM-dd HH:mm:ss"));
        }

        /// <summary>
        /// 测试 DateTime.DiffDays
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestDateTimeDiffDaysAsync()
        {
            var factory = ServiceProvider.GetRequiredService<IDatabaseFactory>();

            await using var database = factory.CreateDatabase<T>(ConnectionString);
            var syntax = database.GetService<ISyntaxProvider>();

            var sql = $"select {syntax!.DateTime.DiffDays(syntax.Delimit("OrderDate"), syntax.Convert(syntax.Delimit("RequiredDate"), DbType.DateTime))} from orders where {syntax.Delimit("OrderID")}=10248";

            var ret = await database.ExecuteScalarAsync<int>(sql);

            Assert.AreEqual(28, ret);
        }

        /// <summary>
        /// 测试 DateTime.DiffHours
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestDateTimeDiffHoursAsync()
        {
            var factory = ServiceProvider.GetRequiredService<IDatabaseFactory>();

            await using var database = factory.CreateDatabase<T>(ConnectionString);
            var syntax = database.GetService<ISyntaxProvider>();

            var sql = $"select {syntax!.DateTime.DiffHours(syntax.Delimit("OrderDate"), syntax.Convert(syntax.Delimit("RequiredDate"), DbType.DateTime))} from orders where {syntax.Delimit("OrderID")}=10248";

            var ret = await database.ExecuteScalarAsync<int>(sql);

            Assert.AreEqual(24 * 28, ret);
        }

        /// <summary>
        /// 测试 DateTime.DiffMinutes
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestDateTimeDiffMinutesAsync()
        {
            var factory = ServiceProvider.GetRequiredService<IDatabaseFactory>();

            await using var database = factory.CreateDatabase<T>(ConnectionString);
            var syntax = database.GetService<ISyntaxProvider>();

            var sql = $"select {syntax!.DateTime.DiffMinutes("'2023-01-01 14:20:44'", "'2023-01-01 14:21:44'")}{syntax.FakeTable}";

            var ret = await database.ExecuteScalarAsync<int>(sql);

            Assert.AreEqual(1, ret);
        }

        /// <summary>
        /// 测试 DateTime.DiffSeconds
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestDateTimeDiffSecondsAsync()
        {
            var factory = ServiceProvider.GetRequiredService<IDatabaseFactory>();

            await using var database = factory.CreateDatabase<T>(ConnectionString);
            var syntax = database.GetService<ISyntaxProvider>();

            var sql = $"select {syntax!.DateTime.DiffSeconds("'2023-01-01 14:20:44'", "'2023-01-01 14:21:44'")}{syntax.FakeTable}";

            var ret = await database.ExecuteScalarAsync<int>(sql);

            Assert.AreEqual(60, ret);
        }
    }
}
