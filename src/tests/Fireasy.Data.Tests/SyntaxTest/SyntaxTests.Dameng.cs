namespace Fireasy.Data.Tests.SyntaxTest
{
    [TestClass]
    [TestCategory("SyntaxTests")]
    public class SyntaxTests_Dameng : SyntaxTests<DamengProvider>
    {
        protected override string ConnectionString => Constants.Dameng_ConnectionString;

        protected override string InstanceName => Constants.Dameng_InstanceName;
    }
}
