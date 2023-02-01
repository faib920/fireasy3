namespace Fireasy.Data.Tests.SyntaxTest
{
    [TestClass]
    [TestCategory("SyntaxTests")]
    public class SyntaxTests_Firebird : SyntaxTests<FirebirdProvider>
    {
        protected override string ConnectionString => Constants.Firebird_ConnectionString;

        protected override string InstanceName => Constants.Firebird_InstanceName;
    }
}
