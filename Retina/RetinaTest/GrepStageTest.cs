using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RetinaTest
{
    [TestClass]
    public class GrepStageTest : RetinaTestBase
    {
        [TestMethod]
        public void TestBasicGrep()
        {
            AssertProgram(new TestSuite { Sources = { @"G`" }, TestCases = { { "\nabc\n!!!\n\ndef\n", "\nabc\n!!!\n\ndef\n" } } });
            AssertProgram(new TestSuite { Sources = { @"G`." }, TestCases = { { "\nabc\n!!!\n\ndef\n", "abc\n!!!\ndef" } } });
            AssertProgram(new TestSuite { Sources = { @"G`\w" }, TestCases = { { "\nabc\n!!!\n\ndef\n", "abc\ndef" } } });
        }

        [TestMethod]
        public void TestMatchAcrossLines()
        {
            AssertProgram(new TestSuite { Sources = { @"Gs`.+" }, TestCases = { { "\nabc\n!!!\n\ndef\n", "\nabc\n!!!\n\ndef\n" } } });
            AssertProgram(new TestSuite { Sources = { @"G`\n" }, TestCases = { { "\nabc\n!!!\n\ndef\n", "\nabc\n!!!\n\ndef\n" } } });
            AssertProgram(new TestSuite { Sources = { @"G`\w\n" }, TestCases = { { "\nabc\n!!!\n\ndef\n", "abc\n!!!\ndef\n" } } });
        }

        [TestMethod]
        public void TestOverlappingMatches()
        {
            AssertProgram(new TestSuite { Sources = { @"Gsw`b...|c" }, TestCases = { { "\nabc\n!!!\n\ndef\n", "abc\n!!!" } } });
        }

        [TestMethod]
        public void TestReverse()
        {
            AssertProgram(new TestSuite { Sources = { @"G^`" }, TestCases = { { "\nabc\n!!!\n\ndef\n", "\ndef\n\n!!!\nabc\n" } } });
            AssertProgram(new TestSuite { Sources = { @"G^sw`b...|c" }, TestCases = { { "\nabc\n!!!\n\ndef\n", "!!!\nabc" } } });
        }
    }
}
