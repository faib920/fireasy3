// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using System.Collections.Generic;

namespace Fireasy.Configuration
{
    /// <summary>
    /// 用于绑定的 <see cref="IConfigurationSection"/>。
    /// </summary>
    public sealed class BindingConfiguration : Microsoft.Extensions.Configuration.IConfigurationSection
    {
        internal BindingConfiguration(IConfiguration root, string path)
        {
            if (root is BindingConfiguration bc)
            {
                Root = bc.Root;
            }
            else
            {
                Root = root;
            }

            Current = root.GetSection(path);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="root"></param>
        /// <param name="current"></param>
        public BindingConfiguration(IConfiguration root, Microsoft.Extensions.Configuration.IConfigurationSection current)
        {
            if (root is BindingConfiguration bc)
            {
                Root = bc.Root;
            }
            else
            {
                Root = root;
            }

            Current = current;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string? this[string key] { get => Current[key]; set => Current[key] = value; }

        /// <summary>
        /// 获取根节点配置。
        /// </summary>
        public IConfiguration Root { get; }

        /// <summary>
        /// 获取当前节点配置。
        /// </summary>
        public Microsoft.Extensions.Configuration.IConfigurationSection Current { get; }

        /// <summary>
        /// 
        /// </summary>
        public string Key => Current.Key;

        /// <summary>
        /// 
        /// </summary>
        public string Path => Current.Path;

        /// <summary>
        /// 
        /// </summary>
        public string Value { get => Current.Value; set => Current.Value = value; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Microsoft.Extensions.Configuration.IConfigurationSection> GetChildren()
        {
            return Current.GetChildren();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IChangeToken GetReloadToken()
        {
            return Current.GetReloadToken();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public Microsoft.Extensions.Configuration.IConfigurationSection GetSection(string key)
        {
            return Current.GetSection(key);
        }
    }
}