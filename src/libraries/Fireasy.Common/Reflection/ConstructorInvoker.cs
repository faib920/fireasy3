// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Fireasy.Common.Emit;
using System.Reflection;
using System.Reflection.Emit;

namespace Fireasy.Common.Reflection
{
    /// <summary>
    /// 包装 <see cref="ConstructorInfo"/> 对象，创建一个委托来提升构造函数的执行。
    /// </summary>
    public class ConstructorInvoker : IConstructorInvoker
    {
        private readonly Func<object[], object> _invoker;
        private ParameterInfo[] _parameters;

        /// <summary>
        /// 初始化 <see cref="ConstructorInvoker"/> 类的新实例。
        /// </summary>
        /// <param name="constructorInfo">要包装的 <see cref="ConstructorInfo"/> 对象。</param>
        public ConstructorInvoker(ConstructorInfo constructorInfo)
        {
            Guard.ArgumentNull(constructorInfo, nameof(constructorInfo));

            _invoker = GetConstructorDelegate(constructorInfo);
        }

        private Func<object[], object> GetConstructorDelegate(ConstructorInfo constructorInfo)
        {
            var dm = new DynamicMethod("CreateInstance", typeof(object),
                new Type[] { typeof(object[]) }, constructorInfo.DeclaringType, true);

            _parameters = constructorInfo.GetParameters();
            var emiter = new EmitHelper(dm.GetILGenerator());
            emiter
                .For(0, _parameters.Length, (e, i) => e.ldarg_0.ldc_i4(i).ldelem_ref.If(_parameters[i].ParameterType.IsValueType, f => f.unbox_any(_parameters[i].ParameterType), f => f.castclass(_parameters[i].ParameterType)))
                .newobj(constructorInfo)
                .ret();

            return (Func<object[], object>)dm.CreateDelegate(typeof(Func<object[], object>));
        }

        /// <summary>
        /// 使用指定的参数执行构造函数。
        /// </summary>
        /// <param name="parameters">构造函数的参数。</param>
        /// <returns></returns>
        public object Invoke(params object[] parameters)
        {
            if (parameters.Length != _parameters.Length)
            {
                throw new Exception($"参数个数不匹配，应为 {_parameters.Length} 个。");
            }

            return _invoker(parameters);
        }
    }

    /// <summary>
    /// 包装 <see cref="ConstructorInfo"/> 对象，创建一个委托来提升构造函数的执行。
    /// </summary>
    public class ConstructorInvoker<TArg1> : IConstructorInvoker<TArg1>, IConstructorInvoker
    {
        private readonly Func<TArg1, object> _invoker;
        private ParameterInfo[] _parameters;

        /// <summary>
        /// 初始化 <see cref="ConstructorInvoker"/> 类的新实例。
        /// </summary>
        /// <param name="constructorInfo">要包装的 <see cref="ConstructorInfo"/> 对象。</param>
        public ConstructorInvoker(ConstructorInfo constructorInfo)
        {
            Guard.ArgumentNull(constructorInfo, nameof(constructorInfo));

            _invoker = GetConstructorDelegate(constructorInfo);
        }

        private Func<TArg1, object> GetConstructorDelegate(ConstructorInfo constructorInfo)
        {
            var dm = new DynamicMethod("CreateInstance", typeof(object),
                new Type[] { typeof(TArg1) }, constructorInfo.DeclaringType, true);

            _parameters = constructorInfo.GetParameters();
            var emiter = new EmitHelper(dm.GetILGenerator());
            emiter
                .ldarg_0
                .newobj(constructorInfo)
                .ret();

            return (Func<TArg1, object>)dm.CreateDelegate(typeof(Func<TArg1, object>));
        }

        /// <summary>
        /// 使用指定的参数执行构造函数。
        /// </summary>
        /// <param name="arg1">参数1。</param>
        /// <returns></returns>
        public object Invoke(TArg1 arg1)
        {
            return _invoker(arg1);
        }

        object IConstructorInvoker.Invoke(params object[] parameters)
        {
            return Invoke((TArg1)parameters[0]);
        }
    }

