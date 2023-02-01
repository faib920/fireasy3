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
    /// Firebird日期函数语法解析。
    /// </summary>
    public class FirebirdDateTimeSyntax : IDateTimeSyntax
    {
        /// <summary>
        /// 获取当前时间。
        /// </summary>
        /// <returns></returns>
        public virtual string Now()
        {
            return "CURRENT_TIMESTAMP";
        }

        /// <summary>
        /// 获取当前 UTC 时间。
        /// </summary>
        /// <returns></returns>
        public virtual string UtcNow()
        {
            //东8区
            return "DATEADD(HOUR, -8, CURRENT_TIMESTAMP)";
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
            return $"CAST({yearExp} || '-' || {monthExp} || '-' || {dayExp} AS DATE)";
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
            return $"CAST({yearExp} || '-' || {monthExp} || '-' || {dayExp} || ' ' || {hourExp} || ':' || {minuteExp} || ':' || {secondExp} AS TIMESTAMP)";
        }

        /// <summary>
        /// 获取源表达式中的年份。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <returns></returns>
        public virtual string Year(object sourceExp)
        {
            return $"EXTRACT(YEAR FROM {TryCastDateTime(sourceExp)})";
        }

        /// <summary>
        /// 获取源表达式中的月份。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <returns></returns>
        public virtual string Month(object sourceExp)
        {
            return $"EXTRACT(MONTH FROM {TryCastDateTime(sourceExp)})";
        }

        /// <summary>
        /// 获取源表达式中的天数。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <returns></returns>
        public virtual string Day(object sourceExp)
        {
            return $"EXTRACT(DAY FROM {TryCastDateTime(sourceExp)})";
        }

        /// <summary>
        /// 获取源表达式中的小时。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <returns></returns>
        public virtual string Hour(object sourceExp)
        {
            return $"EXTRACT(HOUR FROM {TryCastDateTime(sourceExp)})";
        }

        /// <summary>
        /// 获取源表达式中的分。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <returns></returns>
        public virtual string Minute(object sourceExp)
        {
            return $"EXTRACT(MINUTE FROM {TryCastDateTime(sourceExp)})";
        }

        /// <summary>
        /// 获取源表达式中的秒。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <returns></returns>
        public virtual string Second(object sourceExp)
        {
            return $"EXTRACT(SECOND FROM {TryCastDateTime(sourceExp)})";
        }

        /// <summary>
        /// 获取源表达式中的毫秒。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <returns></returns>
        public virtual string Millisecond(object sourceExp)
        {
            return $"EXTRACT(MILLISECOND FROM {TryCastDateTime(sourceExp)})";
        }

        /// <summary>
        /// 获取源表达式中的本周的第几天。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <returns></returns>
        public virtual string DayOfWeek(object sourceExp)
        {
            return $"EXTRACT(WEEKDAY FROM {TryCastDateTime(sourceExp)})";
        }

        /// <summary>
        /// 获取源表达式中的本年的第几天。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <returns></returns>
        public virtual string DayOfYear(object sourceExp)
        {
            return $"EXTRACT(YEARDAY FROM {TryCastDateTime(sourceExp)}) + 1";
        }

        /// <summary>
        /// 获取源表达式中的本年的第几周。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <returns></returns>
        public virtual string WeekOfYear(object sourceExp)
        {
            return $"EXTRACT(WEEK FROM {TryCastDateTime(sourceExp)})";
        }

        /// <summary>
        /// 源表达式增加年。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <param name="yearExp">年份数，可为正可为负。</param>
        /// <returns></returns>
        public virtual string AddYears(object sourceExp, object yearExp)
        {
            return $"DATEADD(YEAR, {yearExp}, {TryCastDateTime(sourceExp)})";
        }

        /// <summary>
        /// 源表达式增加月。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <param name="monthExp">月份数，可为正可为负。</param>
        /// <returns></returns>
        public virtual string AddMonths(object sourceExp, object monthExp)
        {
            return $"DATEADD(MONTH, {monthExp}, {TryCastDateTime(sourceExp)})";
        }

        /// <summary>
        /// 源表达式增加天。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <param name="dayExp">天数，可为正可为负。</param>
        /// <returns></returns>
        public virtual string AddDays(object sourceExp, object dayExp)
        {
            return $"DATEADD(DAY, {dayExp}, {TryCastDateTime(sourceExp)})";
        }

        /// <summary>
        /// 源表达式增加小时。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <param name="hourExp">小时，可为正可为负。</param>
        /// <returns></returns>
        public virtual string AddHours(object sourceExp, object hourExp)
        {
            return $"DATEADD(HOUR, {hourExp}, {TryCastDateTime(sourceExp)})";
        }

        /// <summary>
        /// 源表达式增加分。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <param name="minuteExp">分，可为正可为负。</param>
        /// <returns></returns>
        public virtual string AddMinutes(object sourceExp, object minuteExp)
        {
            return $"DATEADD(MINUTE, {minuteExp}, {TryCastDateTime(sourceExp)})";
        }

        /// <summary>
        /// 源表达式增加秒。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <param name="secondExp">秒，可为正可为负。</param>
        /// <returns></returns>
        public virtual string AddSeconds(object sourceExp, object secondExp)
        {
            return $"DATEADD(SECOND, {secondExp}, {TryCastDateTime(sourceExp)})";
        }

        /// <summary>
        /// 计算两个表达式相差的天数。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <param name="otherExp">结束日期。</param>
        /// <returns></returns>
        public virtual string DiffDays(object sourceExp, object otherExp)
        {
            return $"DATEDIFF(DAY FROM {TryCastDateTime(sourceExp)} TO {TryCastDateTime(otherExp)})";
        }

        /// <summary>
        /// 计算两个表达式相差的小时数。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <param name="otherExp">结束日期。</param>
        /// <returns></returns>
        public virtual string DiffHours(object sourceExp, object otherExp)
        {
            return $"DATEDIFF(HOUR FROM {TryCastDateTime(sourceExp)} TO {TryCastDateTime(otherExp)})";
        }

        /// <summary>
        /// 计算两个表达式相差的分钟数。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <param name="otherExp">结束日期。</param>
        /// <returns></returns>
        public virtual string DiffMinutes(object sourceExp, object otherExp)
        {
            return $"DATEDIFF(MINUTE FROM {TryCastDateTime(sourceExp)} TO {TryCastDateTime(otherExp)})";
        }

        /// <summary>
        /// 计算两个表达式相差的秒数。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <param name="otherExp">结束日期。</param>
        /// <returns></returns>
        public virtual string DiffSeconds(object sourceExp, object otherExp)
        {
            return $"DATEDIFF(SECOND FROM {TryCastDateTime(sourceExp)} TO {TryCastDateTime(otherExp)})";
        }

        private object TryCastDateTime(object sourceExp)
        {
            if (sourceExp is string str && str.StartsWith("'"))
            {
                return $"CAST({sourceExp} AS TIMESTAMP)";
            }

            return sourceExp;
        }
    }
}
