// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Microsoft.Extensions.Options;
using System.IO;
using System.Reflection;

namespace Fireasy.Common.Compiler
{
    /// <summary>
    /// 扩展方法。
    /// </summary>
    public static class CodeCompilerExtensions
    {
        /// <summary>
        /// 使用一组源程序文件来生成一个程序集。
        /// </summary>
        /// <param name="compiler">代码编译器。</param>
        /// <param name="fileNames">外部的一组源程序文件。</param>
        /// <param name="options">配置选项。</param>
        /// <returns>由代码编译成的程序集。</returns>
        public static Assembly? CompileAssemblyFromFiles(this ICodeCompiler compiler, string[] fileNames, ConfigureOptions? options = null)
        {
            return compiler.CompileAssembly(fileNames.Select(s => File.ReadAllText(s)).ToArray(), options);
        }

        /// <summary>
        /// 使用一段源代码来生成一个程序集。
        /// </summary>
        /// <param name="compiler">代码编译器。</param>
        /// <param name="source">源代码。</param>
        /// <param name="options">配置选项。</param>
        /// <returns>由代码编译成的程序集。</returns>
        public static Assembly? CompileAssembly(this ICodeCompiler compiler, string source, ConfigureOptions? options = null)
        {
            return compiler.CompileAssembly(new[] { source }, options);
        }

        /// <summary>
        /// 编译代码并返回指定方法的委托。如果未指定方法名称，则返回类的第一个方法。
        /// </summary>
        /// <param name="compiler">代码编译器。</param>
        /// <typeparam name="TDelegate">委托类型。</typeparam>
        /// <param name="source">程序源代码，代码中只允许包含一个类。</param>
        /// <param name="methodName">方法的名称。</param>
        /// <param name="options">配置选项。</param>
        /// <returns>代码中对应方法的委托。</returns>
        public static TDelegate? CompileDelegate<TDelegate>(this ICodeCompiler compiler, string source, string? methodName = null, ConfigureOptions? options = null)
        {
            var compileType = compiler.CompileType(source, options: options);
            return MakeDelegate<TDelegate>(compileType, methodName);
        }

        /// <summary>
        /// 编译代码生成一个新类型。
        /// </summary>
        /// <param name="compiler">代码编译器。</param>
        /// <param name="source">程序源代码。</param>
        /// <param name="typeName">类的名称。</param>
        /// <param name="options">配置选项。</param>
        /// <returns>由代码编译成的动态类型。</returns>
        public static Type? CompileType(this ICodeCompiler compiler, string source, string? typeName = null, ConfigureOptions? options = null)
        {
            var assembly = compiler.CompileAssembly(source, options);

            return GetTypeFromAssembly(assembly, typeName);
        }

        private static Type? GetTypeFromAssembly(Assembly? assembly, string? typeName = null)
        {
            if (assembly != null)
            {
                if (!string.IsNullOrEmpty(typeName))
                {
                    return assembly.GetType(typeName);
                }

                var types = assembly.GetExportedTypes();
                if (types.Length > 0)
                {
                    return types[0];
                }
            }

            return null;
        }

        private static TDelegate? MakeDelegate<TDelegate>(Type? compileType, string? methodName)
        {
            if (compileType == null)
            {
                return default;
            }

            var method = string.IsNullOrEmpty(methodName) ? compileType.GetMethods()[0] : compileType.GetMethod(methodName);
            if (method == null)
            {
                throw new CodeCompileException(string.IsNullOrEmpty(methodName) ? $"类型 {compileType} 中未包含任何方法。" : $"方法 {methodName} 未找到。");
            }

            if (!method.IsStatic && !compileType.GetTypeInfo().GetConstructors().Any(s => s.GetParameters().Length == 0))
            {
                throw new CodeCompileException($"类型 {compileType} 必须定义无参数的构造函数。");
            }

            return (TDelegate)(object)Delegate.CreateDelegate(typeof(TDelegate), method.IsStatic ? null : Activator.CreateInstance(compileType), method);
        }

    }
}
