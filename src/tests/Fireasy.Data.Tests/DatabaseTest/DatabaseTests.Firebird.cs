namespace Fireasy.Data.Tests.DatabaseTest
{
    [TestClass]
    [TestCategory("DatabaseTests")]
    public class DatabaseTests_Firebird : DatabaseTests<FirebirdProvider>
    {
        protected override string ConnectionString => Constants.Firebird_ConnectionString;

        protected override string InstanceName => Constants.Firebird_InstanceName;

        protected override string ProviderName => "Firebird";
    }
}
