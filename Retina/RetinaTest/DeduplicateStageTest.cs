using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RetinaTest
{
    [TestClass]
    public class DeduplicateStageTest : RetinaTestBase
    {
        [TestMethod]
        public void TestBasicDeduplication()
        {
            AssertProgram(new TestSuite { Sources = { @"D`." }, TestCases = { { "abacbcedef", "abcedf" } } });
            AssertProgram(new TestSuite { Sources = { @"D`\w+" }, TestCases = { { "abc def abc ab ghi def", "abc def  ab ghi " } } });
        }

        [TestMethod]
        public void TestRTLMatching()
        {
            AssertProgram(new TestSuite { Sources = { @"Dr`." }, TestCases = { { "abacbcedef", "abcdef" } } });
            AssertProgram(new TestSuite { Sources = { @"Dr`\w+" }, TestCases = { { "abc def abc ab ghi def", "  abc ab ghi def" } } });
        }

        [TestMethod]
        public void TestDeduplicateBy()
        {
            AssertProgram(new TestSuite { Sources = { @"D$`(\w)\w*", "$1" }, TestCases = { { "abc def abc ab ghi def", "abc def   ghi " } } });
        }
    }
}
