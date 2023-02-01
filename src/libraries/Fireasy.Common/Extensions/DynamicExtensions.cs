// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using System.Dynamic;
using System.Linq.Expressions;

namespace Fireasy.Common.Extensions
{
    /// <summary>
    /// 动态类型的扩展。
    /// </summary>
    public static class DynamicExtensions
    {
        /// <summary>
        /// 获取动态对象中成员名称的枚举。
        /// </summary>
        /// <param name="dynamicProvider">一个动态对象。</param>
        /// <returns></returns>
        public static IEnumerable<string> GetDynamicMemberNames(this IDynamicMetaObjectProvider dynamicProvider)
        {
            var metaObject = dynamicProvider.GetMetaObject(Expression.Constant(dynamicProvider));
            return metaObject.GetDynamicMemberNames();
        }
    }
}
