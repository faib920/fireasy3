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
        /// <param name="providerType"></param>
        /// <returns></returns>
        public DbProviderFactory? GetDbProviderFactory(Type providerType)
        {
            if (_providerFactoryMappers.TryGetValue(providerType, out var factory))
            {
                return factory;
            }

            return null;
        }
    }
}
