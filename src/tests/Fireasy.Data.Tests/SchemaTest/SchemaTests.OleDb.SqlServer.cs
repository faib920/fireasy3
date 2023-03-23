namespace Fireasy.Data.Tests.SchemaTest
{
    [TestClass]
    [TestCategory("SchemaTests")]
    public class SchemaTests_OleDb_SqlServer : SchemaTests<OleDbProvider>
    {
        protected override string ConnectionString => Constants.OleDb_SqlServer_ConnectionString;

        protected override string InstanceName => Constants.OleDb_SqlServer_InstanceName;
    }
}
