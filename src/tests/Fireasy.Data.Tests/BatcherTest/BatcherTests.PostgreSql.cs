namespace Fireasy.Data.Tests.BatcherTest
{
    [TestClass]
    [TestCategory("Batcherests")]
    public class BatcherTests_PostgreSql : BatcherTests<PostgreSqlProvider>
    {
        protected override string ConnectionString => Constants.PostgreSql_ConnectionString;

        protected override string InstanceName => Constants.PostgreSql_InstanceName;
    }
}
