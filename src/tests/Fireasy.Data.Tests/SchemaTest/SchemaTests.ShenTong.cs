namespace Fireasy.Data.Tests.SchemaTest
{
    [TestClass]
    [TestCategory("SchemaTests")]
    public class SchemaTests_ShenTong : SchemaTests<ShenTongProvider>
    {
        protected override string ConnectionString => Constants.ShenTong_ConnectionString;

        protected override string InstanceName => Constants.ShenTong_InstanceName;
    }
}
