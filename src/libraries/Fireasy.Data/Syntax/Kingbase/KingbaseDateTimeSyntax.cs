// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Fireasy.Data.Syntax
{
    /// <summary>
    /// Kingbase日期函数语法解析。
    /// </summary>
    public class KingbaseDateTimeSyntax : IDateTimeSyntax
    {
        /// <summary>
        /// 获取当前时间。
        /// </summary>
        /// <returns></returns>
        public virtual string Now()
        {
            return "(NOW() AT TIME ZONE CURRENT_SETTING('TimeZone'))";
        }

        /// <summary>
        /// 获取当前 UTC 时间。
        /// </summary>
        /// <returns></returns>
        public virtual string UtcNow()
        {
            return "(NOW() AT TIME ZONE 'UTC')";
        }

        /// <summary>
        /// 初始化日期。
        /// </summary>
        /// <param name="yearExp">年表达式。</param>
        /// <param name="monthExp">月表达式。</param>
        /// <param name="dayExp">日表达式。</param>
        /// <returns></returns>
        public virtual string New(object yearExp, object monthExp, object dayExp)
        {
            return $"TO_DATE(CONCAT({yearExp}, '-', {monthExp}, '-', {dayExp}), 'YYYY-MM-DD')";
        }

        /// <summary>
        /// 初始化日期时间。
        /// </summary>
        /// <param name="yearExp">年表达式。</param>
        /// <param name="monthExp">月表达式。</param>
        /// <param name="dayExp">日表达式。</param>
        /// <param name="hourExp">时表达式。</param>
        /// <param name="minuteExp">分表达式。</param>
        /// <param name="secondExp">秒表达式。</param>
        /// <returns></returns>
        public virtual string New(object yearExp, object monthExp, object dayExp, object hourExp, object minuteExp, object secondExp)
        {
            return $"CONCAT({yearExp}, '-', {monthExp}, '-', {dayExp}, ' ', {hourExp}, ':', {minuteExp}, ':', {secondExp})::TIMESTAMP";
        }

        /// <summary>
        /// 获取源表达式中的年份。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <returns></returns>
        public virtual string Year(object sourceExp)
        {
            return $"DATE_PART('YEAR', {TryCastDateTime(sourceExp)})";
        }

        /// <summary>
        /// 获取源表达式中的月份。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <returns></returns>
        public virtual string Month(object sourceExp)
        {
            return $"DATE_PART('MONTH', {TryCastDateTime(sourceExp)})";
        }

        /// <summary>
        /// 获取源表达式中的天数。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <returns></returns>
        public virtual string Day(object sourceExp)
        {
            return $"DATE_PART('DAY', {TryCastDateTime(sourceExp)})";
        }

        /// <summary>
        /// 获取源表达式中的小时。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <returns></returns>
        public virtual string Hour(object sourceExp)
        {
            return $"DATE_PART('HOUR', {TryCastDateTime(sourceExp)} )";
        }

        /// <summary>
        /// 获取源表达式中的分。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <returns></returns>
        public virtual string Minute(object sourceExp)
        {
            return $"DATE_PART('MINUTE', {TryCastDateTime(sourceExp)} )";
        }

        /// <summary>
        /// 获取源表达式中的秒。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <returns></returns>
        public virtual string Second(object sourceExp)
        {
            return $"DATE_PART('SECOND', {TryCastDateTime(sourceExp)} )";
        }

        /// <summary>
        /// 获取源表达式中的毫秒。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <returns></returns>
        public virtual string Millisecond(object sourceExp)
        {
            return $"DATE_PART('MILLISECONDS', {TryCastDateTime(sourceExp)} )";
        }

        /// <summary>
        /// 获取源表达式中的本周的第几天。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <returns></returns>
        public virtual string DayOfWeek(object sourceExp)
        {
            return $"DATE_PART('DOW', {TryCastDateTime(sourceExp)} )";
        }

        /// <summary>
        /// 获取源表达式中的本年的第几天。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <returns></returns>
        public virtual string DayOfYear(object sourceExp)
        {
            return $"DATE_PART('DOY', {TryCastDateTime(sourceExp)} )";
        }

        /// <summary>
        /// 获取源表达式中的本年的第几周。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <returns></returns>
        public virtual string WeekOfYear(object sourceExp)
        {
            return $"DATE_PART('WEEK', {TryCastDateTime(sourceExp)} )";
        }

        /// <summary>
        /// 源表达式增加年。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <param name="yearExp">年份数，可为正可为负。</param>
        /// <returns></returns>
        public virtual string AddYears(object sourceExp, object yearExp)
        {
            return $"{TryCastDateTime(sourceExp)} + INTERVAL '{yearExp} YEARS'";
        }

        /// <summary>
        /// 源表达式增加月。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <param name="monthExp">月份数，可为正可为负。</param>
        /// <returns></returns>
        public virtual string AddMonths(object sourceExp, object monthExp)
        {
            return $"{TryCastDateTime(sourceExp)} + INTERVAL '{monthExp} MONTHS'";
        }

        /// <summary>
        /// 源表达式增加天。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <param name="dayExp">天数，可为正可为负。</param>
        /// <returns></returns>
        public virtual string AddDays(object sourceExp, object dayExp)
        {
            return $"{TryCastDateTime(sourceExp)} + INTERVAL '{dayExp} DAYS'";
        }

        /// <summary>
        /// 源表达式增加小时。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <param name="hourExp">小时，可为正可为负。</param>
        /// <returns></returns>
        public virtual string AddHours(object sourceExp, object hourExp)
        {
            return $"{TryCastDateTime(sourceExp)} + INTERVAL '{hourExp} HOURS'";
        }

        /// <summary>
        /// 源表达式增加分。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <param name="minuteExp">分，可为正可为负。</param>
        /// <returns></returns>
        public virtual string AddMinutes(object sourceExp, object minuteExp)
        {
            return $"{TryCastDateTime(sourceExp)} + INTERVAL '{minuteExp} MINUTES'";
        }

        /// <summary>
        /// 源表达式增加秒。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <param name="secondExp">秒，可为正可为负。</param>
        /// <returns></returns>
        public virtual string AddSeconds(object sourceExp, object secondExp)
        {
            return $"{TryCastDateTime(sourceExp)} + INTERVAL '{secondExp} SECONDS'";
        }

        /// <summary>
        /// 计算两个表达式相差的天数。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <param name="otherExp">结束日期。</param>
        /// <returns></returns>
        public virtual string DiffDays(object sourceExp, object otherExp)
        {
            return $"DATE_PART('DAY', {TryCastDateTime(otherExp)}-{TryCastDateTime(sourceExp)})";
        }

        /// <summary>
        /// 计算两个表达式相差的小时数。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <param name="otherExp">结束日期。</param>
        /// <returns></returns>
        public virtual string DiffHours(object sourceExp, object otherExp)
        {
            return $"(DATE_PART('DAY', {TryCastDateTime(otherExp)}-{TryCastDateTime(sourceExp)}) * 24 + DATE_PART('HOUR', {TryCastDateTime(otherExp)}-{TryCastDateTime(sourceExp)}))";
        }

        /// <summary>
        /// 计算两个表达式相差的分钟数。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <param name="otherExp">结束日期。</param>
        /// <returns></returns>
        public virtual string DiffMinutes(object sourceExp, object otherExp)
        {
            return $"(DATE_PART('DAY', {TryCastDateTime(otherExp)}-{TryCastDateTime(sourceExp)}) * 1440 + DATE_PART('HOUR', {otherExp}::timestamp-{sourceExp}::timestamp) * 60 + DATE_PART('MINUTE', {otherExp}::timestamp-{sourceExp}::timestamp))";
        }

        /// <summary>
        /// 计算两个表达式相差的秒数。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <param name="otherExp">结束日期。</param>
        /// <returns></returns>
        public virtual string DiffSeconds(object sourceExp, object otherExp)
        {
            return $"(DATE_PART('DAY', {TryCastDateTime(otherExp)}-{TryCastDateTime(sourceExp)}) * 86400 + DATE_PART('HOUR', {TryCastDateTime(otherExp)}-{TryCastDateTime(sourceExp)}) * 3600 + DATE_PART('MINUTE', {TryCastDateTime(otherExp)}-{TryCastDateTime(sourceExp)}) * 60 + DATE_PART('SECOND', {TryCastDateTime(otherExp)}-{TryCastDateTime(sourceExp)}))";
        }

        private object TryCastDateTime(object sourceExp)
        {
            if (sourceExp is string str && str.StartsWith("'"))
            {
                return $"{sourceExp}::TIMESTAMP";
            }

            return sourceExp;
        }

    }
}
