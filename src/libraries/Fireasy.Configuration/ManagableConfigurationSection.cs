// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Fireasy.Common.ObjectActivator;
using Fireasy.Common.Extensions;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Fireasy.Configuration
{
    /// <summary>
    /// 可托管的基于实例的配置节。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class ManagableConfigurationSection<T> : DefaultInstaneConfigurationSection<T>, IManagableConfigurationSection where T : IConfigurationSettingItem
    {
        /// <summary>
        /// 获取实例创建工厂。
        /// </summary>
        public IManagedFactory? Factory { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public override void Bind(BindingContext context)
        {
            var factory = context.Configuration.GetSection("managed").Value;
            if (!string.IsNullOrEmpty(factory))
            {
                var type = factory!.ParseType();
                if (type != null)
                {
                    var objActivator = context.ServiceProvider.GetRequiredService<IObjectActivator>();
                    Factory = objActivator.CreateInstance(type) as IManagedFactory;
                }
            }

            base.InternalBind(context);
        }
    }
}
