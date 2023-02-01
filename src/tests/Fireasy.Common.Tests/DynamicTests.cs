using Fireasy.Common.Dynamic;
using Fireasy.Tests.Base;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;

namespace Fireasy.Common.Tests
{
    /// <summary>
    /// 动态测试
    /// </summary>
    [TestClass]
    public class DynamicTests : ServiceProviderBaseTests
    {
        /// <summary>
        /// 测试动态的 TypeDescriptor
        /// </summary>
        [TestMethod]
        public void TestDynamicTypeDescriptor()
        {
            //如果不获取 DynamicDescriptionSupporter 则该测试将失败
            var supporter = ServiceProvider.GetService<DynamicDescriptionSupporter>();

            var obj = (IDictionary<string, object>)new DynamicExpandoObject();
            obj.Add("Name", "fireasy");

            var property = TypeDescriptor.GetProperties(obj).Find("Name", false);

            Assert.IsNotNull(property);

            var value = property.GetValue(obj);

            Assert.AreEqual("fireasy", value);
        }
    }
}
