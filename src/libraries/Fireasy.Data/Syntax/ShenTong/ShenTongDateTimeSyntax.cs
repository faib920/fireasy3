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
    /// ShenTong日期函数语法解析。
    /// </summary>
    public class ShenTongDateTimeSyntax : IDateTimeSyntax
    {
        /// <summary>
        /// 获取当前时间。
        /// </summary>
        /// <returns></returns>
        public virtual string Now()
        {
            return "SYSTIMESTAMP";
        }

        /// <summary>
        /// 获取当前 UTC 时间。
        /// </summary>
        /// <returns></returns>
        public virtual string UtcNow()
        {
            return "SYS_EXTRACT_UTC(SYSTIMESTAMP)";
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
            return $"TO_DATE({yearExp} || '-' || {monthExp} || '-' || {dayExp},'YYYY-MM-DD')";
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
            return $"TO_TIMESTAMP({yearExp} || '-' || {monthExp} || '-' || {dayExp} || ' ' || {hourExp} || ':' || {minuteExp} || ':' || {secondExp},'YYYY-MM-DD HH24:MI:SS')";
        }

        /// <summary>
        /// 获取源表达式中的年份。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <returns></returns>
        public virtual string Year(object sourceExp)
        {
            return $"TO_NUMBER(TO_CHAR({TryCastDateTime(sourceExp)},'YYYY'))";
        }

        /// <summary>
        /// 获取源表达式中的月份。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <returns></returns>
        public virtual string Month(object sourceExp)
        {
            return $"TO_NUMBER(TO_CHAR({TryCastDateTime(sourceExp)},'MM'))";
        }

        /// <summary>
        /// 获取源表达式中的天数。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <returns></returns>
        public virtual string Day(object sourceExp)
        {
            return $"TO_NUMBER(TO_CHAR({TryCastDateTime(sourceExp)},'DD'))";
        }

        /// <summary>
        /// 获取源表达式中的小时。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <returns></returns>
        public virtual string Hour(object sourceExp)
        {
            return $"TO_NUMBER(TO_CHAR({TryCastDateTime(sourceExp)},'HH24'))";
        }

        /// <summary>
        /// 获取源表达式中的分。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <returns></returns>
        public virtual string Minute(object sourceExp)
        {
            return $"TO_NUMBER(TO_CHAR({TryCastDateTime(sourceExp)},'MI'))";
        }

        /// <summary>
        /// 获取源表达式中的秒。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <returns></returns>
        public virtual string Second(object sourceExp)
        {
            return $"TO_NUMBER(TO_CHAR({TryCastDateTime(sourceExp)},'SS'))";
        }

        /// <summary>
        /// 获取源表达式中的毫秒。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <returns></returns>
        public virtual string Millisecond(object sourceExp)
        {
            throw new SyntaxNotSupportedException(nameof(Millisecond));
        }

        /// <summary>
        /// 获取源表达式中的本周的第几天。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <returns></returns>
        public virtual string DayOfWeek(object sourceExp)
        {
            return $"(TO_NUMBER(TO_CHAR({TryCastDateTime(sourceExp)},'D')) - 1)";
        }

        /// <summary>
        /// 获取源表达式中的本年的第几天。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <returns></returns>
        public virtual string DayOfYear(object sourceExp)
        {
            return $"TO_NUMBER(TO_CHAR({TryCastDateTime(sourceExp)},'DDD'))";
        }

        /// <summary>
        /// 获取源表达式中的本年的第几周。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <returns></returns>
        public virtual string WeekOfYear(object sourceExp)
        {
            return $"TO_NUMBER(TO_CHAR({TryCastDateTime(sourceExp)},'WW'))";
        }

        /// <summary>
        /// 源表达式增加年。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <param name="yearExp">年份数，可为正可为负。</param>
        /// <returns></returns>
        public virtual string AddYears(object sourceExp, object yearExp)
        {
            return $"ADD_MONTHS({TryCastDateTime(sourceExp)}, 12 * {yearExp})";
        }

        /// <summary>
        /// 源表达式增加月。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <param name="monthExp">月份数，可为正可为负。</param>
        /// <returns></returns>
        public virtual string AddMonths(object sourceExp, object monthExp)
        {
            return $"ADD_MONTHS({TryCastDateTime(sourceExp)}, {monthExp})";
        }

        /// <summary>
        /// 源表达式增加天。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <param name="dayExp">天数，可为正可为负。</param>
        /// <returns></returns>
        public virtual string AddDays(object sourceExp, object dayExp)
        {
            return $"({TryCastDateTime(sourceExp)} + {dayExp})";
        }

        /// <summary>
        /// 源表达式增加小时。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <param name="hourExp">小时，可为正可为负。</param>
        /// <returns></returns>
        public virtual string AddHours(object sourceExp, object hourExp)
        {
            return $"({TryCastDateTime(sourceExp)} + (1 / 24) * {hourExp})";
        }

        /// <summary>
        /// 源表达式增加分。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <param name="minuteExp">分，可为正可为负。</param>
        /// <returns></returns>
        public virtual string AddMinutes(object sourceExp, object minuteExp)
        {
            return $"{TryCastDateTime(sourceExp)} + (1 / 24 / 60) * {minuteExp}";
        }

        /// <summary>
        /// 源表达式增加秒。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <param name="secondExp">秒，可为正可为负。</param>
        /// <returns></returns>
        public virtual string AddSeconds(object sourceExp, object secondExp)
        {
            return $"({TryCastDateTime(sourceExp)} + (1 / 24 / 60 / 60) * {secondExp})";
        }

        /// <summary>
        /// 计算两个表达式相差的天数。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <param name="otherExp">结束日期。</param>
        /// <returns></returns>
        public virtual string DiffDays(object sourceExp, object otherExp)
        {
            return $"ROUND({TryCastDateTime(otherExp)} - {TryCastDateTime(sourceExp)})";
        }

        /// <summary>
        /// 计算两个表达式相差的小时数。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <param name="otherExp">结束日期。</param>
        /// <returns></returns>
        public virtual string DiffHours(object sourceExp, object otherExp)
        {
            throw new SyntaxNotSupportedException(nameof(DiffHours));
        }

        /// <summary>
        /// 计算两个表达式相差的分钟数。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <param name="otherExp">结束日期。</param>
        /// <returns></returns>
        public virtual string DiffMinutes(object sourceExp, object otherExp)
        {
            throw new SyntaxNotSupportedException(nameof(DiffMinutes));
        }

        /// <summary>
        /// 计算两个表达式相差的秒数。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <param name="otherExp">结束日期。</param>
        /// <returns></returns>
        public virtual string DiffSeconds(object sourceExp, object otherExp)
        {
            throw new SyntaxNotSupportedException(nameof(DiffSeconds));
        }

        private object TryCastDateTime(object sourceExp)
        {
            if (sourceExp is string str && str.StartsWith("'"))
            {
                return $"TO_TIMESTAMP({sourceExp},'YYYY-MM-DD HH24:MI:SS')";
            }

            return sourceExp;
        }

    }
}
