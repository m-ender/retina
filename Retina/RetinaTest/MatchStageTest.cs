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
    }
}
