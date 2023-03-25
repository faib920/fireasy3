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
    /// Kingbase记录包装器。
    /// </summary>
    public class KingbaseRecordWrapper : GeneralRecordWrapper
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
                case "KdbndpTypes.KdbndpBox":
                case "KdbndpTypes.KdbndpCircle":
                case "KdbndpTypes.KdbndpLine":
                case "KdbndpTypes.KdbndpLSeg":
                case "KdbndpTypes.KdbndpPath":
                case "KdbndpTypes.KdbndpPoint":
                case "KdbndpTypes.KdbndpPolygon":
                case "KdbndpTypes.KdbndpTsQuery":
                case "KdbndpTypes.KdbndpTsVector":
                    return typeof(string);
                case "System.Collections.BitArray":
                    return typeof(byte[]);
            }

            if (fieldType.IsGenericType && fieldType.GetGenericTypeDefinition().FullName == "KdbndpTypes.KdbndpRange`1")
            {
                switch (fieldType.GetGenericArguments()[0].FullName)
                {
                    case "System.Int32":
                        return typeof(int[]);
                    case "System.Int64":
                        return typeof(long[]);
                    case "System.DateTime":
                        return typeof(DateTime?[]);
                }

                return typeof(object);
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
            var fieldType = value.GetType();

            switch (fieldType.FullName)
            {
                case "KdbndpTypes.KdbndpBox":
                case "KdbndpTypes.KdbndpCircle":
                case "KdbndpTypes.KdbndpLine":
                case "KdbndpTypes.KdbndpLSeg":
                case "KdbndpTypes.KdbndpPath":
                case "KdbndpTypes.KdbndpLogSequenceNumber":
                case "KdbndpTypes.KdbndpPoint":
                case "KdbndpTypes.KdbndpPolygon":
                case "KdbndpTypes.KdbndpTsQuery":
                case "KdbndpTypes.KdbndpTsVector":
                case "System.Collections.BitArray":
                    if (value is BitArray array)
                    {
                        var bytes = new byte[array.Length];
                        array.CopyTo(bytes, 0);
                        return bytes;
                    }
                    break;
            }

            if (fieldType.IsGenericType && fieldType.GetGenericTypeDefinition().FullName == "KdbndpTypes.KdbndpRange`1")
            {
                var arr = value.ToString().Replace("[", string.Empty).Replace(")", string.Empty).Split(',');

                switch (fieldType.GetGenericArguments()[0].FullName)
                {
                    case "System.Int32":
                        return new int[] { GetInt32Value(arr[0]), GetInt32Value(arr[1]) };
                    case "System.Int64":
                        return new long[] { GetInt64Value(arr[0]), GetInt64Value(arr[1]) };
                    case "System.DateTime":
                        return new DateTime?[] { GetDateTimeValue(arr[0]), GetDateTimeValue(arr[1]) };
                }
            }

            return value;
        }

        private int GetInt32Value(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return 0;
            }

            return Convert.ToInt32(str);
        }

        private long GetInt64Value(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return 0L;
            }

            return Convert.ToInt64(str);
        }

        private DateTime? GetDateTimeValue(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return null;
            }

            return Convert.ToDateTime(str);
        }
    }
}
