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
                    "$\"",
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
                    "$\"",
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
                    "$\"",
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
                    "$\"",
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
                Sources = { @"@%`." },
                TestCases = { { "abc\ndef\nghi\njkl\nmno", new string[]
                {
                    "3\n12",
                    "6\n9",
                    "9\n6",
                    "12\n3",
                }
                } }
            });
        }

        [TestMethod]
        public void TestRandomLine()
        {
            AssertRandomProgram(new RandomTestSuite
            {
                Sources = { @"&%`." },
                TestCases = { { "abc\ndef\nghi\njkl\nmno", new string[]
                {
                    "3\ndef\nghi\njkl\nmno",
                    "abc\n3\nghi\njkl\nmno",
                    "abc\ndef\n3\njkl\nmno",
                    "abc\ndef\nghi\n3\nmno",
                    "abc\ndef\nghi\njkl\n3",
                }
                } }
            });
        }
    }
}
