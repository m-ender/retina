using Microsoft.VisualStudio.TestTools.UnitTesting;
using Retina;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RetinaTest
{
    [TestClass]
    public class RetinaTestBase
    {
        internal void AssertProgram(TestSuite testSuite)
        {
            var interpreter = new Interpreter(testSuite.Sources);

            foreach (var testCase in testSuite.TestCases)
            {
                var actualOutput = new StringWriter();
                interpreter.Execute(testCase.Input, actualOutput);

                Assert.AreEqual(testCase.Output, actualOutput.ToString());
            }
        }
        
        internal void AssertRandomProgram(RandomTestSuite testSuite)
        {
            var interpreter = new Interpreter(testSuite.Sources);

            foreach (var testCase in testSuite.TestCases)
            {
                // TODO: Test the actual distribution.

                // Compute number of samples to make the chance of missing an element less than 0.001
                int nOutputs = testCase.Outputs.Count;
                int samples = Math.Max(10, (int)Math.Ceiling(Math.Log(0.001, (nOutputs - 1.0) / nOutputs)));

                var actualOutputs = new HashSet<string>();
                
                for (int i = 0; i < samples; ++i)
                {
                    var actualOutput = new StringWriter();
                    interpreter.Execute(testCase.Input, actualOutput);
                    actualOutputs.Add(actualOutput.ToString());
                }

                CollectionAssert.AreEquivalent(testCase.Outputs, actualOutputs.ToList());
            }
        }
    }
}
