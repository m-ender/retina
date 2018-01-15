using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace RetinaTest
{
    [TestClass]
    public class ConstantStageTest : RetinaTestBase
    {
        [TestMethod]
        public void TestConstantStage()
        {
            AssertProgram(new TestSuite
            {
                Sources = { "K`)$+(" },
                TestCases = {
                    { "abc", ")$+(" },
                    { "", ")$+(" },
                    { "123\n456", ")$+(" },
                }
            });
        }

        [TestMethod]
        public void TestConditional()
        {
            AssertProgram(new TestSuite
            {
                Sources = { @"K/\d/`)$+(" },
                TestCases = {
                    { "abc", "abc" },
                    { "", "" },
                    { "123\n456", ")$+(" },
                }
            });

            AssertProgram(new TestSuite
            {
                Sources = { @"K'3`)$+(" },
                TestCases = {
                    { "12", "12" },
                    { "", "" },
                    { "123\n456", ")$+(" },
                }
            });

            AssertProgram(new TestSuite
            {
                Sources = { @"K""23""`)$+(" },
                TestCases = {
                    { "32", "32" },
                    { "", "" },
                    { "123\n456", ")$+(" },
                }
            });
        }
    }
}
