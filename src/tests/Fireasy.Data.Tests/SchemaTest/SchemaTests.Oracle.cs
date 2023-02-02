namespace Fireasy.Data.Tests.SchemaTest
{
    [TestClass]
    [TestCategory("SchemaTests")]
    public class SchemaTests_Oracle : SchemaTests<OracleProvider>
    {
        protected override string ConnectionString => Constants.Oracle_ConnectionString;

        protected override string InstanceName => Constants.Oracle_InstanceName;
    }
}
