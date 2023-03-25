// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Fireasy.Data.RecordWrapper
{
    /// <summary>
    /// PostgreSql记录包装器。
    /// </summary>
    public class PostgreSqlRecordWrapper : GeneralRecordWrapper
    {
        /// <summary>
        /// 获取指定索引处的字段类型。
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        public override Type GetFieldType(IDataReader reader, int i)
        {
            var fieldType = base.GetFieldType(reader, i);

            switch (fieldType.FullName)
            {
                case "NpgsqlTypes.NpgsqlBox":
                case "NpgsqlTypes.NpgsqlCircle":
                case "NpgsqlTypes.NpgsqlLine":
                case "NpgsqlTypes.NpgsqlLSeg":
                case "NpgsqlTypes.NpgsqlPath":
                case "NpgsqlTypes.NpgsqlLogSequenceNumber":
                case "NpgsqlTypes.NpgsqlPoint":
                case "NpgsqlTypes.NpgsqlPolygon":
                case "NpgsqlTypes.NpgsqlTsQuery":
                case "NpgsqlTypes.NpgsqlTsVector":
                    return typeof(string);
                case "System.Collections.BitArray":
                    return typeof(byte[]);
            }

            return fieldType;
        }

        /// <summary>
        /// 返回指定字段的值。
        /// </summary>
        /// <param name="reader">一个 <see cref="IDataRecord"/> 对象。</param>
        /// <param name="i">字段的索引。</param>
        /// <returns>该字段包含的对象。</returns>
        public override object? GetValue(IDataRecord reader, int i)
        {
            if (reader.IsDBNull(i))
            {
                return DBNull.Value;
            }

            var value = base.GetValue(reader, i);
            switch (value.GetType().FullName)
            {
                case "NpgsqlTypes.NpgsqlBox":
                case "NpgsqlTypes.NpgsqlCircle":
                case "NpgsqlTypes.NpgsqlLine":
                case "NpgsqlTypes.NpgsqlLSeg":
                case "NpgsqlTypes.NpgsqlPath":
                case "NpgsqlTypes.NpgsqlLogSequenceNumber":
                case "NpgsqlTypes.NpgsqlPoint":
                case "NpgsqlTypes.NpgsqlPolygon":
                case "NpgsqlTypes.NpgsqlTsQuery":
                case "NpgsqlTypes.NpgsqlTsVector":
                    return value?.ToString();
                case "System.Collections.BitArray":
                    if (value is BitArray array)
                    {
                        var bytes = new byte[array.Length];
                        array.CopyTo(bytes, 0);
                        return bytes;
                    }
                    break;
            }

            return value;
        }
    }
}
