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
    }
}
