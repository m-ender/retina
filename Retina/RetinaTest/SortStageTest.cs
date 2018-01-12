using System;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Retina;
using Retina.Stages;
using System.Collections.Generic;

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
        public void TestNumericSorting()
        {
            AssertProgram(new TestSuite { Sources = { "N`.+" }, TestCases = { { "15ABC\nA1B16\nab0\n-a2bc\n-1xyz\nX-24YZ", "X-24YZ\n-1xyz\nab0\nA1B16\n-a2bc\n15ABC" } } });
            AssertProgram(new TestSuite { Sources = { "N`." }, TestCases = { { "06*/50,(3-", "0*/0,(-356" } } });
        }

        [TestMethod]
        public void TestReverseSorting()
        {
            AssertProgram(new TestSuite { Sources = { "O^`.+" }, TestCases = { { "ABC\nAB\nab\nabc\nxyz\nXYZ", "xyz\nabc\nab\nXYZ\nABC\nAB" } } });
            AssertProgram(new TestSuite { Sources = { "O^`." }, TestCases = { { "gHVs5#p.U#", "spgVUH5.##" } } });
            AssertProgram(new TestSuite { Sources = { "N^`.+" }, TestCases = { { "15ABC\nA1B16\nab0\n-a2bc\n-1xyz\nX-24YZ", "15ABC\n-a2bc\nA1B16\nab0\n-1xyz\nX-24YZ" } } });
            AssertProgram(new TestSuite { Sources = { "N^`." }, TestCases = { { "06*/50,(3-", "653-(,0/*0" } } });
        }

        [TestMethod]
        public void TestSubstitution()
        {
            AssertProgram(new TestSuite { Sources = { "N$`.+", "$.&" }, TestCases = { { "ABC\nAB\nab\nabc\nxyz\nXYZ", "AB\nab\nABC\nabc\nxyz\nXYZ" } } });
            AssertProgram(new TestSuite { Sources = { "O$`.(.)", "$1" }, TestCases = { { "J(EM-+Fq_wW,CVDF|%9Q", "|%J(-+W,DFEM9QCVFq_w" } } });
        }

        [TestMethod]
        public void TestOverlappingMatches()
        {
            AssertProgram(new TestSuite { Sources = { @"Ov`.+" }, TestCases = { { "dcba", "abacbadcba" } } });
            AssertProgram(new TestSuite { Sources = { @"Ow`.+" }, TestCases = { { "dcba", "abbaccbcbaddcdcbdcba" } } });
        }

        [TestMethod]
        public void TestRandom()
        {
            AssertRandomProgram(new RandomTestSuite
            {
                Sources = { @"O?`\w+" },
                TestCases = { { "abc\ndef\nghi", new string[]
                {
                    "abc\ndef\nghi",
                    "abc\nghi\ndef",
                    "def\nabc\nghi",
                    "ghi\nabc\ndef",
                    "def\nghi\nabc",
                    "ghi\ndef\nabc",
                }
                } }
            });
        }
    }
}
