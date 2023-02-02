namespace Fireasy.Data.Tests.DatabaseTest
{
    [TestClass]
    [TestCategory("DatabaseTests")]
    public class DatabaseTests_SQLite : DatabaseTests<SQLiteProvider>
    {
        protected override string ConnectionString => Constants.SQLite_ConnectionString;

        protected override string InstanceName => Constants.SQLite_InstanceName;

        protected override string ProviderName => "SQLite";
    }
}
