// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Fireasy.Common;
using Fireasy.Common.ObjectActivator;
using Fireasy.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Fireasy.Data.Configuration
{
    /// <summary>
    /// 基于路由服务的数据库实例配置。
    /// </summary>
    [Serializable]
    public class RouteInstanceSetting : DefaultInstanceConfigurationSetting
    {
        /// <summary>
        /// 返回数据库类型。
        /// </summary>
        public override string ProviderType
        {
            get { return GetSetting().ProviderType; }
            set { }
        }

        /// <summary>
        /// 获取数据库连接字符串。
        /// </summary>
        public override string ConnectionString
        {
            get { return GetSetting().ConnectionString; }
            set { }
        }

        /// <summary>
        /// 获取或设置路由服务对象。
        /// </summary>
        public IInstanceRouteService RouteService { get; private set; }

        private IInstanceConfigurationSetting GetSetting()
        {
            Guard.NullReference(RouteService);
            return RouteService.GetSetting();
        }

        internal class SettingParseHandler : IConfigurationSettingParseHandler
        {
            public IConfigurationSettingItem Parse(BindingContext context)
            {
                var setting = new RouteInstanceSetting();
                var type = Type.GetType(context.Configuration.GetSection("type").Value, false, true);
                if (type != null)
                {
                    var objActivator = context.ServiceProvider.GetRequiredService<IObjectActivator>();
                    setting.RouteService = objActivator.CreateInstance(type) as IInstanceRouteService;
                }
                return setting;
            }

        }
    }
}
