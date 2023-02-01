using System;

namespace Fireasy.Common.Modularity
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
    public class ModuleStarupAssemblyAttribute : Attribute
    {
        public ModuleStarupAssemblyAttribute(Type moduleType)
        {
            ModuleType = moduleType;
        }

        public Type ModuleType { get; set; }
    }
}
