// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Fireasy.Data.Analyzers.BulkCopyProvider.Generator.Builders;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Fireasy.Data.Analyzers.BulkCopyProvider.Generator
{
    [Generator]
    public class BulkCopyProviderGenerator : ISourceGenerator
    {
        private readonly Dictionary<string, List<BuildMap>> _providers = new ()
        {
            { "SqlServerProvider", new () { new BuildMap(typeof(Microsoft_Data_SqlClient), "Microsoft.Data.SqlClient"), new BuildMap(typeof(System_Data_SqlClient), "System.Data.SqlClient") } },
            { "MySqlProvider", new () { new BuildMap(typeof(MySqlConnector), "MySqlConnector") } },
            { "OracleProvider", new () { new BuildMap(typeof(Oracle_ManagedDataAccess), "Oracle.ManagedDataAccess") } },
            { "DamengProvider", new () { new BuildMap(typeof(DmProvider), "DmProvider") } },
            { "PostgreSqlProvider", new () { new BuildMap(typeof(Npgsql), "Npgsql") } },
            { "KingbaseProvider", new () { new BuildMap(typeof(Kdbndp), "Kdbndp") } }
        };
        private readonly Dictionary<string, string> _buildFlags = new();

        private struct BuildMap
        {
            public BuildMap(Type builderType, string assembly)
            {
                BuilderType = builderType;
                Assembly = assembly;
            }

            public Type BuilderType { get; }

            public string Assembly { get; }
        }

        void ISourceGenerator.Initialize(GeneratorInitializationContext context)
        {
            //Debugger.Launch();
        }

        void ISourceGenerator.Execute(GeneratorExecutionContext context)
        {
            var references = context.Compilation.SourceModule.ReferencedAssemblies;

            foreach (var kvp in _providers)
            {
                foreach (var map in kvp.Value)
                {
                    if (_buildFlags.ContainsKey(kvp.Key))
                    {
                        continue;
                    }

                    if (references.Any(s => s.Name.Equals(map.Assembly)))
                    {
                        var builder = Activator.CreateInstance(map.BuilderType) as IBulkCopyProviderBuilder;
                        if (builder != null)
                        {
                            var source = builder.BuildSource();
                            context.AddSource(builder.BulkCopyProviderTypeName + ".cs", source);
                            _buildFlags.Add(kvp.Key, builder.BulkCopyProviderTypeName);
                        }
                    }
                }
            }

            if (_buildFlags.Count > 0)
            {
                context.AddSource("BulkCopyProviderServicesDeployer.cs", BuildDiscoverSourceCode());
            }
        }

        private SourceText BuildDiscoverSourceCode()
        {
            var sb = new StringBuilder();
            sb.AppendLine(@"
using Fireasy.Common.DependencyInjection;
using Fireasy.Data.Provider;
using Fireasy.Data.Batcher;
using Microsoft.Extensions.DependencyInjection;

[assembly: Fireasy.Common.DependencyInjection.ServicesDeployAttribute(typeof(__BulkCopyProviderNs.__BulkCopyProviderServicesDeployer), Priority = 1)]

namespace __BulkCopyProviderNs
{
    internal class __BulkCopyProviderServicesDeployer: IServicesDeployer
    {
        void IServicesDeployer.Configure(IServiceCollection services)
        {
            var customizer = services.GetOrAddObjectAccessor<ProviderCustomizer>();
");

            foreach (var flag in _buildFlags)
            {
                sb.AppendLine($@"            customizer.AddProivderService<{flag.Key}, {flag.Value}>();");
            }
            sb.AppendLine(@"
        }
    }
}");
            return SourceText.From(sb.ToString(), Encoding.UTF8);
        }

    }
}
