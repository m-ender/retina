using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace RetinaTest
{
    [TestClass]
    public class AntiGrepStageTest : RetinaTestBase
    {
        [TestMethod]
        public void TestBasicAntiGrep()
        {
            AssertProgram(new TestSuite { Sources = { @"A`" }, TestCases = { { "abc", "" } } });
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

        [TestMethod]
        public void TestOverlappingMatches()
        {
            AssertProgram(new TestSuite { Sources = { @"Asw`b...|c" }, TestCases = { { "\nabc\n!!!\n\ndef\n", "\n\ndef\n" } } });
        }

        [TestMethod]
        public void TestReverse()
        {
            AssertProgram(new TestSuite { Sources = { @"A^`\w" }, TestCases = { { "\nabc\n!!!\n\ndef\n", "\n\n!!!\n" } } });
            AssertProgram(new TestSuite { Sources = { @"A^sw`b...|c" }, TestCases = { { "\nabc\n!!!\n\ndef\n", "\ndef\n\n" } } });
        }

        [TestMethod]
        public void TestLineLimit()
        {
            AssertProgram(new TestSuite { Sources = { @"Am, 1,`.+" }, TestCases = { { "\nabc\n!!!\n\ndef\n", "\nabc\n\n" } } });
            AssertProgram(new TestSuite { Sources = { @"Am^, 1,`.+" }, TestCases = { { "\nabc\n!!!\n\ndef\n", "\n\nabc\n" } } });
        }

        [TestMethod]
        public void TestListFormatting()
        {
            AssertProgram(new TestSuite { Sources = { @"A['[|"", ""]']`!" }, TestCases = { { "abc\ndef\nghi", "[abc, def, ghi]" } } });
        }

        [TestMethod]
        public void TestRandom()
        {
            AssertRandomProgram(new RandomTestSuite
            {
                Sources = { @"A&`[a-z]" },
                TestCases = { { "a\nbc\ndef\n1234\nghijklmno", new string[]
                {
                    "bc\ndef\n1234\nghijklmno",
                    "a\ndef\n1234\nghijklmno",
                    "a\nbc\n1234\nghijklmno",
                    "a\nbc\ndef\n1234",
                } } }
            });

            AssertRandomProgram(new RandomTestSuite
            {
                Sources = { @"A&, 1,`[a-z]" },
                TestCases = { { "a\nbc\ndef\n1234\nghijklmno", new string[]
                {
                    "a\ndef\n1234\nghijklmno",
                    "a\nbc\n1234\nghijklmno",
                    "a\nbc\ndef\n1234",
                } } }
            });
        }
    }
}
