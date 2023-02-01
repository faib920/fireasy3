// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Fireasy.Common.DependencyInjection
{
    /// <summary>
    /// 缺省的对象访问器。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DefaultObjectAccessor<T> : IObjectAccessor<T>
    {
        /// <summary>
        /// 初始化 <see cref="DefaultObjectAccessor{TValue}"/> 类的新实例。
        /// </summary>
        /// <param name="obj"></param>
        public DefaultObjectAccessor(T? obj)
        {
            Value = obj;
        }

        /// <summary>
        /// 获取实例。
        /// </summary>
        public T? Value { get; set; }
    }
}
