namespace Fireasy.Data.Tests.DatabaseTest
{
    [TestClass]
    [TestCategory("DatabaseTests")]
    public class DatabaseTests_PostgreSql : DatabaseTests<PostgreSqlProvider>
    {
        protected override string ConnectionString => Constants.PostgreSql_ConnectionString;

        protected override string InstanceName => Constants.PostgreSql_InstanceName;

        protected override string ProviderName => "PostgreSql";

        protected override void ConfigureBuilder(SetupBuilder builder)
        {
            base.ConfigureBuilder(builder);
        }
    }
}
