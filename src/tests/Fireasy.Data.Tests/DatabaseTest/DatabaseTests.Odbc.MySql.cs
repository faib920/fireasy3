namespace Fireasy.Data.Tests.DatabaseTest
{
    [TestClass]
    [TestCategory("DatabaseTests")]
    public class DatabaseTests_Odbc_MySql : DatabaseTests<OdbcProvider>
    {
        protected override string ConnectionString => Constants.Odbc_MySql_ConnectionString;

        protected override string InstanceName => Constants.Odbc_MySql_InstanceName;

        protected override string ProviderName => "Odbc";
    }
}
