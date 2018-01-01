using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RetinaTest
{
    [TestClass]
    public class AntiGrepStageTest : RetinaTestBase
    {
        [TestMethod]
        public void TestBasicAntiGrep()
        {
            AssertProgram(new TestSuite { Sources = { @"A`" }, TestCases = { { "\nabc\n!!!\n\ndef\n", "" } } });
            AssertProgram(new TestSuite { Sources = { @"A`." }, TestCases = { { "\nabc\n!!!\n\ndef\n", "\n\n" } } });
            AssertProgram(new TestSuite { Sources = { @"A`\w" }, TestCases = { { "\nabc\n!!!\n\ndef\n", "\n!!!\n\n" } } });
        }

        [TestMethod]
        public void TestMatchAcrossLines()
        {
            AssertProgram(new TestSuite { Sources = { @"As`.+" }, TestCases = { { "\nabc\n!!!\n\ndef\n", "" } } });
            AssertProgram(new TestSuite { Sources = { @"A`\n" }, TestCases = { { "\nabc\n!!!\n\ndef\n", "" } } });
            AssertProgram(new TestSuite { Sources = { @"A`\w\n" }, TestCases = { { "\nabc\n!!!\n\ndef\n", "\n" } } });
        }
    }
}
