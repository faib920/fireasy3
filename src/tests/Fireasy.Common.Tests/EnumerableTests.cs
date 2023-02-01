using Fireasy.Common.Extensions;

namespace Fireasy.Common.Tests
{
    [TestClass]
    public class EnumerableTests
    {
        [TestMethod]
        public void TestForEach()
        {
            var list = Enumerable.Range(0, 50);

            list.ForEach(s =>
            {
                Console.WriteLine(s);
            });
        }

        [TestMethod]
        public void TestForEach4Index()
        {
            var list = Enumerable.Range(0, 50);

            list.ForEach((s, i) =>
            {
                Console.WriteLine(s + " " + i);
            });
        }

        [TestMethod]
        public void TestSplit()
        {
            var list = Enumerable.Range(0, 1035);

            //500-500-35
            Assert.AreEqual(35, list.Split().ElementAt(2).Count());
        }

        [TestMethod]
        public void TestSplitUseAverage()
        {
            var list = Enumerable.Range(0, 1035);

            //500-268-267
            Assert.AreEqual(267, list.Split(splitMode: SequenceSplitMode.Equationally).ElementAt(2).Count());
        }

        [TestMethod]
        public void TestSplitUseAverage1()
        {
            var list = Enumerable.Range(0, 1034);

            //500-267-267
            Assert.AreEqual(267, list.Split(splitMode: SequenceSplitMode.Equationally).ElementAt(2).Count());
        }
    }
}
