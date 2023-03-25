namespace Fireasy.Data.Tests.SyntaxTest
{
    [TestClass]
    [TestCategory("SyntaxTests")]
    public class SyntaxTests_Kingbase : SyntaxTests<KingbaseProvider>
    {
        protected override string ConnectionString => Constants.Kingbase_ConnectionString;

        protected override string InstanceName => Constants.Kingbase_InstanceName;
    }
}
