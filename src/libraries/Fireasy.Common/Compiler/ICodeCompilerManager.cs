// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Fireasy.Common.Compiler
{
    /// <summary>
    /// 提供代码编译器管理的接口。
    /// </summary>
    public interface ICodeCompilerManager
    {
        /// <summary>
        /// 注册指定语言类型的代码编译器类型。
        /// </summary>
        /// <typeparam name="TCompiler"></typeparam>
        /// <param name="languages">语言。</param>
        void Register<TCompiler>(params string[] languages) where TCompiler : ICodeCompiler;

        /// <summary>
        /// 创建代码编译器。
        /// </summary>
        /// <param name="language">语言。</param>
        /// <returns></returns>
        ICodeCompiler? CreateCompiler(string language);
    }
}
