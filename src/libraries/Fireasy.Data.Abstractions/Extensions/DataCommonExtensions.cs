using Fireasy.Common;
using Fireasy.Common.Dynamic;
using Fireasy.Common.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Text;

namespace Fireasy.Data
{
    /// <summary>
    /// 
    /// </summary>
    public static class DataCommonExtensions
    {
        /// <summary>
        /// 将数组或 <see cref="IEnumerable"/> 中的成员转换到 <see cref="DataTable"/> 对象。
        /// </summary>
        /// <param name="data"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static DataTable ToDataTable(this object data, string tableName = null)
        {
            if (data == null)
            {
                return null;
            }

            if (!(data is DataTable table))
            {
                if (data is IEnumerable)
                {
                    table = ParseFromEnumerable(data as IEnumerable);
                }
                else
                {
                    table = ParseFromEnumerable(new List<object> { data });
                }
            }

            if (table != null)
            {
                table.TableName = tableName;
            }

            return table;
        }

        /// <summary>
        /// 将 <see cref="DataTable"/> 对象输出为 Insert 语句表示。
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public static IEnumerable<string> ToInsertSql(this DataTable table)
        {
            foreach (DataRow row in table.Rows)
            {
                var sb = new StringBuilder();
                sb.AppendFormat("INSERT INTO {0} VALUES(", table.TableName);
                var flag = new AssertFlag();
                foreach (DataColumn column in table.Columns)
                {
                    if (!flag.AssertTrue())
                    {
                        sb.Append(",");
                    }

                    if (row[column] == DBNull.Value)
                    {
                        sb.Append("NULL");
                    }
                    else if (column.DataType == typeof(string) || column.DataType == typeof(DateTime))
                    {
                        sb.AppendFormat("'{0}'", row[column]);
                    }
                    else if (column.DataType == typeof(bool))
                    {
                        sb.Append(Convert.ToInt32(row[column]));
                    }
                    else
                    {
                        sb.Append(row[column]);
                    }
                }

                sb.Append(")");
                yield return sb.ToString();
            }
        }

        /// <summary>
        /// 将 <see cref="DataTable"/> 中的数据转换为数组。
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public static object[][] ToArray(this DataTable table)
        {
            var rows = table.Rows.Count;
            var cols = table.Columns.Count;
            var data = new object[cols][];

            for (var i = 0; i < cols; i++)
            {
                data[i] = new object[rows];
            }

            for (var i = 0; i < rows; i++)
            {
                for (var j = 0; j < cols; j++)
                {
                    data[j][i] = table.Rows[i][j];
                }
            }

            return data;
        }

        /// <summary>
        /// 判断 <see cref="DataSet"/> 是否为 null 或没有任何的 <see cref="DataTable"/> 成员。
        /// </summary>
        /// <param name="ds"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this DataSet ds)
        {
            return ds == null || ds.Tables.Count == 0;
        }

        /// <summary>
        /// 判断 <see cref="DataTable"/> 是否为 null 或没有任何的 <see cref="DataRow"/> 成员。
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this DataTable table)
        {
            return table == null || table.Rows.Count == 0;
        }

        /// <summary>
        /// 循环取 <see cref="DataTable"/> 中的 <see cref="DataColumn"/> 集合。
        /// </summary>
        /// <param name="table"></param>
        /// <param name="action"></param>
        /// <param name="predicate"></param>
        public static void EachColumns(this DataTable table, Action<DataColumn, int> action, Func<DataColumn, bool> predicate = null)
        {
            if (table.IsNullOrEmpty() || action == null)
            {
                return;
            }

            var count = table.Columns.Count;
            for (var i = 0; i < count; i++)
            {
                var column = table.Columns[i];
                if (predicate != null && !predicate(column))
                {
                    continue;
                }

                action(column, i);
            }
        }

        /// <summary>
        /// 循环取 <see cref="DataTable"/> 中的 <see cref="DataColumn"/> 集合。
        /// </summary>
        /// <param name="table"></param>
        /// <param name="action"></param>
        /// <param name="predicate"></param>
        public static void EachColumns(this DataTable table, Action<DataColumn> action, Func<DataColumn, bool> predicate = null)
        {
            if (table.IsNullOrEmpty() || action == null)
            {
                return;
            }

            var count = table.Columns.Count;
            for (var i = 0; i < count; i++)
            {
                var column = table.Columns[i];
                if (predicate != null && !predicate(column))
                {
                    continue;
                }

                action(column);
            }
        }

        /// <summary>
        /// 循环取 <see cref="DataTable"/> 中的 <see cref="DataRow"/> 集合。
        /// </summary>
        /// <param name="table"></param>
        /// <param name="action"></param>
        /// <param name="predicate"></param>
        public static void EachRows(this DataTable table, Action<DataRow, int> action, Func<DataRow, bool> predicate = null)
        {
            if (table.IsNullOrEmpty() || action == null)
            {
                return;
            }

            var count = table.Rows.Count;
            for (var i = 0; i < count; i++)
            {
                var row = table.Rows[i];
                if (predicate != null && !predicate(row))
                {
                    continue;
                }

                action(row, i);
            }
        }

        /// <summary>
        /// 循环取 <see cref="DataTable"/> 中的 <see cref="DataRow"/> 集合。
        /// </summary>
        /// <param name="table"></param>
        /// <param name="action"></param>
        /// <param name="predicate"></param>
        public static void EachRows(this DataTable table, Action<DataRow> action, Func<DataRow, bool> predicate = null)
        {
            if (table.IsNullOrEmpty() || action == null)
            {
                return;
            }

            var count = table.Rows.Count;
            for (var i = 0; i < count; i++)
            {
                var row = table.Rows[i];
                if (predicate != null && !predicate(row))
                {
                    continue;
                }

                action(row);
            }
        }

        /// <summary>
        /// 获取是否支持数据类型。
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsDbTypeSupported(this Type type)
        {
            Guard.ArgumentNull(type, nameof(type));
            type = type.GetNonNullableType();
            var typeCode = Type.GetTypeCode(type);

            return typeCode != TypeCode.Object &&
                typeCode != TypeCode.Empty &&
                typeCode != TypeCode.DBNull;
        }

        /// <summary>
        /// 判断 <paramref name="dbType"/> 是否为字符串类型。
        /// </summary>
        /// <param name="dbType"></param>
        /// <returns></returns>
        public static bool IsStringDbType(this DbType dbType)
        {
            return dbType == DbType.String ||
                dbType == DbType.StringFixedLength ||
                dbType == DbType.AnsiString ||
                dbType == DbType.AnsiStringFixedLength;
        }

        private static DataTable ParseFromEnumerable(IEnumerable enumerable)
        {
            var enumerator = enumerable.GetEnumerator();
            var table = new DataTable();
            var flag = new AssertFlag();
            PropertyDescriptorCollection properties = null;
            while (enumerator.MoveNext())
            {
                var current = enumerator.Current;
                if (flag.AssertTrue())
                {
                    properties = TypeDescriptor.GetProperties(current);
                    foreach (PropertyDescriptor pro in properties)
                    {
                        table.Columns.Add(pro.Name, pro.PropertyType.GetNonNullableType());
                    }
                }

                var data = new object[properties.Count];
                for (int i = 0, n = data.Length; i < n; i++)
                {
                    data[i] = properties[i].GetValue(current) ?? DBNull.Value;
                }

                table.Rows.Add(data);
            }

            return table;
        }
    }
}
