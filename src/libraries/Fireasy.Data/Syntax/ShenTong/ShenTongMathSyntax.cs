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
    /// ShenTong数学函数语法解析。
    /// </summary>
    public class ShenTongMathSyntax : IMathSyntax
    {
        /// <summary>
        /// 两个表达式进行与运算。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <param name="otherExp">参与运算的表达式。</param>
        /// <returns></returns>
        public virtual string BitAnd(object sourceExp, object otherExp)
        {
            return $"BITAND({sourceExp}, {otherExp})";
        }

        /// <summary>
        /// 两个表达式进行或运算。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <param name="otherExp">参与运算的表达式。</param>
        /// <returns></returns>
        public virtual string BitOr(object sourceExp, object otherExp)
        {
            return $"({sourceExp} + {otherExp}) - BITAND({sourceExp}, {otherExp})";
        }

        /// <summary>
        /// 返回源表达式的非值。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <returns></returns>
        public virtual string BitNot(object sourceExp)
        {
            return $"(-1 - {sourceExp})";
        }

        /// <summary>
        /// 对源表达式进行求余运算。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <param name="otherExp">参与运算的表达式。</param>
        /// <returns></returns>
        public virtual string Modulo(object sourceExp, object otherExp)
        {
            return $"MOD({sourceExp}, {otherExp})";
        }

        /// <summary>
        /// 对两个表达式进行异或运算。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <param name="otherExp">参与运算的表达式。</param>
        /// <returns></returns>
        public virtual string ExclusiveOr(object sourceExp, object otherExp)
        {
            return $"({sourceExp} + {otherExp}) - BITAND({sourceExp}, {otherExp}) * 2";
        }

        /// <summary>
        /// 返回源表达式的最小整数值。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <returns></returns>
        public virtual string Ceiling(object sourceExp)
        {
            return $"CEIL({sourceExp})";
        }

        /// <summary>
        /// 将源表达式的小数位舍入。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <param name="digitExp">小数位数。</param>
        /// <returns></returns>
        public virtual string Round(object sourceExp, object? digitExp = null)
        {
            return $"ROUND({sourceExp}, {digitExp ?? 0})";
        }

        /// <summary>
        /// 返回源表达式的整数部份。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <returns></returns>
        public virtual string Truncate(object sourceExp)
        {
            return $"TRUNC({sourceExp})";
        }

        /// <summary>
        /// 返回源表达式的最大整数值。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <returns></returns>
        public virtual string Floor(object sourceExp)
        {
            return $"FLOOR({sourceExp})";
        }

        /// <summary>
        /// 返回以e为底的对数值。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <returns></returns>
        public virtual string Log(object sourceExp)
        {
            return $"ROUND(LN({sourceExp}), 9)";
        }

        /// <summary>
        /// 返回以10为底的对数值。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <returns></returns>
        public virtual string Log10(object sourceExp)
        {
            return $"ROUND(LOG(10, {sourceExp}), 9)";
        }

        /// <summary>
        /// 返回e的指定次冪。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <returns></returns>
        public virtual string Exp(object sourceExp)
        {
            return $"ROUND(EXP({sourceExp}), 9";
        }

        /// <summary>
        /// 返回源表达式的绝对值。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <returns></returns>
        public virtual string Abs(object sourceExp)
        {
            return $"ABS({sourceExp})";
        }

        /// <summary>
        /// 返回源表达式的反值。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <returns></returns>
        public virtual string Negate(object sourceExp)
        {
            return $"(0 - {sourceExp})";
        }

        /// <summary>
        /// 返回源表达式的指定冪。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <param name="powerExp">冪。</param>
        /// <returns></returns>
        public virtual string Power(object sourceExp, object powerExp)
        {
            return $"POWER({sourceExp}, {powerExp})";
        }

        /// <summary>
        /// 返回源表达式的二次开方值。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <returns></returns>
        public virtual string Sqrt(object sourceExp)
        {
            return $"ROUND(SQRT({sourceExp}), 6)";
        }

        /// <summary>
        /// 返回源表达式的正弦值。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <returns></returns>
        public virtual string Sin(object sourceExp)
        {
            return $"ROUND(SIN({sourceExp}), 9)";
        }

        /// <summary>
        /// 返回源表达式的余弦值。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <returns></returns>
        public virtual string Cos(object sourceExp)
        {
            return $"ROUND(COS({sourceExp}), 9)";
        }

        /// <summary>
        /// 返回源表达式的正切值。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <returns></returns>
        public virtual string Tan(object sourceExp)
        {
            return $"ROUND(TAN({sourceExp}), 9)";
        }

        /// <summary>
        /// 返回源表达式的反正弦值。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <returns></returns>
        public virtual string Asin(object sourceExp)
        {
            return $"ROUND(ASIN({sourceExp}), 9)";
        }

        /// <summary>
        /// 返回源表达式的反余弦值。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <returns></returns>
        public virtual string Acos(object sourceExp)
        {
            return $"ROUND(ACOS({sourceExp}), 9)";
        }

        /// <summary>
        /// 返回源表达式的反正切值。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <returns></returns>
        public virtual string Atan(object sourceExp)
        {
            return $"ROUND(ATAN({sourceExp}), 9)";
        }

        /// <summary>
        /// 返回源表达式的符号。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <returns></returns>
        public virtual string Sign(object sourceExp)
        {
            return $"ROUND(SIGN({sourceExp}), 9)";
        }

        /// <summary>
        /// 返回源表达式左移后的值。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <param name="shiftExp">位数。</param>
        /// <returns></returns>
        public virtual string LeftShift(object sourceExp, object shiftExp)
        {
            return $"({sourceExp} * POWER(2, {shiftExp}))";
        }

        /// <summary>
        /// 返回源表达式右移后的值。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <param name="shiftExp">位数。</param>
        /// <returns></returns>
        public virtual string RightShift(object sourceExp, object shiftExp)
        {
            return $"FLOOR({sourceExp} / POWER(2, {shiftExp}))";
        }

        /// <summary>
        /// 返回随机数生成函数。
        /// </summary>
        /// <returns></returns>
        public virtual string Random()
        {
            return "DBMS_RANDOM.RANDOM()";
        }
    }
}

