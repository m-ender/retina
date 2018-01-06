using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RetinaTest
{
    [TestClass]
    public class MatchMaskStageTest : RetinaTestBase
    {
        [TestMethod]
        public void TestBasicMatchMask()
        {
            AssertProgram(new TestSuite
            {
                Sources =
                {
                    "_`.",
                    "$_",
                },
                TestCases = { { "123\nabc\n<>", "123123123\nabcabcabc\n<><>" } }
            });
        }

        [TestMethod]
        public void TestCharacterParam()
        {
            AssertProgram(new TestSuite
            {
                Sources =
                {
                    "',_`.",
                    "$_$_",
                },
                TestCases = { { "123,abc,<>", "123,,abc,,<>" } }
            });
        }

        [TestMethod]
        public void TestStringParam()
        {
            AssertProgram(new TestSuite
            {
                Sources =
                {
                    "\", \"_`.",
                    "$_",
                },
                TestCases = { { "123, abc, <>", "123, , abc, , <>" } }
            });
        }

        [TestMethod]
        public void TestRegexParam()
        {
            AssertProgram(new TestSuite
            {
                Sources =
                {
                    @"/\w+/_`.",
                    "$_",
                },
                TestCases = { { "123, abc; XYZ", "123123123, abcabcabc; XYZXYZXYZ" } }
            });
        }

        [TestMethod]
        public void TestMatchLimit()
        {
            AssertProgram(new TestSuite
            {
                Sources =
                {
                    ",2,_`.",
                    "$&$&",
                },
                TestCases = { { "abc\ndef\nghi\njkl\nmno", "aabbcc\ndef\ngghhii\njkl\nmmnnoo" } }
            });
        }
    }
}
