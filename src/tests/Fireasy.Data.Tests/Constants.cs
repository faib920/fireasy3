namespace Fireasy.Data.Tests
{
    public class Constants
    {
        public const string SQLite_ConnectionString = "Data source=|appdir|..\\..\\..\\..\\..\\..\\db\\Northwind.db3;Pooling=True";
        public const string MySql_ConnectionString = "Data Source=localhost;database=northwind;User Id=root;password=faib;pooling=true;charset=utf8";
        public const string SqlServer_ConnectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=|appdir|..\\..\\..\\..\\..\\..\\db\\Northwind.mdf;Integrated Security=True;Connect Timeout=30";
        public const string Oracle_ConnectionString = "Data Source=localhost/orcl;User ID=c##test;Password=Faib1234";
        public const string PostgreSql_ConnectionString = "Server=localhost;User Id=postgres;Password=faib;Database=postgres;";
        public const string Firebird_ConnectionString = "User=SYSDBA;Password=masterkey;Database=|appdir|..\\..\\..\\..\\..\\..\\db\\northwind.fdb; DataSource=localhost";
        public const string Dameng_ConnectionString = "Server=localhost; UserId=DM; PWD=1234567890";

        public const string SQLite_InstanceName = "sqlite";
        public const string MySql_InstanceName = "mysql";
        public const string SqlServer_InstanceName = "sqlserver";
        public const string Oracle_InstanceName = "oracle";
        public const string PostgreSql_InstanceName = "pgsql";
        public const string Firebird_InstanceName = "firebird";
        public const string Dameng_InstanceName = "dm";
    }
}
