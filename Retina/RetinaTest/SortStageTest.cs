using System;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Retina;
using Retina.Stages;

namespace RetinaTest
{
    [TestClass]
    public class SortStageTest
    {
        [TestMethod]
        public void TestBasicSorting()
        {
            AssertSort(".+", "$&", "", "ABC\nAB\nab\nabc\nxyz\nXYZ", "AB\nABC\nXYZ\nab\nabc\nxyz");
            AssertSort(".", "$&", "", "pHSyRi|.A7", ".7AHRSipy|");
        }

        [TestMethod]
        public void TestReverseSorting()
        {
            AssertSort(".+", "$&", "^", "ABC\nAB\nab\nabc\nxyz\nXYZ", "xyz\nabc\nab\nXYZ\nABC\nAB");
            AssertSort(".", "$&", "^", "gHVs5#p.U#", "spgVUH5.##");
        }

        [TestMethod]
        public void TestNumericalSorting()
        {
            AssertSort(".+", "$&", "#", "15ABC\nA1B16\nab0\n-a2bc\n-1xyz\nX-24YZ", "X-24YZ\n-1xyz\nab0\nA1B16\n-a2bc\n15ABC");
            AssertSort(".", "$&", "#", "06*/50,(3-", "0*/0,(-356");
        }

        [TestMethod]
        public void TestSubstitution()
        {
            AssertSort(".+", "$.&", "$#", "ABC\nAB\nab\nabc\nxyz\nXYZ", "AB\nab\nABC\nabc\nxyz\nXYZ");
            AssertSort(".(.)", "$1", "$", "J(EM-+Fq_wW,CVDF|%9Q", "|%J(-+W,DFEM9QCVFq_w");
        }

        private void AssertSort(string regex, string replacement, string optionsString, string input, string expectedOutput)
        {
            // optionsString should only contain a subset of "#^ceimnrsx"
            var options = new Options(optionsString, Modes.Sort);
            options.Silent = true;

            var stage = new SortStage(options, regex, replacement);

            Assert.AreEqual(expectedOutput, stage.Execute(input).ToString());
        }
    }
}
