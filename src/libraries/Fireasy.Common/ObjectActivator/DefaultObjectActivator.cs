// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Fireasy.Common.DynamicProxy;
using Fireasy.Common.Extensions;
using Fireasy.Common.Reflection;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Fireasy.Common.ObjectActivator
{
    /// <summary>
    /// 缺省的对象创建器。
    /// </summary>
    public class DefaultObjectActivator : IObjectActivator
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IReflectionFactory? _reflectionFactory;
        private readonly IDynamicProxyFactory? _dynamicProxyFactory;

        /// <summary>
        /// 初始化 <see cref="DefaultObjectActivator"/> 类的新实例。
        /// </summary>
        /// <param name="serviceProvider"></param>
        public DefaultObjectActivator(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _reflectionFactory = _serviceProvider.GetRequiredService<IReflectionFactory>();
            _dynamicProxyFactory = _serviceProvider.GetService<IDynamicProxyFactory>();
        }

        /// <summary>
        /// 创建类型 <typeparamref name="T"/> 的实例。
        /// </summary>
        /// <typeparam name="T">对象的类型。</typeparam>
        /// <param name="args">构造器参数，如果缺省则使用 <see cref="IServiceProvider.GetService(Type)"/> 来匹配构造函数的参数。</param>
        /// <returns></returns>
        public T? CreateInstance<T>(params object[] args)
        {
            if (CreateInstance(typeof(T), args) is T obj)
            {
                return obj;
            }

            return default;
        }

        /// <summary>
        /// 创建类型 <paramref name="type"/> 的实例。
        /// </summary>
        /// <param name="type">对象的类型。</param>
        /// <param name="args">构造器参数，如果缺省则使用 <see cref="IServiceProvider.GetService(Type)"/> 来匹配构造函数的参数。</param>
        /// <returns></returns>
        public object? CreateInstance(Type type, params object[] args)
        {
            Guard.ArgumentNull(type, nameof(type));

            if (_dynamicProxyFactory != null && type.IsDynamicProxySupportNotImplemented())
            {
                type = _dynamicProxyFactory.GetProxyType(type);
            }

            if (args?.Length == 0)
            {
                return CreateUseServiceProvider(type);
            }

            var constructor = type.GetTypeInfo().DeclaredConstructors.FirstOrDefault(s => s.GetParameters().Length == args!.Length);
            if (constructor == null)
            {
                throw new InvalidOperationException("未找到匹配的构造函数。");
            }

            var accessor = _reflectionFactory!.GetInvoker(constructor);
            return accessor?.Invoke(args!);
        }

        private object? CreateUseServiceProvider(Type type)
        {
            var constructors = from s in type.GetTypeInfo().DeclaredConstructors
                               where !s.IsStatic && s.IsPublic
                               let pars = s.GetParameters()
                               orderby pars.Length descending
                               select new { info = s, pars };

            var exceptions = new List<Exception>();

            foreach (var cons in constructors)
            {
                var length = cons.pars.Length;
                var match = 0;
                var arguments = new object[length];
                for (var i = 0; i < length; i++)
                {
                    var parType = cons.pars[i].ParameterType;
                    if (parType == typeof(IServiceProvider))
                    {
                        arguments[i] = _serviceProvider;
                        match++;
                    }
                    else
                    {
                        var svrArg = _serviceProvider.TryGetService(parType);
                        if (svrArg != null)
                        {
                            arguments[i] = svrArg;
                            match++;
                        }
                        else
                        {
                            exceptions.Add(new ArgumentNullException());
                        }
                    }
                }

                if (match == length)
                {
                    var accessor = _reflectionFactory!.GetInvoker(cons.info);
                    return accessor?.Invoke(arguments).TrySetServiceProvider(_serviceProvider);
                }
            }

            if (exceptions.Count > 0)
            {
                throw new AggregateException("创建失败", exceptions);
            }

            return null;
        }
    }
}
