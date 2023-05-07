namespace Fireasy.Data.Tests.DatabaseTest
{
    [TestClass]
    [TestCategory("DatabaseTests")]
    public class DatabaseTests_Odbc_Dameng : DatabaseTests<OdbcProvider>
    {
        protected override string ConnectionString => Constants.Odbc_Dameng_ConnectionString;

        protected override string InstanceName => Constants.Odbc_Dameng_InstanceName;

        protected override string ProviderName => "Odbc";
    }
}
