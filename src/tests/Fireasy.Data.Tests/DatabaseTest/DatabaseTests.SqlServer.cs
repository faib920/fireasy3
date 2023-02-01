namespace Fireasy.Data.Tests.DatabaseTest
{
    [TestClass]
    [TestCategory("DatabaseTests")]
    public class DatabaseTests_SqlServer : DatabaseTests<SqlServerProvider>
    {
        protected override string ConnectionString => Constants.SqlServer_ConnectionString;

        protected override string InstanceName => Constants.SqlServer_InstanceName;
    }
}
