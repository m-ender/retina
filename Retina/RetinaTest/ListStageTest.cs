using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RetinaTest
{
    [TestClass]
    public class ListStageTest : RetinaTestBase
    {
        [TestMethod]
        public void TestBasicList()
        {
            AssertProgram(new TestSuite
            {
                Sources = { @"L`\w+" },
                TestCases = {
                    { "abc!", "abc" },
                    { "Hello, World!", "Hello\nWorld" },
                    { "(~^.^)~", "" },
                }
            });

            AssertProgram(new TestSuite
            {
                Sources = { @"L`\b\w*" },
                TestCases = {
                    { "abc!", "abc\n" },
                    { "Hello, World!", "Hello\n\nWorld\n" },
                    { "(~^.^)~", "" },
                }
            });
        }

        [TestMethod]
        public void TestRTLMatching()
        {
            AssertProgram(new TestSuite
            {
                Sources = { @"Lr`\w+" },
                TestCases = {
                    { "abc!", "abc" },
                    { "Hello, World!", "Hello\nWorld" },
                    { "(~^.^)~", "" },
                }
            });

            AssertProgram(new TestSuite
            {
                Sources = { @"Lr`\w*\b" },
                TestCases = {
                    { "abc!", "\nabc" },
                    { "Hello, World!", "\nHello\n\nWorld" },
                    { "(~^.^)~", "" },
                }
            });
        }

        [TestMethod]
        public void TestOverlappingMatches()
        {
            AssertProgram(new TestSuite { Sources = { @"Lv`.+" }, TestCases = { { "abcd", "abcd\nbcd\ncd\nd" } } });
            AssertProgram(new TestSuite { Sources = { @"Lrv`.+" }, TestCases = { { "abcd", "a\nab\nabc\nabcd" } } });
            AssertProgram(new TestSuite { Sources = { @"Lw`.+" }, TestCases = { { "abcd", "a\nab\nabc\nabcd\nb\nbc\nbcd\nc\ncd\nd" } } });
            AssertProgram(new TestSuite { Sources = { @"Lrw`.+" }, TestCases = { { "abcd", "a\nab\nb\nabc\nbc\nc\nabcd\nbcd\ncd\nd" } } });
            AssertProgram(new TestSuite { Sources = { @"Lw`(?<=\d).+(?=\d)" }, TestCases = { { "ab1cd2ef3gh4ij", "cd\ncd2ef\ncd2ef3gh\nef\nef3gh\ngh" } } });
            AssertProgram(new TestSuite { Sources = { @"Lrw`(?<=\d).+(?=\d)" }, TestCases = { { "ab1cd2ef3gh4ij", "cd\ncd2ef\nef\ncd2ef3gh\nef3gh\ngh" } } });
        }

        [TestMethod]
        public void TestReverse()
        {
            AssertProgram(new TestSuite
            {
                Sources = { @"L^`\w+" },
                TestCases = {
                    { "abc!", "abc" },
                    { "Hello, World!", "World\nHello" },
                    { "(~^.^)~", "" },
                }
            });

            AssertProgram(new TestSuite
            {
                Sources = { @"L^`\b\w*" },
                TestCases = {
                    { "abc!", "\nabc" },
                    { "Hello, World!", "\nWorld\n\nHello" },
                    { "(~^.^)~", "" },
                }
            });


            AssertProgram(new TestSuite
            {
                Sources = { @"L^r`\w+" },
                TestCases = {
                    { "abc!", "abc" },
                    { "Hello, World!", "World\nHello" },
                    { "(~^.^)~", "" },
                }
            });

            AssertProgram(new TestSuite
            {
                Sources = { @"L^r`\w*\b" },
                TestCases = {
                    { "abc!", "abc\n" },
                    { "Hello, World!", "World\n\nHello\n" },
                    { "(~^.^)~", "" },
                }
            });

            AssertProgram(new TestSuite { Sources = { @"L^v`.+" }, TestCases = { { "abcd", "d\ncd\nbcd\nabcd" } } });
            AssertProgram(new TestSuite { Sources = { @"L^rv`.+" }, TestCases = { { "abcd", "abcd\nabc\nab\na" } } });
            AssertProgram(new TestSuite { Sources = { @"L^w`.+" }, TestCases = { { "abcd", "d\ncd\nc\nbcd\nbc\nb\nabcd\nabc\nab\na" } } });
            AssertProgram(new TestSuite { Sources = { @"L^rw`.+" }, TestCases = { { "abcd", "d\ncd\nbcd\nabcd\nc\nbc\nabc\nb\nab\na" } } });
            AssertProgram(new TestSuite { Sources = { @"L^w`(?<=\d).+(?=\d)" }, TestCases = { { "ab1cd2ef3gh4ij", "gh\nef3gh\nef\ncd2ef3gh\ncd2ef\ncd" } } });
            AssertProgram(new TestSuite { Sources = { @"L^rw`(?<=\d).+(?=\d)" }, TestCases = { { "ab1cd2ef3gh4ij", "gh\nef3gh\ncd2ef3gh\nef\ncd2ef\ncd" } } });
        }

        [TestMethod]
        public void TestReplacement()
        {
            AssertProgram(new TestSuite
            {
                Sources = { @"L$`\w+", "$.&,$&" },
                TestCases = {
                    { "abc!", "3,abc" },
                    { "Hello, World!", "5,Hello\n5,World" },
                    { "(~^.^)~", "" },
                }
            });
        }

        [TestMethod]
        public void TestCharacterLimit()
        {
            AssertProgram(new TestSuite
            {
                Sources = { @"L, 1,3`\w+" },
                TestCases = {
                    { "abc!", "bc" },
                    { "Hello, World!", "ell\norl" },
                    { "(~^.^)~", "" },
                }
            });

            AssertProgram(new TestSuite
            {
                Sources = { @"L, 1,-2`\w+" },
                TestCases = {
                    { "abc!", "b" },
                    { "Hello, World!", "ell\norl" },
                    { "(~^.^)~", "" },
                }
            });

            AssertProgram(new TestSuite
            {
                Sources = { @"L$, 1,5`\w+", "$&$&" },
                TestCases = {
                    { "abc!", "bcabc" },
                    { "Hello, World!", "elloH\norldW" },
                    { "(~^.^)~", "" },
                }
            });

            AssertProgram(new TestSuite
            {
                Sources = { @"L$, 1,-5`\w+", "$&$&" },
                TestCases = {
                    { "abc!", "b" },
                    { "Hello, World!", "elloH\norldW" },
                    { "(~^.^)~", "" },
                }
            });
        }

        [TestMethod]
        public void TestListFormatting()
        {
            AssertProgram(new TestSuite { Sources = { @"L['[|"", ""]']`." }, TestCases = { { "abcd", "[a, b, c, d]" } } });
        }
    }
}
