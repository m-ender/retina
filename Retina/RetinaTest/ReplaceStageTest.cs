using System;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Retina;
using Retina.Stages;
using System.Collections.Generic;

namespace RetinaTest
{
    [TestClass]
    public class ReplaceStageTest : RetinaTestBase
    {
        [TestMethod]
        public void TestDefaultSubstitution()
        {
            // If there is no source for the substitution, it should default to an empty string.
            AssertProgram(new TestSuite { Sources = { @"R`\d+" }, TestCases = { { "ab12cd34ef56gh78ij", "abcdefghij" } } });
        }

        [TestMethod]
        public void TestOverlappingMatches()
        {
            AssertProgram(new TestSuite { Sources = { "v`.+", "$&" }, TestCases = { { "abcd", "abcdbcdcdd" } } });
            AssertProgram(new TestSuite { Sources = { "w`.+", "$&" }, TestCases = { { "abcd", "aababcabcdbbcbcdccdd" } } });
            AssertProgram(new TestSuite { Sources = { "w`.+", "$.`" }, TestCases = { { "abcd", "0000111223" } } });
            AssertProgram(new TestSuite { Sources = { "wr`.+", "$.`" }, TestCases = { { "abcd", "0010120123" } } });
            AssertProgram(new TestSuite { Sources = { @"w`\d+", "$&" }, TestCases = { { "ab12cd34ef56gh78ij", "ab1122cd3344ef5566gh7788ij" } } });
        }

        [TestMethod]
        public void TestReverse()
        {
            AssertProgram(new TestSuite { Sources = { @"^`\w+", "$.&" }, TestCases = { { "<a,bc;def>", "<3,2;1>" } } });

        }
    }
}
