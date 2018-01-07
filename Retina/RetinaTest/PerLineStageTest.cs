using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

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
        
        [TestMethod]
        public void TestRandomMatch()
        {
            AssertRandomProgram(new RandomTestSuite
            {
                Sources = {
                    @"@%s`.+",
                    @"$.&",
                },
                TestCases = { { "abc\ndef\nghi\njkl\nmno", new List<string>
                {
                    "3\n15",
                    "7\n11",
                    "11\n7",
                    "15\n3",
                }
                } }
            });
        }
    }
}
