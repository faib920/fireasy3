namespace Fireasy.Data.Tests.SyntaxTest
{
    [TestClass]
    [TestCategory("SyntaxTests")]
    public class SyntaxTests_OleDb_Access : SyntaxTests<OleDbProvider>
    {
        protected override string ConnectionString => Constants.OleDb_Access_ConnectionString;

        protected override string InstanceName => Constants.OleDb_Access_InstanceName;
    }
}
