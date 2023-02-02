namespace Fireasy.Data.Tests.SchemaTest
{
    [TestClass]
    [TestCategory("SchemaTests")]
    public class SchemaTests_Dameng : SchemaTests<DamengProvider>
    {
        protected override string ConnectionString => Constants.Dameng_ConnectionString;

        protected override string InstanceName => Constants.Dameng_InstanceName;
    }
}
