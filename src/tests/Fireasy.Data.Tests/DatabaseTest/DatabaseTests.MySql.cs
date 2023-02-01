namespace Fireasy.Data.Tests.DatabaseTest
{
    [TestClass]
    [TestCategory("DatabaseTests")]
    public class DatabaseTests_MySql : DatabaseTests<MySqlProvider>
    {
        protected override string ConnectionString => Constants.MySql_ConnectionString;

        protected override string InstanceName => Constants.MySql_InstanceName;
    }
}
