// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Fireasy.Data.Converter
{
    public interface IConvertManager
    {
        /// <summary>
        /// 根据对象类型创建相应的转换器。
        /// </summary>
        /// <param name="conversionType">要转换的数据类型。</param>
        /// <returns>返回一个 <see cref="IValueConverter"/> 实例，如果未找到对应的转换器，则返回 null。</returns>
        IValueConverter GetConverter(Type conversionType);

        // <summary>
        /// 判断指定的类型是否支持转换。
        /// </summary>
        /// <param name="conversionType"></param>
        /// <returns></returns>
        bool CanConvert(Type conversionType);
    }
}
