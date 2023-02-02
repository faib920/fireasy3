namespace Fireasy.Data.Tests.SchemaTest
{
    [TestClass]
    [TestCategory("SchemaTests")]
    public class SchemaTests_MySql : SchemaTests<MySqlProvider>
    {
        protected override string ConnectionString => Constants.MySql_ConnectionString;

        protected override string InstanceName => Constants.MySql_InstanceName;
    }
}
