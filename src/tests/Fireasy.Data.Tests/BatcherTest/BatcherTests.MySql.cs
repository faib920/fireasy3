namespace Fireasy.Data.Tests.BatcherTest
{
    [TestClass]
    [TestCategory("Batcherests")]
    public class BatcherTests_MySql : BatcherTests<MySqlProvider>
    {
        protected override string ConnectionString => Constants.MySql_ConnectionString;

        protected override string InstanceName => Constants.MySql_InstanceName;
    }
}
