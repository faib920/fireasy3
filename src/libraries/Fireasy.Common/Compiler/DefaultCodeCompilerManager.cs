// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Fireasy.Common.Extensions;
using Fireasy.Common.ObjectActivator;
using Microsoft.Extensions.DependencyInjection;

namespace Fireasy.Common.Compiler
{
    /// <summary>
    /// 缺省的代码编译器管理器。
    /// </summary>
    public class DefaultCodeCompilerManager : ICodeCompilerManager
    {
        private readonly Dictionary<string, Type> _languageMappers = new(new StringIgnoreCaseComparer());

        private class StringIgnoreCaseComparer : IEqualityComparer<string>
        {
            public bool Equals(string x, string y)
            {
                return string.Compare(x, y, true) == 0;
            }

            public int GetHashCode(string obj)
            {
                return obj?.GetHashCode() ?? 0;
            }
        }

        /// <summary>
        /// 初始化 <see cref="DefaultCodeCompilerManager"/> 类新实例。
        /// </summary>
        public DefaultCodeCompilerManager()
        {
            Register<CSharpCodeCompiler>("csharp", "c#");
        }

        /// <summary>
        /// 注册指定语言类型的代码编译器类型。
        /// </summary>
        /// <typeparam name="TCompiler"></typeparam>
        /// <param name="languages">语言。</param>
        public void Register<TCompiler>(params string[] languages) where TCompiler : ICodeCompiler
        {
            foreach (var language in languages)
            {
                _languageMappers.AddOrReplace(language, typeof(TCompiler));
            }
        }

        /// <summary>
        /// 创建代码编译器。
        /// </summary>
        /// <param name="language">语言。</param>
        /// <returns></returns>
        public ICodeCompiler? CreateCompiler(string language)
        {
            if (_languageMappers.TryGetValue(language, out var compilerType))
            {
                return Activator.CreateInstance(compilerType) as ICodeCompiler;
            }

            return null;
        }
    }
}
