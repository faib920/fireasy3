// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Fireasy.Common.Collections;

namespace Fireasy.Common.DynamicProxy
{
    public static class Container
    {
        private static ExtraConcurrentDictionary<Type, Type> _types = new();

        public static bool TryAdd(Type type, Type proxyType)
        {
            return _types.TryAdd(type, proxyType);
        }

        public static bool TryGet(Type type, out Type proxyType)
        {
            return _types.TryGetValue(type, out proxyType);
        }
    }
}
