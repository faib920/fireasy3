namespace Fireasy.Data.Tests.SchemaTest
{
    [TestClass]
    [TestCategory("SchemaTests")]
    public class SchemaTests_Odbc_Dameng : SchemaTests<OdbcProvider>
    {
        protected override string ConnectionString => Constants.Odbc_Dameng_ConnectionString;

        protected override string InstanceName => Constants.Odbc_Dameng_InstanceName;
    }
}
