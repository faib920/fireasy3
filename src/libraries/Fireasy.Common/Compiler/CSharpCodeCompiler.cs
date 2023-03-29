// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using System.IO;
using System.Reflection;
using System.Text;

namespace Fireasy.Common.Compiler
{
    /// <summary>
    /// CSharp 代码编译器。无法继承此类。
    /// </summary>
    public sealed class CSharpCodeCompiler : ICodeCompiler
    {
        /// <summary>
        /// 编译代码生成一个程序集。
        /// </summary>
        /// <param name="sources">程序源代码。</param>
        /// <param name="options">配置选项。</param>
        /// <returns>由代码编译成的程序集。</returns>
        public Assembly? CompileAssembly(IEnumerable<string> sources, ConfigureOptions? options = null)
        {
            var dotnetPath = Path.GetDirectoryName(typeof(object).Assembly.Location);
            Func<string, MetadataReference> parser = path => File.Exists(path) ?
                MetadataReference.CreateFromFile(path) :
                MetadataReference.CreateFromFile(Path.Combine(dotnetPath, path));

            options ??= new ConfigureOptions();
            var assemblyName = string.IsNullOrWhiteSpace(options.AssemblyName) ? Guid.NewGuid().ToString() : options.AssemblyName;
            var compilation = CSharpCompilation.Create(assemblyName)
                .AddSyntaxTrees(sources.Select(s => CSharpSyntaxTree.ParseText(s)))
                .AddReferences(MetadataReference.CreateFromFile(typeof(object).Assembly.Location))
                .AddReferences(options.Assemblies.Select(parser))
                .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, optimizationLevel: OptimizationLevel.Release));

            if (!string.IsNullOrEmpty(options.OutputAssembly))
            {
                var result = compilation.Emit(options.OutputAssembly);
                if (result.Success)
                {
                    return Assembly.LoadFrom(options.OutputAssembly);
                }
                else
                {
                    ThrowCompileException(result);
                    return null;
                }
            }
            else
            {
                using var ms = new MemoryStream();
                var result = compilation.Emit(ms);
                if (result.Success)
                {
                    return Assembly.Load(ms.ToArray());
                }
                else
                {
                    ThrowCompileException(result);
                    return null;
                }
            }
        }

        private void ThrowCompileException(EmitResult result)
        {
            var errorBuilder = new StringBuilder();

            foreach (var diagnostic in result.Diagnostics.Where(diagnostic =>
                        diagnostic.IsWarningAsError ||
                        diagnostic.Severity == DiagnosticSeverity.Error))
            {
                errorBuilder.AppendFormat("{0}: {1}", diagnostic.Id, diagnostic.GetMessage());
            }

            throw new CodeCompileException(errorBuilder.ToString());
        }
    }
}
