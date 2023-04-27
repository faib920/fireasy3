// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Fireasy.Data
{
    /// <summary>
    /// 连接串参数名称。
    /// </summary>
    public class ConnectionParameterKeys
    {
        public static string[] Server = new[] { "data source", "server", "host" };
        public static string[] Database = new[] { "database", "initial catalog", "db" };
        public static string[] UserId = new[] { "user id", "userid", "uid", "username", "user name", "user" };
        public static string[] Password = new[] { "password", "pwd" };
    }
}
