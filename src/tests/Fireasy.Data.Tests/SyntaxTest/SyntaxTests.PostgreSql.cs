namespace Fireasy.Data.Tests.SyntaxTest
{
    [TestClass]
    [TestCategory("SyntaxTests")]
    public class SyntaxTests_PostgreSql : SyntaxTests<PostgreSqlProvider>
    {
        protected override string ConnectionString => Constants.PostgreSql_ConnectionString;

        protected override string InstanceName => Constants.PostgreSql_InstanceName;
    }
}
