namespace Fireasy.Data.Tests.SchemaTest
{
    [TestClass]
    [TestCategory("SchemaTests")]
    public class SchemaTests_Firebird : SchemaTests<FirebirdProvider>
    {
        protected override string ConnectionString => Constants.Firebird_ConnectionString;

        protected override string InstanceName => Constants.Firebird_InstanceName;
    }
}
