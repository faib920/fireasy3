namespace Fireasy.Data.Tests.BatcherTest
{
    [TestClass]
    [TestCategory("Batcherests")]
    public class BatcherTests_SqlServer : BatcherTests<SqlServerProvider>
    {
        protected override string ConnectionString => Constants.SqlServer_ConnectionString;

        protected override string InstanceName => Constants.SqlServer_InstanceName;
    }
}
