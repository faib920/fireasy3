// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Fireasy.Common.Extensions;
using System.Reflection;

namespace Fireasy.Common.DynamicProxy
{
    /// <summary>
    /// 辅助类。
    /// </summary>
    public static class Utils
    {
        /// <summary>
        /// 获取所有拦截特性。
        /// </summary>
        /// <param name="callInfo"></param>
        /// <returns></returns>
        public static InterceptAttribute[] GetInterceptAttributes(InterceptCallInfo callInfo)
        {
            return callInfo.Member.GetCustomAttributes<InterceptAttribute>(true)
                .Union(callInfo.DefinedType.GetCustomAttributes<InterceptAttribute>(true))
                .ToArray();
        }

        /// <summary>
        /// 判断类型是否支持动态代理但是未实现动态代理。
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsDynamicProxySupportNotImplemented(this Type type)
        {
            return typeof(IDynamicProxySupport).IsAssignableFrom(type) && !typeof(IDynamicProxyImplemented).IsAssignableFrom(type);
        }
    }
}
