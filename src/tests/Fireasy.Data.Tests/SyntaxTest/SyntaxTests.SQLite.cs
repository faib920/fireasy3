namespace Fireasy.Data.Tests.SyntaxTest
{
    [TestClass]
    [TestCategory("SyntaxTests")]
    public class SyntaxTests_SQLite : SyntaxTests<SQLiteProvider>
    {
        protected override string ConnectionString => Constants.SQLite_ConnectionString;

        protected override string InstanceName => Constants.SQLite_InstanceName;
    }
}
