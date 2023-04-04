namespace Fireasy.Data.Tests.DatabaseTest
{
    [TestClass]
    [TestCategory("DatabaseTests")]
    public class DatabaseTests_ShenTong : DatabaseTests<ShenTongProvider>
    {
        protected override string ConnectionString => Constants.ShenTong_ConnectionString;

        protected override string InstanceName => Constants.ShenTong_InstanceName;

        protected override string ProviderName => "ShenTong";
    }
}
