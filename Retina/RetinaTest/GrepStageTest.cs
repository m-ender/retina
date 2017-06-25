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
        public void TestBasicAntiGrep()
        {
            AssertProgram(new TestSuite { Sources = { @"A`" }, TestCases = { { "\nabc\n!!!\n\ndef\n", "" } } });
            AssertProgram(new TestSuite { Sources = { @"A`." }, TestCases = { { "\nabc\n!!!\n\ndef\n", "\n\n" } } });
            AssertProgram(new TestSuite { Sources = { @"A`\w" }, TestCases = { { "\nabc\n!!!\n\ndef\n", "\n!!!\n\n" } } });
        }
    }
}
