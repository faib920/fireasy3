namespace Fireasy.Data.Tests.BatcherTest
{
    [TestClass]
    [TestCategory("Batcherests")]
    public class BatcherTests_Dameng : BatcherTests<DamengProvider>
    {
        protected override string ConnectionString => Constants.Dameng_ConnectionString;

        protected override string InstanceName => Constants.Dameng_InstanceName;
    }
}
