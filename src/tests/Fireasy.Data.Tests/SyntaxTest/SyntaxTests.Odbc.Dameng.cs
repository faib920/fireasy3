namespace Fireasy.Data.Tests.SyntaxTest
{
    [TestClass]
    [TestCategory("SyntaxTests")]
    public class SyntaxTests_Odbc_Dameng : SyntaxTests<OdbcProvider>
    {
        protected override string ConnectionString => Constants.Odbc_Dameng_ConnectionString;

        protected override string InstanceName => Constants.Odbc_Dameng_InstanceName;
    }
}
