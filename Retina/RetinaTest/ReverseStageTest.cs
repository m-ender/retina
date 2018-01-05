using System;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Retina;
using Retina.Stages;

namespace RetinaTest
{
    [TestClass]
    public class ReverseStageTest : RetinaTestBase
    {
        [TestMethod]
        public void TestBasicReversal()
        {
            AssertProgram(new TestSuite { Sources = { "V`.+" }, TestCases = { { "ABC\nAB\nab\nabc\nxyz\nXYZ", "CBA\nBA\nba\ncba\nzyx\nZYX" } } });
            AssertProgram(new TestSuite { Sources = { "V`." }, TestCases = { { "pHSyRi|.A7", "pHSyRi|.A7" } } });

            // Regex should default to "(?m:^.*$)"
            AssertProgram(new TestSuite { Sources = { "V`" }, TestCases = { { "ABC\nAB\nab\nabc\nxyz\nXYZ", "CBA\nBA\nba\ncba\nzyx\nZYX" } } });
        }

        [TestMethod]
        public void TestReverseOption()
        {
            // Reverses the list of matches as well.
            AssertProgram(new TestSuite { Sources = { "V^`.+" }, TestCases = { { "ABC\nAB\nab\nabc\nxyz\nXYZ", "ZYX\nzyx\ncba\nba\nBA\nCBA" } } });
            AssertProgram(new TestSuite { Sources = { "V^`." }, TestCases = { { "pHSyRi|.A7", "7A.|iRySHp" } } });
        }

        [TestMethod]
        public void TestSubstitution()
        {
            // The replacement gets reversed instead.
            AssertProgram(new TestSuite { Sources = { @"V$`\w(\w+)", "$1" }, TestCases = { { "Hello, World!", "olle, dlro!" } } });
        }

        [TestMethod]
        public void TestOverlappingMatches()
        {
            AssertProgram(new TestSuite { Sources = { @"Vv`.+" }, TestCases = { { "abcd", "dcbadcbdcd" } } });
            AssertProgram(new TestSuite { Sources = { @"Vw`.+" }, TestCases = { { "abcd", "abacbadcbabcbdcbcdcd" } } });
        }

        [TestMethod]
        public void TestCharacterLimit()
        {
            AssertProgram(new TestSuite { Sources = { @"V, 1,3,`\w+" }, TestCases = { { "HelloWorld, shenanigans", "HrlloWoeld, ssengniaanh" } } });
            AssertProgram(new TestSuite { Sources = { @"V, ~1,3,`\w+" }, TestCases = { { "HelloWorld, shenanigans", "delooWlrlH, nhaianngess" } } });
        }
    }
}
