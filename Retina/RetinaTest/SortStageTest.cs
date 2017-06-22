using System;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Retina;
using Retina.Stages;

namespace RetinaTest
{
    [TestClass]
    public class SortStageTest : RetinaTestBase
    {
        [TestMethod]
        public void TestBasicSorting()
        {
            AssertProgram(new TestSuite { Sources = { "O`.+" }, TestCases = { { "ABC\nAB\nab\nabc\nxyz\nXYZ", "AB\nABC\nXYZ\nab\nabc\nxyz" } } });
            AssertProgram(new TestSuite { Sources = { "O`." }, TestCases = { { "pHSyRi|.A7", ".7AHRSipy|" } } });

            // Regex should default to ".+"
            AssertProgram(new TestSuite { Sources = { "O`" }, TestCases = { { "ABC\nAB\nab\nabc\nxyz\nXYZ", "AB\nABC\nXYZ\nab\nabc\nxyz" } } });
        }

        [TestMethod]
        public void TestReverseSorting()
        {
            AssertProgram(new TestSuite { Sources = { "O^`.+" }, TestCases = { { "ABC\nAB\nab\nabc\nxyz\nXYZ", "xyz\nabc\nab\nXYZ\nABC\nAB" } } });
            AssertProgram(new TestSuite { Sources = { "O^`." }, TestCases = { { "gHVs5#p.U#", "spgVUH5.##" } } });
        }

        [TestMethod]
        public void TestNumericalSorting()
        {
            AssertProgram(new TestSuite { Sources = { "O#`.+" }, TestCases = { { "15ABC\nA1B16\nab0\n-a2bc\n-1xyz\nX-24YZ", "X-24YZ\n-1xyz\nab0\nA1B16\n-a2bc\n15ABC" } } });
            AssertProgram(new TestSuite { Sources = { "O#`." }, TestCases = { { "06*/50,(3-", "0*/0,(-356" } } });
        }

        [TestMethod]
        public void TestSubstitution()
        {
            AssertProgram(new TestSuite { Sources = { "O$#`.+", "$.&" }, TestCases = { { "ABC\nAB\nab\nabc\nxyz\nXYZ", "AB\nab\nABC\nabc\nxyz\nXYZ" } } });
            AssertProgram(new TestSuite { Sources = { "O$`.(.)", "$1" }, TestCases = { { "J(EM-+Fq_wW,CVDF|%9Q", "|%J(-+W,DFEM9QCVFq_w" } } });
        }
    }
}
