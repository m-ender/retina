using Microsoft.VisualStudio.TestTools.UnitTesting;
using Retina;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetinaTest
{
    [TestClass]
    public class RetinaTestBase
    {
        protected void AssertProgram(string[] sources, string input, string expectedOutput)
        {
            var interpreter = new Interpreter(sources.ToList());
            var actualOutput = new StringWriter();

            interpreter.Execute(input, actualOutput);
            
            Assert.AreEqual(expectedOutput, actualOutput.ToString());
        }
    }
}
