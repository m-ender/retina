using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RetinaTest
{
    [TestClass]
    public class LoopTest : RetinaTestBase
    {
        [TestMethod]
        public void TestBasicLoop()
        {
            AssertProgram(new TestSuite { Sources = { @"+`." }, TestCases = { { "abcdefghijklmnopqrstuvwxyz", "1" } } });
            AssertProgram(new TestSuite { Sources = { @"+`(.)\1", @"$1" }, TestCases = { { "aaaaaaabbbbbbcccccccddddeeeee", "abcde" } } });
        }

        [TestMethod]
        public void TestGroupLoop()
        {
            AssertProgram(new TestSuite {
                Sources = {
                    @"{`ab",
                    @"c",
                    @"}`ac",
                    @"b"
                },
                TestCases = { { "aaaaaaaab", "b" } }
            });
        }
    }
}
