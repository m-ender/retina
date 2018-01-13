using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace RetinaTest
{
    [TestClass]
    public class DryRunStageTest : RetinaTestBase
    {
        [TestMethod]
        public void TestBasicDryRun()
        {
            AssertProgram(new TestSuite { Sources = { @"*\`." }, TestCases = { { "123", "3\n123" } } });
        }

        [TestMethod]
        public void TestConditional()
        {
            AssertProgram(new TestSuite
            {
                Sources = { @"/../*`." },
                TestCases = {
                    { "123", "123" },
                    { "Hello, World!", "13" },
                }
            });

            AssertProgram(new TestSuite
            {
                Sources = { @"/../^*`." },
                TestCases = {
                    { "123", "3" },
                    { "Hello, World!", "Hello, World!" },
                }
            });

            AssertProgram(new TestSuite
            {
                Sources = { @"'3*`." },
                TestCases = {
                    { "1234", "1234" },
                    { "Hello, World!", "13" },
                }
            });

            AssertProgram(new TestSuite
            {
                Sources = { @"'3^*`." },
                TestCases = {
                    { "1234", "4" },
                    { "Hello, World!", "Hello, World!" },
                }
            });
        }
    }
}
