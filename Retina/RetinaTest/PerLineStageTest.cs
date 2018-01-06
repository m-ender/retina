using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RetinaTest
{
    [TestClass]
    public class PerLineStageTest : RetinaTestBase
    {
        [TestMethod]
        public void TestBasicPerLineMode()
        {
            AssertProgram(new TestSuite
            {
                Sources =
                {
                    "%`.",
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
                    "',%`.",
                    "$_",
                },
                TestCases = { { "123,abc,<>", "123123123,abcabcabc,<><>" } }
            });
        }

        [TestMethod]
        public void TestStringParam()
        {
            AssertProgram(new TestSuite
            {
                Sources =
                {
                    "\", \"%`.",
                    "$_",
                },
                TestCases = { { "123, abc, <>", "123123123, abcabcabc, <><>" } }
            });
        }

        [TestMethod]
        public void TestRegexParam()
        {
            AssertProgram(new TestSuite
            {
                Sources =
                {
                    @"/\W+/%`.",
                    "$_",
                },
                TestCases = { { "123, abc; XYZ", "123123123, abcabcabc; XYZXYZXYZ" } }
            });
        }

        [TestMethod]
        public void TestLineLimit()
        {
            AssertProgram(new TestSuite
            {
                Sources =
                {
                    ",2,%`.",
                    "$&$&",
                },
                TestCases = { { "abc\ndef\nghi\njkl\nmno", "aabbcc\ndef\ngghhii\njkl\nmmnnoo" } }
            });
        }
    }
}
