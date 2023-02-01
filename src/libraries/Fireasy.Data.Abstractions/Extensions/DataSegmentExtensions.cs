namespace Fireasy.Data.Extensions
{
    public static class DataSegmentExtensions
    {
        /// <summary>
        /// 构造 <see cref="IDataSegment"/> 的分页条件。
        /// </summary>
        /// <param name="segment"></param>
        /// <param name="fieldName">分页列名称。</param>
        /// <returns></returns>
        public static string Condition(this IDataSegment segment, string fieldName)
        {
            if (segment.Start != null && segment.End != null)
            {
                return string.Format("{0} BETWEEN {1} AND {2}", fieldName, segment.Start, segment.End);
            }

            if (segment.Start != null && segment.End == null)
            {
                return string.Format("{0} >= {1}", fieldName, segment.Start);
            }

            if (segment.Start == null && segment.End != null)
            {
                return string.Format("{0} <= {1}", fieldName, segment.End);
            }

            return "1 = 1";
        }
    }
}
