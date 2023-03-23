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
    /// OleDb驱动数学函数语法解析。
    /// </summary>
    public class OleDbMathSyntax : IMathSyntax
    {
        /// <summary>
        /// 两个表达式进行与运算。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <param name="otherExp">参与运算的表达式。</param>
        /// <returns></returns>
        public string BitAnd(object sourceExp, object otherExp)
        {
            return $"{sourceExp} BAND {otherExp}";
        }

        /// <summary>
        /// 两个表达式进行或运算。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <param name="otherExp">参与运算的表达式。</param>
        /// <returns></returns>
        public string BitOr(object sourceExp, object otherExp)
        {
            return $"{sourceExp} BOR {otherExp}";
        }

        /// <summary>
        /// 返回源表达式的非值。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <returns></returns>
        public string BitNot(object sourceExp)
        {
            return $"NOT {sourceExp}";
        }

        /// <summary>
        /// 对源表达式进行求余运算。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <param name="otherExp">参与运算的表达式。</param>
        /// <returns></returns>
        public string Modulo(object sourceExp, object otherExp)
        {
            return $"({sourceExp} MOD {otherExp})";
        }

        /// <summary>
        /// 对两个表达式进行异或运算。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <param name="otherExp">参与运算的表达式。</param>
        /// <returns></returns>
        public string ExclusiveOr(object sourceExp, object otherExp)
        {
            return $"{sourceExp} XOR {otherExp}";
        }

        /// <summary>
        /// 返回源表达式的最小整数值。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <returns></returns>
        public string Ceiling(object sourceExp)
        {
            return $"CEILING({sourceExp})";
        }

        /// <summary>
        /// 将源表达式的小数位舍入。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <param name="digitExp">小数位数。</param>
        /// <returns></returns>
        public string Round(object sourceExp, object digitExp = null)
        {
            return $"ROUND({sourceExp}, {digitExp ?? 0})";
        }

        /// <summary>
        /// 返回源表达式的整数部份。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <returns></returns>
        public string Truncate(object sourceExp)
        {
            return $"FIX({sourceExp})";
        }

        /// <summary>
        /// 返回源表达式的最大整数值。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <returns></returns>
        public string Floor(object sourceExp)
        {
            throw new SyntaxNotSupportedException(nameof(Floor));
        }

        /// <summary>
        /// 返回以e为底的对数值。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <returns></returns>
        public string Log(object sourceExp)
        {
            return $"LOG({sourceExp})";
        }

        /// <summary>
        /// 返回以10为底的对数值。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <returns></returns>
        public string Log10(object sourceExp)
        {
            return $"LOG10({sourceExp})";
        }

        /// <summary>
        /// 返回e的指定次冪。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <returns></returns>
        public string Exp(object sourceExp)
        {
            return $"EXP({sourceExp})";
        }

        /// <summary>
        /// 返回源表达式的绝对值。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <returns></returns>
        public string Abs(object sourceExp)
        {
            return $"ABS({sourceExp})";
        }

        /// <summary>
        /// 返回源表达式的反值。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <returns></returns>
        public string Negate(object sourceExp)
        {
            return $"-{sourceExp}";
        }

        /// <summary>
        /// 返回源表达式的指定冪。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <param name="powerExp">冪。</param>
        /// <returns></returns>
        public string Power(object sourceExp, object powerExp)
        {
            return $"{sourceExp} ^ {powerExp}";
        }

        /// <summary>
        /// 返回源表达式的二次开方值。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <returns></returns>
        public string Sqrt(object sourceExp)
        {
            return $"SQR({sourceExp})";
        }

        /// <summary>
        /// 返回源表达式的正弦值。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <returns></returns>
        public string Sin(object sourceExp)
        {
            return $"SIN({sourceExp})";
        }

        /// <summary>
        /// 返回源表达式的余弦值。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <returns></returns>
        public string Cos(object sourceExp)
        {
            return $"COS({sourceExp})";
        }

        /// <summary>
        /// 返回源表达式的正切值。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <returns></returns>
        public string Tan(object sourceExp)
        {
            return $"TAN({sourceExp})";
        }

        /// <summary>
        /// 返回源表达式的反正弦值。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <returns></returns>
        public string Asin(object sourceExp)
        {
            return $"ASIN({sourceExp})";
        }

        /// <summary>
        /// 返回源表达式的反余弦值。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <returns></returns>
        public string Acos(object sourceExp)
        {
            return $"ACOS({sourceExp})";
        }

        /// <summary>
        /// 返回源表达式的反正切值。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <returns></returns>
        public string Atan(object sourceExp)
        {
            return $"ATN({sourceExp})";
        }

        /// <summary>
        /// 返回源表达式的符号。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <returns></returns>
        public string Sign(object sourceExp)
        {
            return $"SIGN({sourceExp})";
        }

        /// <summary>
        /// 返回源表达式左移后的值。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <param name="shiftExp">位数。</param>
        /// <returns></returns>
        public string LeftShift(object sourceExp, object shiftExp)
        {
            return $"{sourceExp} * (2 ^ {shiftExp})";
        }

        /// <summary>
        /// 返回源表达式右移后的值。
        /// </summary>
        /// <param name="sourceExp">源表达式。</param>
        /// <param name="shiftExp">位数。</param>
        /// <returns></returns>
        public string RightShift(object sourceExp, object shiftExp)
        {
            return $"{sourceExp} / (2 ^ {shiftExp})";
        }

        /// <summary>
        /// 返回随机数生成函数。
        /// </summary>
        /// <returns></returns>
        public string Random()
        {
            return "RND()";
        }
    }
}
