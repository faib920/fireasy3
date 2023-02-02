namespace Fireasy.Data.Tests.SchemaTest
{
    [TestClass]
    [TestCategory("SchemaTests")]
    public class SchemaTests_SqlServer : SchemaTests<SqlServerProvider>
    {
        protected override string ConnectionString => Constants.SqlServer_ConnectionString;

        protected override string InstanceName => Constants.SqlServer_InstanceName;
    }
}
