namespace Fireasy.Data.Tests.SchemaTest
{
    [TestClass]
    [TestCategory("SchemaTests")]
    public class SchemaTests_SQLite : SchemaTests<SQLiteProvider>
    {
        protected override string ConnectionString => Constants.SQLite_ConnectionString;

        protected override string InstanceName => Constants.SQLite_InstanceName;
    }
}
