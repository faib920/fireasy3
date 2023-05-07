namespace Fireasy.Data.Tests.DatabaseTest
{
    [TestClass]
    [TestCategory("DatabaseTests")]
    public class DatabaseTests_Odbc_Kingbase : DatabaseTests<OdbcProvider>
    {
        protected override string ConnectionString => Constants.Odbc_Kingbase_ConnectionString;

        protected override string InstanceName => Constants.Odbc_Kingbase_InstanceName;

        protected override string ProviderName => "Odbc";
    }
}
