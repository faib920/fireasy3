// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Fireasy.Common.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Fireasy.Common.Modularity
{
    public class DefaultModuleStartup : IModuleStartup
    {
        public DefaultModuleStartup(IServiceCollection services, IConfiguration configuration, Assembly assembly)
        {
            services.AddSingleton<IModuleStartup>(this);

            Initialize(services, configuration, assembly);
        }

        private void Initialize(IServiceCollection services, IConfiguration configuration, Assembly assembly)
        {
            var modules = new List<Module>();

            assembly.ForEachAssemblies(ass =>
            {
                var attrReg = ass.GetCustomAttributes<ModuleStarupAssemblyAttribute>().FirstOrDefault();
                if (attrReg?.ModuleType != null && typeof(Module).IsAssignableFrom(attrReg.ModuleType))
                {
                    var module = attrReg.ModuleType.New<Module>();
                    if (module != null)
                    {
                        modules.Add(module);
                    }
                }

            }, FilterAssembly);

            foreach (var module in modules)
            {
                module.OnPreConfigure();

                module.OnPostConfigure();
            }
        }

        private static bool FilterAssembly(Assembly assembly)
        {
            return assembly.IsDefined(typeof(ModuleStarupAssemblyAttribute));
        }

        public void Initialize(IServiceProvider serviceProvider)
        {
        }

        private class Hero
        {
        }
    }
}
