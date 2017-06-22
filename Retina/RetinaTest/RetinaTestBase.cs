using Microsoft.VisualStudio.TestTools.UnitTesting;
using Retina;
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
    }
}
