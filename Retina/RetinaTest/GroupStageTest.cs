using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace RetinaTest
{
    [TestClass]
    public class GroupStageTest : RetinaTestBase
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
        public void TestNestedCompoundStages()
        {
            AssertProgram(new TestSuite
            {
                Sources =
                {
                    @"\(`.+",
                    @"$&$&",
                    @"*)T`l`L",
                },
                TestCases = { { "abc", "ABCABC\nabc" } }
            });

            AssertProgram(new TestSuite
            {
                Sources =
                {
                    @"\(`^\w",
                    @"",
                    @"}G`",
                },
                TestCases = { { "abc!", "bc!\nc!\n!\n!\n!" } }
            });


            AssertProgram(new TestSuite
            {
                Sources =
                {
                    @"{`^\w",
                    @"",
                    @"\)G`",
                },
                TestCases = { { "abc!", "!\n" } }
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
                    @"%*(\`$",
                    @"$`",
                    @"\`$",
                    @"$`",
                },
                TestCases = { { "a\nb\nc", "aa\naaaa\nbb\nbbbb\ncc\ncccc\na\nb\nc" } }
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

        [TestMethod]
        public void TestConditional()
        {
            AssertProgram(new TestSuite
            {
                Sources = {
                    @"/\d/(`.+",
                    @"$&$&",
                    @"T`l`L",
                    @"^.",
                    @"$&$&",
                },
                TestCases = {
                    { "123abc", "123abc123abc" },
                    { "defghi", "DDEFGHI" },
                }
            });

            AssertProgram(new TestSuite
            {
                Sources = {
                    @"/\d/^(`.+",
                    @"$&$&",
                    @"T`l`L",
                    @"^.",
                    @"$&$&",
                },
                TestCases = {
                    { "123abc", "1123ABC" },
                    { "defghi", "defghidefghi" },
                }
            });

            AssertProgram(new TestSuite
            {
                Sources = {
                    @"""()""(`.+",
                    @"$&$&",
                    @"T`l`L",
                    @"^.",
                    @"$&$&",
                },
                TestCases = {
                    { "()abc", "()abc()abc" },
                    { "defghi", "DDEFGHI" },
                }
            });

            AssertProgram(new TestSuite
            {
                Sources = {
                    @"""()""^(`.+",
                    @"$&$&",
                    @"T`l`L",
                    @"^.",
                    @"$&$&",
                },
                TestCases = {
                    { "()abc", "(()ABC" },
                    { "defghi", "defghidefghi" },
                }
            });
        }

        [TestMethod]
        public void TestRandom()
        {
            AssertRandomProgram(new RandomTestSuite
            {
                Sources = {
                    @"?(C`.",
                    @"G`\w",
                    @"O`",
                    @"\d",
                    @"*!,",
                },
                TestCases = { { "abc\n123\n<!>", new string[]
                {
                    "9",
                    "abc\n123",
                    "123\n<!>\nabc",
                    "abc\n!,!!,!!!,\n<!>",
                } } }
            });
        }
    }
}
