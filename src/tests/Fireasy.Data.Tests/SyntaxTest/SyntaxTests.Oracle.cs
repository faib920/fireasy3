namespace Fireasy.Data.Tests.SyntaxTest
{
    [TestClass]
    [TestCategory("SyntaxTests")]
    public class SyntaxTests_Oracle : SyntaxTests<OracleProvider>
    {
        protected override string ConnectionString => Constants.Oracle_ConnectionString;

        protected override string InstanceName => Constants.Oracle_InstanceName;
    }
}
