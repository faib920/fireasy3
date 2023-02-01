// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using AutoMapper;
using AutoMapper.Internal;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Fireasy.AutoMapper
{
    /// <summary>
    /// 
    /// </summary>
    public class AutoMapperOptions
    {
        private readonly List<Type> _serviceTypes = new List<Type>();
        private readonly HashSet<Assembly> _existsAssemblies = new HashSet<Assembly>();

        /// <summary>
        /// 
        /// </summary>
        public List<Action<IMapperConfigurationExpression>> Configurators { get; } = new List<Action<IMapperConfigurationExpression>>();

        /// <summary>
        /// 添加配置文件。
        /// </summary>
        /// <typeparam name="TProfile"></typeparam>
        public AutoMapperOptions AddProfile<TProfile>() where TProfile : Profile, new()
        {
            Configurators!.Add(c => c.AddProfile<TProfile>());

            RegisterDependencyServices(typeof(TProfile).Assembly);

            return this;
        }

        /// <summary>
        /// 添加程序集里的 <see cref="Profile"/> 及定义了 <see cref="AutoMapAttribute"/> 特性的类。
        /// </summary>
        /// <param name="assemblies">要搜索的程序集数组，如果没有指定，则为当前调用的程序集。</param>
        public AutoMapperOptions AddProfiles(params Assembly[]? assemblies)
        {
            if (assemblies == null)
            {
                Configurators!.Add(c => c.AddMaps(Assembly.GetCallingAssembly()));
            }
            else
            {
                assemblies.ForAll(s => Configurators!.Add(c => c.AddMaps(s)));
            }

            RegisterDependencyServices(assemblies);

            return this;
        }

        internal void Setup(IServiceCollection services)
        {
            _serviceTypes.ForEach(s => services.AddTransient(s));
        }

        private void RegisterDependencyServices(params Assembly[]? assemblies)
        {
            if (assemblies?.Count() <= 0)
            {
                return;
            }

            var allTypes = assemblies.Where(a => !_existsAssemblies.Contains(a) && !a.IsDynamic && a.GetName().Name != "AutoMapper")
                .Distinct().SelectMany(a => a.DefinedTypes).ToArray();
            foreach (TypeInfo item in new Type[]
            {
                typeof(IValueResolver<,,>),
                typeof(IMemberValueResolver<,,,>),
                typeof(ITypeConverter<,>),
                typeof(IValueConverter<,>),
                typeof(IMappingAction<,>)
            }.SelectMany(openType => allTypes.Where(t => t.IsClass && !t.IsAbstract && ImplementsGenericInterface(t.AsType(), openType))))
            {
                _existsAssemblies.Add(item.Assembly);
                _serviceTypes.Add(item.AsType());
            }
        }

        private static bool ImplementsGenericInterface(Type type, Type interfaceType)
        {
            if (!IsGenericType(type, interfaceType))
            {
                return type.GetTypeInfo().ImplementedInterfaces.Any((Type @interface) => IsGenericType(@interface, interfaceType));
            }

            return true;
        }

        private static bool IsGenericType(Type type, Type genericType)
        {
            if (type.GetTypeInfo().IsGenericType)
            {
                return type.GetGenericTypeDefinition() == genericType;
            }

            return false;
        }
    }
}
