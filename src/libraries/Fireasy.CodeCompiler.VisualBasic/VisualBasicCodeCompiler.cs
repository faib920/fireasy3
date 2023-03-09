// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Fireasy.Common.Compiler;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.VisualBasic;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Fireasy.CodeCompiler.VBasic
{
    /// <summary>
    /// VisualBasic 代码编译器。无法继承此类。
    /// </summary>
    public class VisualBasicCodeCompiler : ICodeCompiler
    {
        /// <summary>
        /// 编译代码生成一个程序集。
        /// </summary>
        /// <param name="source">程序源代码。</param>
        /// <param name="options">配置选项。</param>
        /// <returns>由代码编译成的程序集。</returns>
        public Assembly? CompileAssembly(string source, ConfigureOptions? options = null)
        {
            options ??= new ConfigureOptions();
            var compilation = VisualBasicCompilation.Create(Guid.NewGuid().ToString())
                .AddSyntaxTrees(VisualBasicSyntaxTree.ParseText(source))
                .AddReferences(MetadataReference.CreateFromFile(typeof(object).Assembly.Location))
                .AddReferences(options.Assemblies.Select(s => MetadataReference.CreateFromFile(s)))
                .WithOptions(new VisualBasicCompilationOptions(OutputKind.DynamicallyLinkedLibrary, optimizationLevel: OptimizationLevel.Release));

            if (!string.IsNullOrEmpty(options.OutputAssembly))
            {
                var result = compilation.Emit(options.OutputAssembly);
                if (result.Success)
                {
                    return Assembly.Load(options.OutputAssembly);
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
