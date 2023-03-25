namespace Fireasy.Data.Tests.SchemaTest
{
    [TestClass]
    [TestCategory("SchemaTests")]
    public class SchemaTests_Kingbase : SchemaTests<KingbaseProvider>
    {
        protected override string ConnectionString => Constants.Kingbase_ConnectionString;

        protected override string InstanceName => Constants.Kingbase_InstanceName;
    }
}
