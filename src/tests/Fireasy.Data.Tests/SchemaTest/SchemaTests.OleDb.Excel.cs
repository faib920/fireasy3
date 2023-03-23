namespace Fireasy.Data.Tests.SchemaTest
{
    [TestClass]
    [TestCategory("SchemaTests")]
    public class SchemaTests_OleDb_Excel : SchemaTests<OleDbProvider>
    {
        protected override string ConnectionString => Constants.OleDb_Excel_ConnectionString;

        protected override string InstanceName => Constants.OleDb_Excel_InstanceName;
    }
}
