// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Fireasy.Common.Serialization
{
    /// <summary>
    /// 提供对象 Json 序列化的方法。
    /// </summary>
    public interface IJsonSerializer
    {
        /// <summary>
        /// 将对象转换为使用 Json 文本表示。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">要序列化的对象。</param>
        /// <param name="options">序列化选项。</param>
        /// <returns>表示对象的文本。</returns>
        string Serialize<T>(T value, object? options = null);

        /// <summary>
        /// 从 Json 文本中解析出类型 <typeparamref name="T"/> 的对象。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json">表示对象的文本。</param>
        /// <param name="options">序列化选项。</param>
        /// <returns>解析后的对象。</returns>
        T? Deserialize<T>(string json, object? options = null);

        /// <summary>
        /// 从 Json 文本中解析出类型 <typeparamref name="T"/> 的对象，<typeparamref name="T"/> 可以是匿名类型。
        /// </summary>
        /// <typeparam name="T">自定义匿名类型。</typeparam>
        /// <param name="json">表示对象的文本</param>
        /// <param name="anyObj">为构造 <typeparamref name="T"/> 类型而初始化的对象。</param>
        /// <param name="options">序列化选项。</param>
        /// <returns>解析后的对象。</returns>
        T? Deserialize<T>(string json, T anyObj, object? options = null);

        /// <summary>
        /// 从 Json 文本中解析出类型 <paramref name="type"/> 的对象。
        /// </summary>
        /// <param name="json">表示对象的文本。</param>
        /// <param name="type">可序列化的对象类型。</param>
        /// <param name="options">序列化选项。</param>
        /// <returns>对象。</returns>
        object? Deserialize(string json, Type type, object? options = null);

        /// <summary>
        /// 从 Json 文本中解析出动态类型对象。
        /// </summary>
        /// <param name="json">表示对象的文本。</param>
        /// <param name="options">序列化选项。</param>
        /// <returns>对象。</returns>
        dynamic? Deserialize(string json, object? options = null);
    }
}
