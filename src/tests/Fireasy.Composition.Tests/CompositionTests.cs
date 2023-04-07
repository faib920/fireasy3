using Fireasy.Tests.Base;
using Microsoft.Extensions.Logging;
using System.ComponentModel.Composition;

namespace Fireasy.Composition.Tests
{
    [TestClass]
    public class CompositionTests : ConfigurationBaseTests
    {
        [TestMethod]
        public void TestGetExportedServices()
        {
            var services = ServiceProvider.GetExportedServices<IExportService>();
            Assert.AreEqual(2, services.Count());
        }
    }

    public interface IExportService
    {
        void Hello();
    }

    [
    [Export(typeof(IExportService))]
    public class ExportService1 : IExportService
    {
        public void Hello()
        {
        }
    }

    [Export(typeof(IExportService))]
    public class ExportService2 : IExportService
    {
        public void Hello()
        {
        }
    }
}