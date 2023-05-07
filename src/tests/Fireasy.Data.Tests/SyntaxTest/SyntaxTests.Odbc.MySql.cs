namespace Fireasy.Data.Tests.SyntaxTest
{
    [TestClass]
    [TestCategory("SyntaxTests")]
    public class SyntaxTests_Odbc_MySql : SyntaxTests<OdbcProvider>
    {
        protected override string ConnectionString => Constants.Odbc_MySql_ConnectionString;

        protected override string InstanceName => Constants.Odbc_MySql_InstanceName;
    }
}
