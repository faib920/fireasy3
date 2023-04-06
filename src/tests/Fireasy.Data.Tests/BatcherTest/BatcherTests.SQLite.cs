namespace Fireasy.Data.Tests.BatcherTest
{
    [TestClass]
    [TestCategory("Batcherests")]
    public class BatcherTests_SQLite : BatcherTests<SQLiteProvider>
    {
        protected override string ConnectionString => Constants.SQLite_ConnectionString;

        protected override string InstanceName => Constants.SQLite_InstanceName;
    }
}
