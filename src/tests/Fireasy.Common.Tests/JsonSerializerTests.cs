using Fireasy.Common.Serialization;
using Fireasy.Tests.Base;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;

namespace Fireasy.Common.Tests
{
    /// <summary>
    /// Json 序列化测试
    /// </summary>
    [TestClass]
    public class JsonSerializerTests : ServiceProviderBaseTests
    {
        /// <summary>
        /// 测试序列化
        /// </summary>
        [TestMethod]
        public void TestSerialize()
        {
            var obj = new TestObject { Name = "fireasy", Address = "kunming", Age = 30 };

            var serializer = ServiceProvider.GetRequiredService<IJsonSerializer>();
            var json = serializer.Serialize(obj);

            Console.WriteLine(json);
        }

        /// <summary>
        /// 测试序列化（使用选项）
        /// </summary>
        [TestMethod]
        public void TestSerializeUseOptions()
        {
            var obj = new TestObject { Name = "fireasy", Address = "kunming", Age = 30 };

            var serializer = ServiceProvider.GetRequiredService<IJsonSerializer>();
            var json = serializer.Serialize(obj, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, WriteIndented = true });

            Console.WriteLine(json);
        }

        /// <summary>
        /// 测试反序列化
        /// </summary>
        [TestMethod]
        public void TestDeserialize()
        {
            var json = """
{
    "Name": "fireasy",
    "Address": "kunming",
    "Age": 30
}
""";

            var serializer = ServiceProvider.GetRequiredService<IJsonSerializer>();
            var obj = serializer.Deserialize<TestObject>(json);

            Assert.AreEqual("fireasy", obj.Name);
        }

        /// <summary>
        /// 测试反序列化匿名类型
        /// </summary>
        [TestMethod]
        public void TestDeserializeAnonymous()
        {
            var json = """
{
    "Name": "fireasy",
    "Address": "kunming",
    "Age": 30
}
""";

            var serializer = ServiceProvider.GetRequiredService<IJsonSerializer>();
            var obj = serializer.Deserialize(json, new { Name = "", Address= "", Age = 1 });

            Assert.AreEqual("fireasy", obj.Name);
        }

        /// <summary>
        /// 测试反序列化动态类型
        /// </summary>
        [TestMethod]
        public void TestDeserializeDynamic()
        {
            var json = """
{
    "Name": "fireasy",
    "Address": "kunming",
    "Age": 30,
    "Works": [
        {
            "Id": 1,
            "Name": "work1",
            "Current": true
        },
        {
            "Id": 2,
            "Name": "work2",
            "Current": false
        }
    ]
}
""";

            var serializer = ServiceProvider.GetRequiredService<IJsonSerializer>();
            var obj = serializer.Deserialize(json);

            Assert.AreEqual("fireasy", obj.Name);
            Assert.AreEqual(2, obj.Works.Count);
            Assert.AreEqual("work1", obj.Works[0].Name);
        }

        public class TestObject
        {
            public string Name { get; set; }

            public string Address { get; set; }

            public int Age { get; set; }
        }
    }
}
