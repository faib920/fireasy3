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
        public const string Kingbase_ConnectionString = "Server=localhost;port=54321;User Id=system;Password=faib;Database=test;";
        public const string ShenTong_ConnectionString = "Data Source=127.0.0.1;User ID=northwind;Password=faib";
        public const string OleDb_Access_ConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=|appdir|..\\..\\..\\..\\..\\..\\db\\Northwind.accdb;Persist Security Info=False;";
        public const string OleDb_Excel_ConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=|appdir|..\\..\\..\\..\\..\\..\\db\\Northwind.xlsx;Extended Properties='Excel 8.0;HDR=Yes;IMEX=1'";
        public const string OleDb_SqlServer_ConnectionString = "Provider=SQLOLEDB.1;Integrated Security=SSPI;Persist Security Info=False;Data Source=(local);Initial File Name=|appdir|..\\..\\..\\..\\..\\..\\db\\Northwind.mdf";

        public const string SQLite_InstanceName = "sqlite";
        public const string MySql_InstanceName = "mysql";
        public const string SqlServer_InstanceName = "sqlserver";
        public const string Oracle_InstanceName = "oracle";
        public const string PostgreSql_InstanceName = "pgsql";
        public const string Firebird_InstanceName = "firebird";
        public const string Dameng_InstanceName = "dm";
        public const string Kingbase_InstanceName = "kingbase";
        public const string ShenTong_InstanceName = "shentong";
        public const string OleDb_Access_InstanceName = "oledb_access";
        public const string OleDb_Excel_InstanceName = "oledb_excel";
        public const string OleDb_SqlServer_InstanceName = "oledb_sqlserver";
    }
}
