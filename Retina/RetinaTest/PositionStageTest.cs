using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RetinaTest
{
    [TestClass]
    public class PositionStageTest : RetinaTestBase
    {
        [TestMethod]
        public void TestBasicPositions()
        {
            AssertProgram(new TestSuite
            {
                Sources = { @"I`\w+" },
                TestCases = {
                    { "abc!", "0" },
                    { "Hello, World!", "0\n7" },
                    { "(~^.^)~", "" },
                }
            });

            AssertProgram(new TestSuite { Sources = { @"I`" }, TestCases = { { "abcd", "0\n1\n2\n3\n4" } } });
        }

        [TestMethod]
        public void TestRTLMatching()
        {
            AssertProgram(new TestSuite
            {
                Sources = { @"Ir`\w+" },
                TestCases = {
                    { "abc!", "0" },
                    { "Hello, World!", "0\n7" },
                    { "(~^.^)~", "" },
                }
            });

            AssertProgram(new TestSuite { Sources = { @"Ir`" }, TestCases = { { "abcd", "0\n1\n2\n3\n4" } } });
        }

        [TestMethod]
        public void TestOverlappingMatches()
        {
            AssertProgram(new TestSuite { Sources = { @"Iv`.+" }, TestCases = { { "abcd", "0\n1\n2\n3" } } });
            AssertProgram(new TestSuite { Sources = { @"Irv`.+" }, TestCases = { { "abcd", "0\n0\n0\n0" } } });
            AssertProgram(new TestSuite { Sources = { @"Iw`.+" }, TestCases = { { "abcd", "0\n0\n0\n0\n1\n1\n1\n2\n2\n3" } } });
            AssertProgram(new TestSuite { Sources = { @"Irw`.+" }, TestCases = { { "abcd", "0\n0\n1\n0\n1\n2\n0\n1\n2\n3" } } });
        }

        [TestMethod]
        public void TestEndPosition()
        {
            AssertProgram(new TestSuite
            {
                Sources = { @"I^`\w+" },
                TestCases = {
                    { "abc!", "3" },
                    { "Hello, World!", "5\n12" },
                    { "(~^.^)~", "" },
                }
            });

            AssertProgram(new TestSuite { Sources = { @"I^`" }, TestCases = { { "abcd", "0\n1\n2\n3\n4" } } });

            AssertProgram(new TestSuite
            {
                Sources = { @"Ir^`\w+" },
                TestCases = {
                    { "abc!", "3" },
                    { "Hello, World!", "5\n12" },
                    { "(~^.^)~", "" },
                }
            });

            AssertProgram(new TestSuite { Sources = { @"Ir^`" }, TestCases = { { "abcd", "0\n1\n2\n3\n4" } } });

            AssertProgram(new TestSuite { Sources = { @"I^v`.+" }, TestCases = { { "abcd", "4\n4\n4\n4" } } });
            AssertProgram(new TestSuite { Sources = { @"I^rv`.+" }, TestCases = { { "abcd", "1\n2\n3\n4" } } });
            AssertProgram(new TestSuite { Sources = { @"I^w`.+" }, TestCases = { { "abcd", "1\n2\n3\n4\n2\n3\n4\n3\n4\n4" } } });
            AssertProgram(new TestSuite { Sources = { @"I^rw`.+" }, TestCases = { { "abcd", "1\n2\n2\n3\n3\n3\n4\n4\n4\n4" } } });
        }

        [TestMethod]
        public void TestListFormatting()
        {
            AssertProgram(new TestSuite { Sources = { @"I['[|"", ""]']`." }, TestCases = { { "abcd", "[0, 1, 2, 3]" } } });
        }
    }
}
