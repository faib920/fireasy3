﻿// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Fireasy.Data.Syntax
{
    /// <summary>
    /// OleDb驱动日期时间函数语法解析。
    /// </summary>
    public class OleDbDateTimeSyntax : IDateTimeSyntax
    {
        /// <summary>
        /// 初始化日期。
        /// </summary>
        /// <param name="yearExp"></param>
        /// <param name="monthExp"></param>
        /// <param name="dayExp"></param>
        /// <returns></returns>
        public string New(object yearExp, object monthExp, object dayExp)
        {
            return $"DATESERIAL({yearExp}, {monthExp}, {dayExp})";
        }

        /// <summary>
        /// 获取当前时间。
        /// </summary>
        /// <returns></returns>
        public string Now()
        {
            return "NOW()";
        }

        /// <summary>
        /// 获取当前 UTC 时间。
        /// </summary>
        /// <returns></returns>
        public string UtcNow()
        {
            throw new SyntaxNotSupportedException(nameof(UtcNow));
        }

        /// <summary>
        /// 初始化日期时间。
        /// </summary>
        /// <param name="yearExp"></param>
        /// <param name="monthExp"></param>
        /// <param name="dayExp"></param>
        /// <param name="hourExp"></param>
        /// <param name="minuteExp"></param>
        /// <param name="secondExp"></param>
        /// <returns></returns>
        public string New(object yearExp, object monthExp, object dayExp, object hourExp, object minuteExp, object secondExp)
        {
            return $"CDATE({yearExp} & '/' & {monthExp} & '/' & {dayExp} & ' ' & {hourExp} & ':' & {minuteExp} & ':' & {secondExp})";
        }

        /// <summary>
        /// 获取源表达式中的年份。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <returns></returns>
        public string Year(object sourceExp)
        {
            return $"DATEPART('yyyy', {sourceExp})";
        }

        /// <summary>
        /// 获取源表达式中的月份。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <returns></returns>
        public string Month(object sourceExp)
        {
            return $"DATEPART('m', {sourceExp})";
        }

        /// <summary>
        /// 获取源表达式中的天数。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <returns></returns>
        public string Day(object sourceExp)
        {
            return $"DATEPART('d', {sourceExp})";
        }

        /// <summary>
        /// 获取源表达式中的小时。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <returns></returns>
        public string Hour(object sourceExp)
        {
            return $"DATEPART('h', {sourceExp})";
        }

        /// <summary>
        /// 获取源表达式中的分。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <returns></returns>
        public string Minute(object sourceExp)
        {
            return $"DATEPART('n', {sourceExp})";
        }

        /// <summary>
        /// 获取源表达式中的秒。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <returns></returns>
        public string Second(object sourceExp)
        {
            return $"DATEPART('s', {sourceExp})";
        }

        /// <summary>
        /// 获取源表达式中的毫秒。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <returns></returns>
        public string Millisecond(object sourceExp)
        {
            throw new SyntaxNotSupportedException(nameof(Millisecond));
        }

        /// <summary>
        /// 获取源表达式中的本周的第几天。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <returns></returns>
        public string DayOfWeek(object sourceExp)
        {
            return $"DATEPART('w', {sourceExp}) - 1";
        }

        /// <summary>
        /// 获取源表达式中的本年的第几天。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <returns></returns>
        public string DayOfYear(object sourceExp)
        {
            return $"DATEPART('y', {sourceExp})";
        }

        /// <summary>
        /// 获取源表达式中的本年的第几周。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <returns></returns>
        public string WeekOfYear(object sourceExp)
        {
            return $"DATEPART('ww', {sourceExp})";
        }

        /// <summary>
        /// 源表达式增加年。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <param name="yearExp">年份数，可为正可为负。</param>
        /// <returns></returns>
        public string AddYears(object sourceExp, object yearExp)
        {
            return $"DATEADD('yyyy', {yearExp}, {sourceExp})";
        }

        /// <summary>
        /// 源表达式增加月。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <param name="monthExp">月份数，可为正可为负。</param>
        /// <returns></returns>
        public string AddMonths(object sourceExp, object monthExp)
        {
            return $"DATEADD('m', {monthExp}, {sourceExp})";
        }

        /// <summary>
        /// 源表达式增加天。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <param name="dayExp">天数，可为正可为负。</param>
        /// <returns></returns>
        public string AddDays(object sourceExp, object dayExp)
        {
            return $"DATEADD('d', {dayExp}, {sourceExp})";
        }
        /// <summary>
        /// 源表达式增加小时。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <param name="hourExp">小时，可为正可为负。</param>
        /// <returns></returns>
        public string AddHours(object sourceExp, object hourExp)
        {
            return $"DATEADD('h', {hourExp}, {sourceExp})";
        }

        /// <summary>
        /// 源表达式增加分。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <param name="minuteExp">分，可为正可为负。</param>
        /// <returns></returns>
        public string AddMinutes(object sourceExp, object minuteExp)
        {
            return $"DATEADD('n', {minuteExp}, {sourceExp})";
        }

        /// <summary>
        /// 源表达式增加秒。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <param name="secondExp">秒，可为正可为负。</param>
        /// <returns></returns>
        public string AddSeconds(object sourceExp, object secondExp)
        {
            return $"DATEADD('s', {secondExp}, {sourceExp})";
        }


        /// <summary>
        /// 计算两个表达式相差的天数。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <param name="otherExp">结束日期。</param>
        /// <returns></returns>
        public string DiffDays(object sourceExp, object otherExp)
        {
            return $"DATEDIFF('d', {sourceExp}, {otherExp})";
        }

        /// <summary>
        /// 计算两个表达式相差的小时数。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <param name="otherExp">结束日期。</param>
        /// <returns></returns>
        public string DiffHours(object sourceExp, object otherExp)
        {
            return $"DATEDIFF('h', {sourceExp}, {otherExp})";
        }

        /// <summary>
        /// 计算两个表达式相差的分钟数。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <param name="otherExp">结束日期。</param>
        /// <returns></returns>
        public string DiffMinutes(object sourceExp, object otherExp)
        {
            return $"DATEDIFF('n', {sourceExp}, {otherExp})";
        }

        /// <summary>
        /// 计算两个表达式相差的秒数。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <param name="otherExp">结束日期。</param>
        /// <returns></returns>
        public string DiffSeconds(object sourceExp, object otherExp)
        {
            return $"DATEDIFF('s', {sourceExp}, {otherExp})";
        }
    }
}
