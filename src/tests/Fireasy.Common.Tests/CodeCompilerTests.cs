using Fireasy.Common.Compiler;
using Fireasy.Tests.Base;
using Microsoft.Extensions.DependencyInjection;

namespace Fireasy.Common.Tests
{
    [TestClass]
    public class CodeCompilerTests : ServiceProviderBaseTests
    {
        [TestMethod]
        public void TestCompileAssembly()
        {
            var source = @"
public class A
{
    public string Hello(string str)
    {
        return str;
    }
}";
            var codeCompilerManager = ServiceProvider.GetService<ICodeCompilerManager>();
            var codeCompiler = codeCompilerManager!.CreateCompiler("csharp");

            var opt = new ConfigureOptions();
            opt.Assemblies.Add("System.Core.dll");

            var assembly = codeCompiler!.CompileAssembly(source, opt);

            var type = assembly!.GetType("A");

            Assert.IsNotNull(type);
        }

        /// <summary>
        /// 使用vb源代码
        /// </summary>
        [TestMethod]
        public void TestCompileAssemblyUseVb()
        {
            var source = @"
Public Class A
    Public Function Hello(ByVal str As String) As String
        Return str
    End Function
End Class";
            var codeCompilerManager = ServiceProvider.GetService<ICodeCompilerManager>();
            var codeCompiler = codeCompilerManager!.CreateCompiler("vb");

            var assembly = codeCompiler!.CompileAssembly(source);

            var type = assembly!.GetType("A");

            Assert.IsNotNull(type);
        }
    }
}
