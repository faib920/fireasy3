namespace Fireasy.Data.Tests.SchemaTest
{
    [TestClass]
    [TestCategory("SchemaTests")]
    public class SchemaTests_PostgreSql : SchemaTests<PostgreSqlProvider>
    {
        protected override string ConnectionString => Constants.PostgreSql_ConnectionString;

        protected override string InstanceName => Constants.PostgreSql_InstanceName;
    }
}
