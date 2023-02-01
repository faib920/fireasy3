// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Fireasy.Common.Extensions;
using Fireasy.Common.Reflection;
using System.Dynamic;
using System.Runtime.CompilerServices;

namespace Fireasy.Common.Dynamic
{
    /// <summary>
    /// 缺省的动态扩展性。
    /// </summary>
    public sealed class DefaultDynamicExpansibility : IDynamicExpansibility
    {
        private readonly Dictionary<string, CallSite<Func<CallSite, object, object>>> _getCallSites = new Dictionary<string, CallSite<Func<CallSite, object, object>>>();
        private readonly Dictionary<string, CallSite<Func<CallSite, object, object, object>>> _setCallSites = new Dictionary<string, CallSite<Func<CallSite, object, object, object>>>();
        private readonly object _errorResult = new object();
        private readonly BinderWrapper _binderWrapper;

        /// <summary>
        /// 初始化 <see cref="DefaultDynamicExpansibility"/> 类的新实例。
        /// </summary>
        /// <param name="reflectionFactory"></param>
        public DefaultDynamicExpansibility(IReflectionFactory reflectionFactory)
        {
            _binderWrapper = new BinderWrapper(reflectionFactory);
        }

        /// <summary>
        /// 获取动态对象中指定名称的属性值。
        /// </summary>
        /// <param name="dynamicProvider">一个动态对象。</param>
        /// <param name="name">属性的名称。</param>
        /// <returns></returns>
        public object? GetMember(IDynamicMetaObjectProvider dynamicProvider, string name)
        {
            if (TryGetMember(dynamicProvider, name, out var value))
            {
                return value;
            }

            return null;
        }

        /// <summary>
        /// 设置动态对象中指定名称的属性值。
        /// </summary>
        /// <param name="dynamicProvider">一个动态对象。</param>
        /// <param name="name">属性的名称。</param>
        /// <param name="value"></param>
        /// <returns></returns>
        public void SetMember(IDynamicMetaObjectProvider dynamicProvider, string name, object value)
        {
            TrySetMember(dynamicProvider, name, value);
        }

        /// <summary>
        /// 尝试获取动态对象中指定名称的属性值。
        /// </summary>
        /// <param name="dynamicProvider">一个动态对象。</param>
        /// <param name="name">属性的名称。</param>
        /// <param name="value">返回值。</param>
        /// <returns></returns>
        public bool TryGetMember(IDynamicMetaObjectProvider dynamicProvider, string name, out object? value)
        {
            if (dynamicProvider is IDictionary<string, object> dict)
            {
                return dict.TryGetValue(name, out value);
            }

            if (!_getCallSites.TryGetValue(name, out var callSite))
            {
                callSite = CallSite<Func<CallSite, object, object>>.Create(new NoThrowGetBinderMember((GetMemberBinder)_binderWrapper.GetMember(name)));
            }

            var result = callSite.Target(callSite, dynamicProvider);

            if (!ReferenceEquals(result, _errorResult))
            {
                value = result;
                return true;
            }
            else
            {
                value = null;
                return false;
            }
        }

        /// <summary>
        /// 尝试设置动态对象中指定名称的属性值。
        /// </summary>
        /// <param name="dynamicProvider">一个动态对象。</param>
        /// <param name="name">属性的名称。</param>
        /// <param name="value">设置值。</param>
        /// <returns></returns>
        public bool TrySetMember(IDynamicMetaObjectProvider dynamicProvider, string name, object value)
        {
            if (dynamicProvider is IDictionary<string, object> dict)
            {
                dict.AddOrReplace(name, value);
                return true;
            }

            if (!_setCallSites.TryGetValue(name, out var callSite))
            {
                callSite = CallSite<Func<CallSite, object, object, object>>.Create(new NoThrowSetBinderMember((SetMemberBinder)_binderWrapper.SetMember(name)));
            }

            var result = callSite.Target(callSite, dynamicProvider, value);

            return !ReferenceEquals(result, _errorResult);
        }
    }
}
