namespace Fireasy.Data.Tests.SyntaxTest
{
    [TestClass]
    [TestCategory("SyntaxTests")]
    public class SyntaxTests_SqlServer : SyntaxTests<SqlServerProvider>
    {
        protected override string ConnectionString => Constants.SqlServer_ConnectionString;

        protected override string InstanceName => Constants.SqlServer_InstanceName;
    }
}
