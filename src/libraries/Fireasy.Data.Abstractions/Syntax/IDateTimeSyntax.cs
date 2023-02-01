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
    /// 日期时间函数语法。
    /// </summary>
    public interface IDateTimeSyntax
    {
        /// <summary>
        /// 源表达式增加天。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <param name="dayExp">天数，可为正可为负。</param>
        /// <returns></returns>
        string AddDays(object sourceExp, object dayExp);

        /// <summary>
        /// 源表达式增加小时。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <param name="hourExp">小时，可为正可为负。</param>
        /// <returns></returns>
        string AddHours(object sourceExp, object hourExp);

        /// <summary>
        /// 源表达式增加分。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <param name="minuteExp">分，可为正可为负。</param>
        /// <returns></returns>
        string AddMinutes(object sourceExp, object minuteExp);

        /// <summary>
        /// 源表达式增加月。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <param name="monthExp">月份数，可为正可为负。</param>
        /// <returns></returns>
        string AddMonths(object sourceExp, object monthExp);

        /// <summary>
        /// 源表达式增加秒。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <param name="secondExp">秒，可为正可为负。</param>
        /// <returns></returns>
        string AddSeconds(object sourceExp, object secondExp);

        /// <summary>
        /// 源表达式增加年。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <param name="yearExp">年份数，可为正可为负。</param>
        /// <returns></returns>
        string AddYears(object sourceExp, object yearExp);

        /// <summary>
        /// 获取源表达式中的天数。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <returns></returns>
        string Day(object sourceExp);

        /// <summary>
        /// 获取源表达式中的本周的第几天。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <returns></returns>
        string DayOfWeek(object sourceExp);

        /// <summary>
        /// 获取源表达式中的本年的第几天。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <returns></returns>
        string DayOfYear(object sourceExp);

        /// <summary>
        /// 计算两个表达式相差的天数。
        /// </summary>
        /// <param name="startExp">起始日期。</param>
        /// <param name="endExp">结束日期。</param>
        /// <returns></returns>
        string DiffDays(object startExp, object endExp);

        /// <summary>
        /// 计算两个表达式相差的小时数。
        /// </summary>
        /// <param name="startExp">起始日期。</param>
        /// <param name="endExp">结束日期。</param>
        /// <returns></returns>
        string DiffHours(object startExp, object endExp);

        /// <summary>
        /// 计算两个表达式相差的分钟数。
        /// </summary>
        /// <param name="startExp">起始日期。</param>
        /// <param name="endExp">结束日期。</param>
        /// <returns></returns>
        string DiffMinutes(object startExp, object endExp);

        /// <summary>
        /// 计算两个表达式相差的秒数。
        /// </summary>
        /// <param name="startExp">起始日期。</param>
        /// <param name="endExp">结束日期。</param>
        /// <returns></returns>
        string DiffSeconds(object startExp, object endExp);

        /// <summary>
        /// 获取源表达式中的小时。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <returns></returns>
        string Hour(object sourceExp);

        /// <summary>
        /// 获取源表达式中的毫秒。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <returns></returns>
        string Millisecond(object sourceExp);

        /// <summary>
        /// 获取源表达式中的分。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <returns></returns>
        string Minute(object sourceExp);

        /// <summary>
        /// 获取源表达式中的月份。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <returns></returns>
        string Month(object sourceExp);

        /// <summary>
        /// 初始化日期。
        /// </summary>
        /// <param name="yearExp">年表达式。</param>
        /// <param name="monthExp">月表达式。</param>
        /// <param name="dayExp">日表达式。</param>
        /// <returns></returns>
        string New(object yearExp, object monthExp, object dayExp);

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
        string New(object yearExp, object monthExp, object dayExp, object hourExp, object minuteExp, object secondExp);

        /// <summary>
        /// 获取当前时间。
        /// </summary>
        /// <returns></returns>
        string Now();

        /// <summary>
        /// 获取源表达式中的秒。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <returns></returns>
        string Second(object sourceExp);

        /// <summary>
        /// 获取当前 UTC 时间。
        /// </summary>
        /// <returns></returns>
        string UtcNow();

        /// <summary>
        /// 获取源表达式中的本年的第几周。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <returns></returns>
        string WeekOfYear(object sourceExp);

        /// <summary>
        /// 获取源表达式中的年份。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <returns></returns>
        string Year(object sourceExp);
    }
}