namespace Fireasy.Data.Tests.BatcherTest
{
    [TestClass]
    [TestCategory("Batcherests")]
    public class BatcherTests_Oracle : BatcherTests<OracleProvider>
    {
        protected override string ConnectionString => Constants.Oracle_ConnectionString;

        protected override string InstanceName => Constants.Oracle_InstanceName;
    }
}
