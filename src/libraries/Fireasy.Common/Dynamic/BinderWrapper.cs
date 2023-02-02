// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Fireasy.Common.Reflection;
using Microsoft.CSharp.RuntimeBinder;
using System.Runtime.CompilerServices;

namespace Fireasy.Common.Dynamic
{
    internal class BinderWrapper
    {
        private readonly IReflectionFactory? _reflectionFactory;
        private readonly object _getCSharpArgumentInfoArray;
        private readonly object _setCSharpArgumentInfoArray;
        private IMethodInvoker _getMemberCall;
        private IMethodInvoker _setMemberCall;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reflectionFactory"></param>
        public BinderWrapper(IReflectionFactory reflectionFactory)
        {
            _reflectionFactory = reflectionFactory;
            _getCSharpArgumentInfoArray = CreateSharpArgumentInfoArray(0);
            _setCSharpArgumentInfoArray = CreateSharpArgumentInfoArray(0, 3);

            CreateMemberCalls();
        }

        private static object CreateSharpArgumentInfoArray(params int[] values)
        {
            var a = Array.CreateInstance(typeof(CSharpArgumentInfo), values.Length);

            for (var i = 0; i < values.Length; i++)
            {
                var createArgumentInfoMethod = typeof(CSharpArgumentInfo).GetMethod(nameof(CSharpArgumentInfo.Create), new[] { typeof(CSharpArgumentInfoFlags), typeof(string) });
                var arg = createArgumentInfoMethod.Invoke(null, new object[] { 0, null });
                a.SetValue(arg, i);
            }

            return a;
        }

        private void CreateMemberCalls()
        {
            var csharpArgumentInfoTypeEnumerableType = typeof(IEnumerable<>).MakeGenericType(typeof(CSharpArgumentInfo));

            var getMemberMethod = typeof(Binder).GetMethod(nameof(Binder.GetMember), new[] { typeof(CSharpBinderFlags), typeof(string), typeof(Type), csharpArgumentInfoTypeEnumerableType });
            _getMemberCall = _reflectionFactory!.GetInvoker(getMemberMethod);

            var setMemberMethod = typeof(Binder).GetMethod(nameof(Binder.SetMember), new[] { typeof(CSharpBinderFlags), typeof(string), typeof(Type), csharpArgumentInfoTypeEnumerableType });
            _setMemberCall = _reflectionFactory!.GetInvoker(setMemberMethod);
        }

        public CallSiteBinder GetMember(string name)
        {
            return (CallSiteBinder)_getMemberCall.Invoke(null, 0, name, typeof(BinderWrapper), _getCSharpArgumentInfoArray);
        }

        public CallSiteBinder SetMember(string name)
        {
            return (CallSiteBinder)_setMemberCall.Invoke(null, 0, name, typeof(BinderWrapper), _setCSharpArgumentInfoArray);
        }
    }
}
