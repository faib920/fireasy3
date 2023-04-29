namespace Fireasy.Data.Tests.SchemaTest
{
    [TestClass]
    [TestCategory("SchemaTests")]
    public class SchemaTests_Odbc_MySql : SchemaTests<OdbcProvider>
    {
        protected override string ConnectionString => Constants.Odbc_Mysql_ConnectionString;

        protected override string InstanceName => Constants.Odbc_MySql_InstanceName;
    }
}
