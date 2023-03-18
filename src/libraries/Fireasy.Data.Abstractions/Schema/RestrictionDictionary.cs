// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Fireasy.Data.Schema
{
    /// <summary>
    /// 用于存储限制值的字典。无法继承此类。
    /// </summary>
    public sealed class RestrictionDictionary : Dictionary<string, object?>
    {
        public static RestrictionDictionary Empty = new RestrictionDictionary();

        /// <summary>
        /// 使用限制值参数化 <see cref="ParameterCollection"/> 对象。
        /// </summary>
        /// <param name="parameters"><see cref="ParameterCollection"/> 对象。</param>
        /// <param name="parameterName">参数名称。</param>
        /// <param name="propertyName">限制值的名称。</param>
        /// <param name="dbType">数据类型。</param>
        /// <param name="addNullValue">当限制不存在时，添加空值的参数。</param>
        /// <returns></returns>
        public RestrictionDictionary Parameterize(ParameterCollection parameters, string parameterName, string propertyName, DbType dbType = DbType.String, bool addNullValue = true)
        {
            if (!addNullValue && ContainsKey(propertyName))
            {
                parameters.Add(parameterName, string.Empty, this[propertyName], dbType, null, ParameterDirection.Input);
            }
            else if (addNullValue)
            {
                parameters.Add(parameterName, string.Empty, ContainsKey(propertyName) ? this[propertyName] : DBNull.Value, dbType, null, ParameterDirection.Input);
            }

            return this;
        }

        /// <summary>
        /// 尝试从字典里拿到不为 null 的字符串值。
        /// </summary>
        /// <param name="name">字段名称。</param>
        /// <param name="value">对应的值。</param>
        /// <returns></returns>
        public bool TryGetValue(string name, out string? value)
        {
            if (TryGetValue(name, out var obj))
            {
                if (obj != null)
                {
                    value = obj.ToString();
                    return true;
                }
            }

            value = null;
            return false;
        }

        /// <summary>
        /// 获取对应的值。
        /// </summary>
        /// <param name="name">字段名称。</param>
        /// <returns></returns>
        public string? GetValue(string name)
        {
            if (TryGetValue(name, out var obj))
            {
                if (obj != null)
                {
                    return obj.ToString();
                }
            }

            return null;
        }
    }
}
