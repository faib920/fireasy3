// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using System.Text;

namespace Fireasy.Data.Converter
{
    /// <summary>
    /// 数组转换器。
    /// </summary>
    public class ArrayConverter : IValueConverter
    {
        private readonly Type _elementType;

        /// <summary>
        /// 初始化 <see cref="ArrayConverter"/> 类的新实例。
        /// </summary>
        /// <param name="elementType"></param>
        public ArrayConverter(Type elementType)
        {
            _elementType = elementType;
        }

        /// <summary>
        /// 将存储的数据转换为指定的类型。
        /// </summary>
        /// <param name="value">要转换的值。</param>
        /// <param name="dbType">数据列类型。</param>
        /// <returns>一个数组。</returns>
        public object? ConvertFrom(object value, DbType dbType = DbType.String)
        {
            if (value == null || value == DBNull.Value)
            {
                return null;
            }

            if (dbType.IsStringDbType())
            {
                if (_elementType == typeof(byte))
                {
                    try
                    {
                        return Convert.FromBase64String(value.ToString());
                    }
                    catch
                    {
                        return null;
                    }
                }
                else
                {
                    var array = value.ToString().Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(s => Convert.ChangeType(s, _elementType)).ToArray();
                    var result = Array.CreateInstance(_elementType, array.Length);
                    Array.Copy(array, result, array.Length);

                    return result;
                }
            }

            return value;
        }

        /// <summary>
        /// 将特殊对象转换为可存储到数据库的类型。
        /// </summary>
        /// <param name="value">要存储的数组。</param>
        /// <param name="dbType">数据列类型。</param>
        /// <returns></returns>
        public object? ConvertTo(object value, DbType dbType = DbType.String)
        {
            if (dbType.IsStringDbType() && value != null)
            {
                if (_elementType == typeof(byte))
                {
                    return Convert.ToBase64String((byte[])value);
                }
                else
                {
                    return BuildString((Array)value);
                }
            }

            return value;
        }

        private string BuildString(Array array)
        {
            var sb = new StringBuilder();
            foreach (var a in array)
            {
                if (sb.Length > 0)
                {
                    sb.Append(",");
                }

                sb.Append(a);
            }

            return sb.ToString();
        }
    }
}
