using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RetinaTest
{
    [TestClass]
    public class MatchStageTest : RetinaTestBase
    {
        [TestMethod]
        public void TestBasicPrinting()
        {
            AssertProgram(new TestSuite
            {
                Sources = { @"M`\w+" },
                TestCases = {
                    { "abc!", "abc" },
                    { "Hello, World!", "Hello\nWorld" },
                    { "(~^.^)~", "" },
                }
            });

            AssertProgram(new TestSuite
            {
                Sources = { @"M`\b\w*" },
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
                Sources = { @"Mr`\w+" },
                TestCases = {
                    { "abc!", "abc" },
                    { "Hello, World!", "Hello\nWorld" },
                    { "(~^.^)~", "" },
                }
            });

            AssertProgram(new TestSuite
            {
                Sources = { @"Mr`\w*\b" },
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
            AssertProgram(new TestSuite { Sources = { @"Mv`.+" }, TestCases = { { "abcd", "abcd\nbcd\ncd\nd" } } });
            AssertProgram(new TestSuite { Sources = { @"Mrv`.+" }, TestCases = { { "abcd", "a\nab\nabc\nabcd" } } });
            AssertProgram(new TestSuite { Sources = { @"Mw`.+" }, TestCases = { { "abcd", "a\nab\nabc\nabcd\nb\nbc\nbcd\nc\ncd\nd" } } });
            AssertProgram(new TestSuite { Sources = { @"Mrw`.+" }, TestCases = { { "abcd", "a\nab\nb\nabc\nbc\nc\nabcd\nbcd\ncd\nd" } } });
            AssertProgram(new TestSuite { Sources = { @"Mw`(?<=\d).+(?=\d)" }, TestCases = { { "ab1cd2ef3gh4ij", "cd\ncd2ef\ncd2ef3gh\nef\nef3gh\ngh" } } });
            AssertProgram(new TestSuite { Sources = { @"Mrw`(?<=\d).+(?=\d)" }, TestCases = { { "ab1cd2ef3gh4ij", "cd\ncd2ef\nef\ncd2ef3gh\nef3gh\ngh" } } });
        }

        [TestMethod]
        public void TestReverse()
        {
            AssertProgram(new TestSuite
            {
                Sources = { @"M^`\w+" },
                TestCases = {
                    { "abc!", "abc" },
                    { "Hello, World!", "World\nHello" },
                    { "(~^.^)~", "" },
                }
            });

            AssertProgram(new TestSuite
            {
                Sources = { @"M^`\b\w*" },
                TestCases = {
                    { "abc!", "\nabc" },
                    { "Hello, World!", "\nWorld\n\nHello" },
                    { "(~^.^)~", "" },
                }
            });


            AssertProgram(new TestSuite
            {
                Sources = { @"M^r`\w+" },
                TestCases = {
                    { "abc!", "abc" },
                    { "Hello, World!", "World\nHello" },
                    { "(~^.^)~", "" },
                }
            });

            AssertProgram(new TestSuite
            {
                Sources = { @"M^r`\w*\b" },
                TestCases = {
                    { "abc!", "abc\n" },
                    { "Hello, World!", "World\n\nHello\n" },
                    { "(~^.^)~", "" },
                }
            });

            AssertProgram(new TestSuite { Sources = { @"M^v`.+" }, TestCases = { { "abcd", "d\ncd\nbcd\nabcd" } } });
            AssertProgram(new TestSuite { Sources = { @"M^rv`.+" }, TestCases = { { "abcd", "abcd\nabc\nab\na" } } });
            AssertProgram(new TestSuite { Sources = { @"M^w`.+" }, TestCases = { { "abcd", "d\ncd\nc\nbcd\nbc\nb\nabcd\nabc\nab\na" } } });
            AssertProgram(new TestSuite { Sources = { @"M^rw`.+" }, TestCases = { { "abcd", "d\ncd\nbcd\nabcd\nc\nbc\nabc\nb\nab\na" } } });
            AssertProgram(new TestSuite { Sources = { @"M^w`(?<=\d).+(?=\d)" }, TestCases = { { "ab1cd2ef3gh4ij", "gh\nef3gh\nef\ncd2ef3gh\ncd2ef\ncd" } } });
            AssertProgram(new TestSuite { Sources = { @"M^rw`(?<=\d).+(?=\d)" }, TestCases = { { "ab1cd2ef3gh4ij", "gh\nef3gh\ncd2ef3gh\nef\ncd2ef\ncd" } } });
        }
    }
}
