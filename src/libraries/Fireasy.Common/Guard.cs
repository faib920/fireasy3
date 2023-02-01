// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Fireasy.Common
{
    /// <summary>
    /// 提供对程序的异常检查。
    /// </summary>
    public static class Guard
    {
        /// <summary>
        /// 检查对象是否为 null。
        /// </summary>
        /// <param name="obj">要检查的对象。</param>
        /// <param name="message">异常的提示信息。</param>
        /// <exception cref="ArgumentException">对象为 null。</exception>
        public static void NullReference(object? obj, string? message = null)
        {
            if (obj == null)
            {
                throw new ArgumentException(message ?? "所访问的对象为空。");
            }
        }

        /// <summary>
        /// 检查参数是否为 null。
        /// </summary>
        /// <param name="obj">要检查的参数对象。</param>
        /// <param name="paramName">参数的名称。</param>
        /// <param name="message">异常的提示信息。</param>
        /// <exception cref="ArgumentNullException">参数为 null。</exception>
        public static void ArgumentNull(object? obj, string? paramName, string? message = null)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(paramName, message ?? $"参数 {paramName} 为空。");
            }
        }

        /// <summary>
        /// 检查参数是否无效。
        /// </summary>
        /// <param name="predicate">测试的函数。</param>
        /// <param name="paramName">参数的名称。</param>
        /// <param name="message">异常的提示信息。</param>
        /// <exception cref="ArgumentException">函数未成功。</exception>
        public static void Argument(bool predicate, string? paramName, string? message = null)
        {
            if (!predicate)
            {
                throw new ArgumentException(paramName, message ?? $"参数 {paramName} 无效。");
            }
        }

        /// <summary>
        /// 检查给定的索引值是否超出范围。
        /// </summary>
        /// <param name="range">给定的范围。</param>
        /// <param name="index">要检查的索引值。</param>
        /// <param name="message">异常的提示信息。</param>
        /// <exception cref="ArgumentOutOfRangeException">参数无效。</exception>
        public static void OutOfRange(int range, int index, string? message = null)
        {
            if (index < 0 || (range > int.MinValue && index > range - 1))
            {
                throw new ArgumentOutOfRangeException(message ?? "索引值超出了给定的范围。");
            }
        }

        /// <summary>
        /// 检查条件，如果为 false 则引发指定类型的异常。
        /// </summary>
        /// <param name="condition">条件是否成立。</param>
        /// <param name="exception">抛出的异常对象。</param>
        public static void Assert(bool condition, Exception exception)
        {
            if (!condition && exception != null)
            {
                throw exception;
            }
        }
    }
}
