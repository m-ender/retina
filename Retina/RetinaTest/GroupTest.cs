using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RetinaTest
{
    [TestClass]
    public class GroupTest : RetinaTestBase
    {
        [TestMethod]
        public void TestRegexModifiers()
        {
            AssertProgram(new TestSuite
            {
                Sources =
                {
                    "[A-Z]",
                    "$&$&",
                    "i(G`",
                    "[A-Z]",
                    "$&$&",
                    ")G`",
                    "[A-Z]",
                    "$.&",
                },
                TestCases = { { "Hello, World!", "1111eelllloo, 1111oorrlldd!" } }
            });
        }

        [TestMethod]
        public void TestImplicitParentheses()
        {
            AssertProgram(new TestSuite
            {
                Sources =
                {
                    "[A-Z]",
                    "$&$&",
                    "i(G`",
                    "[A-Z]",
                    "$&$&",
                },
                TestCases = { { "Hello, World!", "HHHHeelllloo, WWWWoorrlldd!" } }
            });

            AssertProgram(new TestSuite
            {
                Sources =
                {
                    "[A-Z]",
                    "$&$&",
                    "i)G`",
                    "[A-Z]",
                    "$&$&",
                },
                TestCases = { { "Hello, World!", "HHHHeelllloo, WWWWoorrlldd!" } }
            });
        }

        [TestMethod]
        public void TestPerLineMode()
        {
            AssertProgram(new TestSuite
            {
                Sources =
                {
                    "%(S`,",
                    ".+",
                },
                TestCases = { { "abc\ndef,ghi\n123,456,789", "1\n2\n3" } }
            });


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
        public void TestDryRun()
        {
            AssertProgram(new TestSuite
            {
                Sources =
                {
                    @"[a-z]",
                    @"a",
                    @"*>(`([a-z])[a-z]*",
                    @"$1",
                    @")`(\W)\W*",
                    @"$1",
                    @"."
                },
                TestCases = { { "Hello, World!", "Ha,Wa!13" } }
            });
        }
    }
}
