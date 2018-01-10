using System;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Retina;
using Retina.Stages;

namespace RetinaTest
{
    [TestClass]
    public class PadStageTest : RetinaTestBase
    {
        [TestMethod]
        public void TestBasicPadding()
        {
            AssertProgram(new TestSuite { Sources = { "P`.+" }, TestCases = { { "ABC\nAB\nab\nabcd\nxyz\nXYZ", "ABC \nAB  \nab  \nabcd\nxyz \nXYZ " } } });
        }

        [TestMethod]
        public void TestLeftPadding()
        {
            AssertProgram(new TestSuite { Sources = { "P^`.+" }, TestCases = { { "ABC\nAB\nab\nabcd\nxyz\nXYZ", " ABC\n  AB\n  ab\nabcd\n xyz\n XYZ" } } });
        }

        [TestMethod]
        public void TestPadBy()
        {
            // Replacements are used to determine the padding width
            AssertProgram(new TestSuite { Sources = { @"P$`.", "*" }, TestCases = { { "1234", "1   2   3   4   " } } });
            // That also means the padding width can be shorter than some matches
            AssertProgram(new TestSuite { Sources = { @"P$`.+", "abc" }, TestCases = { { "ABC\nAB\na\nabcd\nxyz\nXYZ", "ABC\nAB \na  \nabcd\nxyz\nXYZ" } } });
        }

        [TestMethod]
        public void TestPaddingCharacter()
        {
            AssertProgram(new TestSuite { Sources = { "P'_`.+" }, TestCases = { { "ABC\nAB\nab\nabcd\nxyz\nXYZ", "ABC_\nAB__\nab__\nabcd\nxyz_\nXYZ_" } } });
        }

        [TestMethod]
        public void TestPaddingString()
        {
            // The string gets repeated cyclically from the left (or from the right for left padding).
            AssertProgram(new TestSuite { Sources = { @"P"".oOo""`.+" }, TestCases = { { "a\nbc\ndef\n0123456789", "aoOo.oOo.o\nbcOo.oOo.o\ndefo.oOo.o\n0123456789" } } });
            AssertProgram(new TestSuite { Sources = { @"P"".oOo""^`.+" }, TestCases = { { "a\nbc\ndef\n0123456789", "Oo.oOo.oOa\nOo.oOo.obc\nOo.oOo.def\n0123456789" } } });
        }

        [TestMethod]
        public void TestOverlappingMatches()
        {
            AssertProgram(new TestSuite { Sources = { @"Pv`.+" }, TestCases = { { "abcd", "abcdbcd cd  d   " } } });
            AssertProgram(new TestSuite { Sources = { @"Pw`.+" }, TestCases = { { "abcd", "a   ab  abc abcdb   bc  bcd c   cd  d   " } } });
        }
    }
}
