// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Fireasy.Common.Extensions;
using System.Data.Common;
using System.Reflection;

namespace Fireasy.Data.Provider
{
    /// <summary>
    /// <see cref="IProvider"/> 定制器。
    /// </summary>
    public class ProviderCustomizer
    {
        private readonly Dictionary<Type, DbProviderFactory?> _providerFactoryMappers = new();
        private readonly Dictionary<string, Type> _providerMappers = new();
        private readonly Dictionary<Type, List<(Type DefinedType, Type ServiceType)>> _providerServiceTypes = new();

        /// <summary>
        /// 添加数据库提供者。
        /// </summary>
        /// <typeparam name="TProvider"></typeparam>
        /// <param name="providerName"></param>
        /// <returns></returns>
        public void AddProvider<TProvider>(string providerName) where TProvider : IProvider
        {
            _providerMappers.AddOrReplace(providerName, typeof(TProvider));
        }

        /// <summary>
        /// 获取数据库提供者。
        /// </summary>
        /// <param name="providerName"></param>
        /// <returns></returns>
        public Type? GetDbProviderType(string providerName)
        {
            if (_providerMappers.TryGetValue(providerName, out var providerType))
            {
                return providerType;
            }

            return null;
        }

        /// <summary>
        /// 遍列所有数据库提供者的映射。
        /// </summary>
        /// <param name="action">遍列的方法。</param>
        public void EachDbProviderTypes(Action<string, Type> action)
        {
            foreach (var kvp in _providerMappers)
            {
                action?.Invoke(kvp.Key, kvp.Value);
            }
        }

        /// <summary>
        /// 指定数据库提供者所使用的 <see cref="DbProviderFactory"/> 类名。
        /// </summary>
        /// <typeparam name="TProvider"></typeparam>
        /// <typeparam name="TFactory"><see cref="DbProviderFactory"/> 的实现类。</typeparam>
        /// <returns></returns>
        public void AddProivderFactory<TProvider, TFactory>()
            where TProvider : IProvider
            where TFactory : DbProviderFactory
        {
            var field = typeof(TFactory).GetTypeInfo().GetFields(BindingFlags.Public | BindingFlags.Static)
                .FirstOrDefault(s => typeof(DbProviderFactory).IsAssignableFrom(s.FieldType));

            DbProviderFactory? factory;
            if (field != null)
            {
                factory = field.GetValue(null) as DbProviderFactory;
            }
            else
            {
                factory = Activator.CreateInstance(typeof(TFactory)) as DbProviderFactory;
            }

            if (factory != null)
            {
                _providerFactoryMappers.AddOrReplace(typeof(TProvider), factory);
            }
        }

        /// <summary>
        /// 指定数据库提供者所使用的 <see cref="DbProviderFactory"/> 类名。
        /// </summary>
        /// <typeparam name="TProvider"></typeparam>
        /// <returns></returns>
        public void AddProivderFactory<TProvider>(DbProviderFactory factory)
            where TProvider : IProvider
        {
            _providerFactoryMappers.AddOrReplace(typeof(TProvider), factory);
        }

        /// <summary>
        /// 获取指定 <see cref="IProvider"/> 的 <see cref="DbProviderFactory"/> 实例。
        /// </summary>
        /// <param name="providerType">数据库提供者类型。</param>
        /// <returns></returns>
        public DbProviderFactory? GetDbProviderFactory(Type providerType)
        {
            if (_providerFactoryMappers.TryGetValue(providerType, out var factory))
            {
                return factory;
            }

            return null;
        }

        /// <summary>
        /// 为数据库提供者添加插件服务。
        /// </summary>
        /// <typeparam name="TProvider">数据库提供者类型。</typeparam>
        /// <typeparam name="TService">插件服务类型。</typeparam>
        /// <returns></returns>
        public void AddProivderService<TProvider, TService>()
            where TProvider : IProvider
            where TService : IProviderService
        {
            var providerType = typeof(TProvider);
            var serviceType = typeof(TService);

            //从插件服务中查找实现 IProviderService 的接口
            var definedType = serviceType.GetInterfaces().FirstOrDefault(s => s != typeof(IProviderService) && typeof(IProviderService).IsAssignableFrom(s));
            if (definedType == null)
            {
                return;
            }

            if (!_providerServiceTypes.TryGetValue(providerType, out var mappers))
            {
                mappers = new();
                _providerServiceTypes.Add(providerType, mappers);
            }

            var map = mappers.FirstOrDefault(s => s.DefinedType == definedType);
            if (map.ServiceType == null)
            {
                mappers.Add((definedType, serviceType));
            }
            else
            {
                map.ServiceType = serviceType;
            }
        }

        /// <summary>
        /// 获取指定 <see cref="IProvider"/> 所有指定的 <see cref="IProviderService"/> 映射。
        /// </summary>
        /// <param name="providerType">数据库提供者类型。</param>
        /// <returns></returns>
        public IEnumerable<(Type DefinedType, Type ServiceType)> GetProviderServiceMappers(Type providerType)
        {
            if (_providerServiceTypes.TryGetValue(providerType, out var mappers))
            {
                foreach (var map in mappers)
                {
                    yield return map;
                }
            }
        }
    }
}
