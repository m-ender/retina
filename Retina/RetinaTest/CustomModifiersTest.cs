using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RetinaTest
{
    [TestClass]
    public class CustomModifiersTest : RetinaTestBase
    {
        [TestMethod]
        public void TestAnchoring()
        {
            AssertProgram(new TestSuite
            {
                Sources = { @"Ma`\d+" },
                TestCases = {
                    { "123", "123" },
                    { "123\n456", "" },
                    { "a123b", "" },
                }
            });

            AssertProgram(new TestSuite
            {
                Sources = { @"Mma`\d+" },
                TestCases = {
                    { "123", "123" },
                    { "123\n456", "" },
                    { "a123b", "" },
                }
            });


            AssertProgram(new TestSuite
            {
                Sources = { @"Ml`\d+" },
                TestCases = {
                    { "123", "123" },
                    { "123\na789b\n456", "123\n456" },
                    { "a123b", "" },
                }
            });

            AssertProgram(new TestSuite
            {
                Sources = { @"Mml`\d+" },
                TestCases = {
                    { "123", "123" },
                    { "123\na789b\n456", "123\n456" },
                    { "a123b", "" },
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
            AssertProgram(new TestSuite { Sources = { @"Mrw`.+" }, TestCases = { { "abcd", "a\nab\nb\nabc\nbc\nc\nabcd\nbcd\ncd\nd" } } });
        }
    }
}
