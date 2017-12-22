using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RetinaTest
{
    [TestClass]
    public class CountStageTest : RetinaTestBase
    {
        [TestMethod]
        public void TestBasicCounting()
        {
            AssertProgram(new TestSuite
            {
                Sources = { @"\w+" },
                TestCases = {
                    { "Hello, World!", "2" },
                    { "(~^.^)~", "0" },
                    { "ab cd ef gh ij kl mn op qr st uv wx yz", "13" },
                }
            });

            AssertProgram(new TestSuite
            {
                Sources = { @"" },
                TestCases = {
                    { "", "1" },
                    { "abc", "4" },
                }
            });

            AssertProgram(new TestSuite
            {
                Sources = { @".*" },
                TestCases = {
                    { "", "1" },
                    { "abc", "2" },
                    { "abc\n\ndef", "5" },
                }
            });
        }
    }
}
