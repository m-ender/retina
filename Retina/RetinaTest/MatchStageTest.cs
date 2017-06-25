using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RetinaTest
{
    [TestClass]
    public class MatchStageTest : RetinaTestBase
    {
        [TestMethod]
        public void TestBasicCounting()
        {
            AssertProgram(new TestSuite
            {
                Sources = { @"\w+" },
                TestCases = {
                    { "Hello, World!", "2" },
                    { "(~^.^)~", "0" },
                    { "ab cd ef gh ij kl mn op qr st uv wx yz", "13" },
                }
            });

            AssertProgram(new TestSuite
            {
                Sources = { @"" },
                TestCases = {
                    { "", "1" },
                    { "abc", "4" },
                }
            });

            AssertProgram(new TestSuite
            {
                Sources = { @".*" },
                TestCases = {
                    { "", "1" },
                    { "abc", "2" },
                    { "abc\n\ndef", "5" },
                }
            });
        }

        [TestMethod]
        public void TestBasicPrinting()
        {
            AssertProgram(new TestSuite
            {
                Sources = { @"!`\w+" },
                TestCases = {
                    { "abc!", "abc" },
                    { "Hello, World!", "Hello\nWorld" },
                    { "(~^.^)~", "" },
                }
            });

            AssertProgram(new TestSuite
            {
                Sources = { @"!`\b\w*" },
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
                Sources = { @"r!`\w+" },
                TestCases = {
                    { "abc!", "abc" },
                    { "Hello, World!", "World\nHello" },
                    { "(~^.^)~", "" },
                }
            });

            AssertProgram(new TestSuite
            {
                Sources = { @"r!`\w*\b" },
                TestCases = {
                    { "abc!", "abc\n" },
                    { "Hello, World!", "World\n\nHello\n" },
                    { "(~^.^)~", "" },
                }
            });
        }

        [TestMethod]
        public void TestUniqueMatches()
        {
            AssertProgram(new TestSuite { Sources = { @"!@`." }, TestCases = { { "abacbcedef", "a\nb\nc\ne\nd\nf" } } });
            AssertProgram(new TestSuite { Sources = { @"r!@`." }, TestCases = { { "abacbcedef", "f\ne\nd\nc\nb\na" } } });
        }

        [TestMethod]
        public void TestOverlappingMatches()
        {
            AssertProgram(new TestSuite { Sources = { @"!&`.+" }, TestCases = { { "abcd", "abcd\nbcd\ncd\nd" } } });
            AssertProgram(new TestSuite { Sources = { @"r!&`.+" }, TestCases = { { "abcd", "abcd\nabc\nab\na" } } });
        }
    }
}
