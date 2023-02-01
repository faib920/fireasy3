using Fireasy.Common.Extensions;

namespace Fireasy.Common.Tests
{
    [TestClass]
    public class AsyncEnumerableTests
    {
        [TestMethod]
        public async void TestAsyncToList()
        {
            var list = await GetAsyncIntegers().ToListAsync();
            Assert.AreEqual(100, list.Count);
        }

        [TestMethod]
        public async void TestAsyncAsEnumerable()
        {
            var count = 0;
            foreach (var item in await GetAsyncIntegers().AsEnumerable())
            {
                count++;
            }
            Assert.AreEqual(100, count);
        }

        [TestMethod]
        public async Task TestAsAsyncEnumerable()
        {
            var count = 0;
            await foreach (var item in GetIntegers().AsAsyncEnumerable())
            {
                count++;
            }
            Assert.AreEqual(100, count);
        }

        private async IAsyncEnumerable<int> GetAsyncIntegers()
        {
            for (var i = 0; i < 100; i++)
            {
                yield return i;
            }
        }

        private IEnumerable<int> GetIntegers()
        {
            for (var i = 0; i < 100; i++)
            {
                yield return i;
            }
        }
    }
}
