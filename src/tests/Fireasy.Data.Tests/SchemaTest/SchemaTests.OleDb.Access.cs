namespace Fireasy.Data.Tests.SchemaTest
{
    [TestClass]
    [TestCategory("SchemaTests")]
    public class SchemaTests_OleDb_Access : SchemaTests<OleDbProvider>
    {
        protected override string ConnectionString => Constants.OleDb_Access_ConnectionString;

        protected override string InstanceName => Constants.OleDb_Access_InstanceName;
    }
}
