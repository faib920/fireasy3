namespace Fireasy.Data.Tests.DatabaseTest
{
    [TestClass]
    [TestCategory("DatabaseTests")]
    public class DatabaseTests_Dameng : DatabaseTests<DamengProvider>
    {
        protected override string ConnectionString => Constants.Dameng_ConnectionString;

        protected override string InstanceName => Constants.Dameng_InstanceName;
    }
}
