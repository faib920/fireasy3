// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Fireasy.Common.Dynamic;
using System.Text.Json;

namespace Fireasy.Common.Serialization
{
    /// <summary>
    /// 缺省的 Json 序列化器。
    /// </summary>
    public class DefaultJsonSerializer : IJsonSerializer
    {
        /// <summary>
        /// 将对象转换为使用 Json 文本表示。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">要序列化的对象。</param>
        /// <param name="options">序列化选项。</param>
        /// <returns>表示对象的文本。</returns>
        public string Serialize<T>(T value, object? options = null)
        {
            return JsonSerializer.Serialize(value, options as JsonSerializerOptions);
        }

        /// <summary>
        /// 从 Json 文本中解析出类型 <typeparamref name="T"/> 的对象。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json">表示对象的文本。</param>
        /// <param name="options">序列化选项。</param>
        /// <returns>解析后的对象。</returns>
        public T? Deserialize<T>(string json, object? options = null)
        {
            var obj = JsonSerializer.Deserialize(json, typeof(T), options as JsonSerializerOptions);
            if (obj is T t)
            {
                return t;
            }

            return default;
        }

        /// <summary>
        /// 从 Json 文本中解析出类型 <typeparamref name="T"/> 的对象，<typeparamref name="T"/> 可以是匿名类型。
        /// </summary>
        /// <typeparam name="T">自定义匿名类型。</typeparam>
        /// <param name="json">表示对象的文本</param>
        /// <param name="anyObj">为构造 <typeparamref name="T"/> 类型而初始化的对象。</param>
        /// <param name="options">序列化选项。</param>
        /// <returns>解析后的对象。</returns>
        public T? Deserialize<T>(string json, T anyObj, object? options = null)
        {
            Guard.ArgumentNull(anyObj, nameof(anyObj));

            var obj = JsonSerializer.Deserialize(json, anyObj!.GetType(), options as JsonSerializerOptions);
            if (obj is T t)
            {
                return t;
            }

            return default;
        }

        /// <summary>
        /// 从 Json 文本中解析出类型 <paramref name="type"/> 的对象。
        /// </summary>
        /// <param name="json">表示对象的文本。</param>
        /// <param name="type">可序列化的对象类型。</param>
        /// <param name="options">序列化选项。</param>
        /// <returns>对象。</returns>
        public object? Deserialize(string json, Type type, object? options = null)
        {
            var obj = JsonSerializer.Deserialize(json, type, options as JsonSerializerOptions);
            return obj;
        }

        /// <summary>
        /// 从 Json 文本中解析出动态类型对象。
        /// </summary>
        /// <param name="json">表示对象的文本。</param>
        /// <param name="options">序列化选项。</param>
        /// <returns>对象。</returns>
        public dynamic? Deserialize(string json, object? options = null)
        {
            var element = JsonSerializer.Deserialize<JsonElement>(json, options as JsonSerializerOptions);
            return ParseElement(element);
        }

        private object? ParseElement(JsonElement element)
        {
            if (element.ValueKind== JsonValueKind.Array)
            {
                var array = new List<object?>();

                foreach (var item in element.EnumerateArray())
                {
                    array.Add(ParseElement(item));
                }

                return array;
            }
            else if (element.ValueKind == JsonValueKind.Object)
            {
                var dict = (IDictionary<string, object?>)new DynamicExpandoObject();
                foreach (var property in element.EnumerateObject())
                {
                    dict.Add(property.Name, ParseElement(property.Value));
                }

                return dict;
            }
            else if (element.ValueKind == JsonValueKind.String)
            {
                return element.GetString();
            }
            else if (element.ValueKind == JsonValueKind.Number)
            {
                if (element.TryGetInt32(out var i))
                {
                    return i;
                }
                if (element.TryGetInt64(out var l))
                {
                    return l;
                }
                if (element.TryGetDecimal(out var d))
                {
                    return d;
                }
                if (element.TryGetSingle(out var f))
                {
                    return f;
                }
                if (element.TryGetDouble(out var b))
                {
                    return b;
                }
            }
            else if (element.ValueKind == JsonValueKind.True)
            {
                return true;
            }
            else if (element.ValueKind == JsonValueKind.False)
            {
                return false;
            }

            return null;
        }
    }
}
