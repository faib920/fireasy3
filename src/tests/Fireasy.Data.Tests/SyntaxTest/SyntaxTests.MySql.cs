namespace Fireasy.Data.Tests.SyntaxTest
{
    [TestClass]
    [TestCategory("SyntaxTests")]
    public class SyntaxTests_MySql : SyntaxTests<MySqlProvider>
    {
        protected override string ConnectionString => Constants.MySql_ConnectionString;

        protected override string InstanceName => Constants.MySql_InstanceName;
    }
}
