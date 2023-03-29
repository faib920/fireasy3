// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Fireasy.Common.Extensions
{
    /// <summary>
    /// 基本扩展方法。
    /// </summary>
    public static class GenericExtension
    {
        /// <summary>
        /// 将对象转换为指定的类型。
        /// </summary>
        /// <typeparam name="TValue">要转换的类型。</typeparam>
        /// <param name="value">源对象。</param>
        /// <returns></returns>
        public static TValue? To<TValue>(this object? value)
        {
            var newValue = To(value, typeof(TValue));

            if (newValue == null)
            {
                return default;
            }

            return (TValue)newValue;
        }

        /// <summary>
        /// 将对象转换为指定的类型。
        /// </summary>
        /// <param name="value">源对象。</param>
        /// <param name="convertType"></param>
        /// <returns></returns>
        public static object? To(this object? value, Type convertType)
        {
            if (value == null || value == DBNull.Value)
            {
                return default;
            }

            var underlyingType = convertType.GetNonNullableType();

            if (underlyingType == typeof(Guid))
            {
                if (Guid.TryParse(value.ToString(), out var guidValue))
                {
                    return guidValue;
                }

                return default;
            }
            //枚举
            if (underlyingType.IsEnum)
            {
                if (!Enum.IsDefined(underlyingType, Convert.ChangeType(value, Enum.GetUnderlyingType(underlyingType))))
                {
                    throw new InvalidCastException($"值无法从 {value!.GetType()} 转换为 {underlyingType}，因为 {underlyingType} 未定义枚举值 {value}。");
                }

                return Enum.Parse(underlyingType, value!.ToString());
            }

            try
            {
                return Convert.ChangeType(value, underlyingType);
            }
            catch (Exception exp)
            {
                throw new InvalidCastException($"值无法从 {value!.GetType()} 转换为 {underlyingType}。", exp);
            }
        }
    }
}
