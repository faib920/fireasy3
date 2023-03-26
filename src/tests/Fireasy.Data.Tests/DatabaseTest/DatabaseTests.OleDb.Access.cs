namespace Fireasy.Data.Tests.DatabaseTest
{
    [TestClass]
    [TestCategory("DatabaseTests")]
    public class DatabaseTests_OleDb_Access : DatabaseTests<OleDbProvider>
    {
        protected override string ConnectionString => Constants.OleDb_Access_ConnectionString;

        protected override string InstanceName => Constants.OleDb_Access_InstanceName;

        protected override string ProviderName => "OleDb";
    }
}
