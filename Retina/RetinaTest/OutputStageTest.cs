using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RetinaTest
{
    [TestClass]
    public class OutputStageTest : RetinaTestBase
    {
        [TestMethod]
        public void TestDefaultOutput()
        {
            AssertProgram(new TestSuite
            {
                Sources = { "G`", "G`" },
                TestCases = { { "Hello, World!", "Hello, World!" } }
            });
        }

        [TestMethod]
        public void TestSilentFlag()
        {
            // It shouldn't matter where the . flag is used.
            AssertProgram(new TestSuite
            {
                Sources = { ".G`", "G`" },
                TestCases = { { "Hello, World!", "" } }
            });
            AssertProgram(new TestSuite
            {
                Sources = { "G`", ".G`" },
                TestCases = { { "Hello, World!", "" } }
            });
        }
        
        [TestMethod]
        public void TestExplicitOutput()
        {
            // : on the last stage should be swallowed by implicit print.
            AssertProgram(new TestSuite
            {
                Sources = { "G`", ":G`" },
                TestCases = { { "Hello, World!", "Hello, World!" } }
            });

            // : anywhere else should print the intermediate result of that stage
            AssertProgram(new TestSuite
            {
                Sources = { ":O`.", "G`" },
                TestCases = { { "Hello, World!", " !,HWdellloor !,HWdellloor" } }
            });
        }

        [TestMethod]
        public void TestPrintOnlyIfChanged()
        {
            // ; on the last stage should replace the implicit :
            AssertProgram(new TestSuite
            {
                Sources = { "G`", ";G`" },
                TestCases = { { "Hello, World!", "" } }
            });
            AssertProgram(new TestSuite
            {
                Sources = { "G`", ";O`." },
                TestCases = { { "Hello, World!", " !,HWdellloor" } }
            });


            AssertProgram(new TestSuite
            {
                Sources = { ";G`", "G`" },
                TestCases = { { "Hello, World!", "Hello, World!" } }
            });
            AssertProgram(new TestSuite
            {
                Sources = { ";O`.", "G`" },
                TestCases = { { "Hello, World!", " !,HWdellloor !,HWdellloor" } }
            });
        }

        [TestMethod]
        public void TestTrailingLinefeed()
        {
            // :\
            AssertProgram(new TestSuite
            {
                Sources = { @"G`", @":\G`" },
                TestCases = { { "Hello, World!", "Hello, World!\n" } }
            });
            AssertProgram(new TestSuite
            {
                Sources = { @"G`", @"\G`" },
                TestCases = { { "Hello, World!", "Hello, World!\n" } }
            });


            // \ as a shorthand for :\
            AssertProgram(new TestSuite
            {
                Sources = { @":\G`", @"G`" },
                TestCases = { { "Hello, World!", "Hello, World!\nHello, World!" } }
            });
            AssertProgram(new TestSuite
            {
                Sources = { @"\G`", @"G`" },
                TestCases = { { "Hello, World!", "Hello, World!\nHello, World!" } }
            });


            // ;\
            AssertProgram(new TestSuite
            {
                Sources = { @"G`", @";\G`" },
                TestCases = { { "Hello, World!", "" } }
            });
            AssertProgram(new TestSuite
            {
                Sources = { @"G`", @";\O`." },
                TestCases = { { "Hello, World!", " !,HWdellloor\n" } }
            });

            AssertProgram(new TestSuite
            {
                Sources = { @";\G`", @"G`" },
                TestCases = { { "Hello, World!", "Hello, World!" } }
            });
            AssertProgram(new TestSuite
            {
                Sources = { @";\O`.", @"G`" },
                TestCases = { { "Hello, World!", " !,HWdellloor\n !,HWdellloor" } }
            });
        }

        [TestMethod]
        public void TestNestedOutput()
        {
            AssertProgram(new TestSuite
            {
                Sources = { @"G`", @"::G`" },
                TestCases = { { "Hello, World!", "Hello, World!Hello, World!" } }
            });
            AssertProgram(new TestSuite
            {
                Sources = { @"::G`", @"G`" },
                TestCases = { { "Hello, World!", "Hello, World!Hello, World!Hello, World!" } }
            });
        }
    }
}
