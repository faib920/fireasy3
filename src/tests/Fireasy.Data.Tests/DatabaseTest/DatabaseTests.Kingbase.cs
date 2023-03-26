namespace Fireasy.Data.Tests.DatabaseTest
{
    [TestClass]
    [TestCategory("DatabaseTests")]
    public class DatabaseTests_Kingbase : DatabaseTests<KingbaseProvider>
    {
        protected override string ConnectionString => Constants.Kingbase_ConnectionString;

        protected override string InstanceName => Constants.Kingbase_InstanceName;

        protected override string ProviderName => "Kingbase";

        protected override void ConfigureBuilder(SetupBuilder builder)
        {
            base.ConfigureBuilder(builder);
        }
    }
}
