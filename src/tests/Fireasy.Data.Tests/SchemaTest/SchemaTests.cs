using Microsoft.Extensions.DependencyInjection;

namespace Fireasy.Data.Tests.SchemaTest
{
    /// <summary>
    /// <see cref="ISchemaProvider"/> 测试类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class SchemaTests<T> : DbInstanceBaseTests where T : IProvider
    {
    }
}
