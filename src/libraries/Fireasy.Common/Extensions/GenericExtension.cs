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
        /// <param name="source">源对象。</param>
        /// <returns></returns>
        public static TValue? To<TValue>(this object? source)
        {
            if (source == null || source == DBNull.Value)
            {
                return default;
            }

            var underlyingType = typeof(TValue).GetNonNullableType();

            if (underlyingType == typeof(Guid))
            {
                if (Guid.TryParse(source.ToString(), out var guidValue))
                {
                    return (TValue)(object)guidValue;
                }

                return default;
            }
            //枚举
            if (underlyingType.IsEnum)
            {
                if (!Enum.IsDefined(underlyingType, Convert.ChangeType(source, Enum.GetUnderlyingType(underlyingType))))
                {
                    throw new InvalidCastException($"值无法从 {source!.GetType()} 转换为 {underlyingType}，因为 {underlyingType} 未定义枚举值 {source}。");
                }

                return (TValue)Enum.Parse(underlyingType, source!.ToString());
            }

            try
            {
                return (TValue)Convert.ChangeType(source, underlyingType);
            }
            catch (Exception exp)
            {
                throw new InvalidCastException($"值无法从 {source!.GetType()} 转换为 {underlyingType}。", exp);
            }
        }
    }
}
