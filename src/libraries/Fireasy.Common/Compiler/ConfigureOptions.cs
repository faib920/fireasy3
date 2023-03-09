namespace Fireasy.Common.Compiler
{
    public class ConfigureOptions
    {
        /// <summary>
        /// 获取或设置输出的程序集。
        /// </summary>
        public string? OutputAssembly { get; set; }

        /// <summary>
        /// 获取或设置编译选项。
        /// </summary>
        public string? CompilerOptions { get; set; }

        /// <summary>
        /// 获取附加的程序集。
        /// </summary>
        public List<string> Assemblies { get; private set; } = new List<string>();
    }
}
