namespace Fireasy.Data.Tests.SyntaxTest
{
    [TestClass]
    [TestCategory("SyntaxTests")]
    public class SyntaxTests_ShenTong : SyntaxTests<ShenTongProvider>
    {
        protected override string ConnectionString => Constants.ShenTong_ConnectionString;

        protected override string InstanceName => Constants.ShenTong_InstanceName;
    }
}
