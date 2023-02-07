namespace Fireasy.Data.RecordWrapper
{
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
                    return typeof(object);
                case "System.Collections.BitArray":
                    return typeof(byte[]);
            }

            return fieldType;
        }
    }
}
