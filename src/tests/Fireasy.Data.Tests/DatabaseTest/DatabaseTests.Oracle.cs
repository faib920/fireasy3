namespace Fireasy.Data.Tests.DatabaseTest
{
    [TestClass]
    [TestCategory("DatabaseTests")]
    public class DatabaseTests_Oracle : DatabaseTests<OracleProvider>
    {
        protected override string ConnectionString => Constants.Oracle_ConnectionString;

        protected override string InstanceName => Constants.Oracle_InstanceName;

        protected override string ProviderName => "Oracle";
    }
}
