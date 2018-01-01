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
    }
}