    /// <summary>
    /// 包装 <see cref="ConstructorInfo"/> 对象，创建一个委托来提升构造函数的执行。
    /// </summary>
    public class ConstructorInvoker<TArg1, TArg2> : IConstructorInvoker<TArg1, TArg2>, IConstructorInvoker
    {
        private readonly Func<TArg1, TArg2, object> _invoker;
        private ParameterInfo[] _parameters;

        /// <summary>
        /// 初始化 <see cref="ConstructorInvoker"/> 类的新实例。
        /// </summary>
        /// <param name="constructorInfo">要包装的 <see cref="ConstructorInfo"/> 对象。</param>
        public ConstructorInvoker(ConstructorInfo constructorInfo)
        {
            Guard.ArgumentNull(constructorInfo, nameof(constructorInfo));

            _invoker = GetConstructorDelegate(constructorInfo);
        }

        private Func<TArg1, TArg2, object> GetConstructorDelegate(ConstructorInfo constructorInfo)
        {
            var dm = new DynamicMethod("CreateInstance", typeof(object),
                new Type[] { typeof(TArg1) }, constructorInfo.DeclaringType, true);

            _parameters = constructorInfo.GetParameters();
            var emiter = new EmitHelper(dm.GetILGenerator());
            emiter
                .ldarg_0
                .newobj(constructorInfo)
                .ret();

            return (Func<TArg1, TArg2, object>)dm.CreateDelegate(typeof(Func<TArg1, TArg2, object>));
        }

        /// <summary>
        /// 使用指定的参数执行构造函数。
        /// </summary>
        /// <param name="arg1">参数1。</param>
        /// <returns></returns>
        public object Invoke(TArg1 arg1, TArg2 arg2)
        {
            return _invoker(arg1, arg2);
        }

        object IConstructorInvoker.Invoke(params object[] parameters)
        {
            return Invoke((TArg1)parameters[0], (TArg2)parameters[1]);
        }
    }

    /// <summary>
    /// 包装 <see cref="ConstructorInfo"/> 对象，创建一个委托来提升构造函数的执行。
    /// </summary>
    public class ConstructorInvoker<TArg1, TArg2, TArg3> : IConstructorInvoker<TArg1, TArg2, TArg3>, IConstructorInvoker
    {
        private readonly Func<TArg1, TArg2, TArg3, object> _invoker;
        private ParameterInfo[] _parameters;

        /// <summary>
        /// 初始化 <see cref="ConstructorInvoker"/> 类的新实例。
        /// </summary>
        /// <param name="constructorInfo">要包装的 <see cref="ConstructorInfo"/> 对象。</param>
        public ConstructorInvoker(ConstructorInfo constructorInfo)
        {
            Guard.ArgumentNull(constructorInfo, nameof(constructorInfo));

            _invoker = GetConstructorDelegate(constructorInfo);
        }

        private Func<TArg1, TArg2, TArg3, object> GetConstructorDelegate(ConstructorInfo constructorInfo)
        {
            var dm = new DynamicMethod("CreateInstance", typeof(object),
                new Type[] { typeof(TArg1), typeof(TArg2), typeof(TArg3) }, constructorInfo.DeclaringType, true);

            _parameters = constructorInfo.GetParameters();
            var emiter = new EmitHelper(dm.GetILGenerator());
            emiter
                .ldarg_0.ldarg_1.ldarg_2
                .newobj(constructorInfo)
                .ret();

            return (Func< TArg1, TArg2, TArg3, object>)dm.CreateDelegate(typeof(Func<TArg1, TArg2, TArg3, object>));
        }

        /// <summary>
        /// 使用指定的参数执行构造函数。
        /// </summary>
        /// <param name="arg1">参数1。</param>
        /// <returns></returns>
        public object Invoke(TArg1 arg1, TArg2 arg2, TArg3 arg3)
        {
            return _invoker(arg1, arg2, arg3);
        }

        object IConstructorInvoker.Invoke(params object[] parameters)
        {
            return Invoke((TArg1)parameters[0], (TArg2)parameters[1], (TArg3)parameters[2]);
        }
    }
}
