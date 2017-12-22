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
        public void TestUniqueMatches()
        {
            AssertProgram(new TestSuite { Sources = { @"Mq`." }, TestCases = { { "abacbcedef", "a\nb\nc\ne\nd\nf" } } });
            AssertProgram(new TestSuite { Sources = { @"Mp`." }, TestCases = { { "abacbcedef", "a\nb\nc\nd\ne\nf" } } });
            AssertProgram(new TestSuite { Sources = { @"Mqr`." }, TestCases = { { "abacbcedef", "a\nb\nc\nd\ne\nf" } } });
            AssertProgram(new TestSuite { Sources = { @"Mpr`." }, TestCases = { { "abacbcedef", "a\nb\nc\ne\nd\nf" } } });
        }

        [TestMethod]
        public void TestOverlappingMatches()
        {
            AssertProgram(new TestSuite { Sources = { @"Mv`.+" }, TestCases = { { "abcd", "abcd\nbcd\ncd\nd" } } });
            AssertProgram(new TestSuite { Sources = { @"Mrv`.+" }, TestCases = { { "abcd", "a\nab\nabc\nabcd" } } });
            AssertProgram(new TestSuite { Sources = { @"Mw`.+" }, TestCases = { { "abcd", "a\nab\nabc\nabcd\nb\nbc\nbcd\nc\ncd\nd" } } });
        }
    }
}
