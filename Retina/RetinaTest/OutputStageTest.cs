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
            // > on the last stage should be swallowed by implicit print.
            AssertProgram(new TestSuite
            {
                Sources = { "G`", ">G`" },
                TestCases = { { "Hello, World!", "Hello, World!" } }
            });

            // > anywhere else should print the intermediate result of that stage
            AssertProgram(new TestSuite
            {
                Sources = { ">O`.", "G`" },
                TestCases = { { "Hello, World!", " !,HWdellloor !,HWdellloor" } }
            });
        }

        [TestMethod]
        public void TestPrePrint()
        {
            // < prints before the stage changes the string and does not replace
            // the implicit output.
            AssertProgram(new TestSuite
            {
                Sources = { @"<`\W", "" },
                TestCases = { { "Hello, World!", "Hello, World!HelloWorld" } }
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
            // \
            AssertProgram(new TestSuite
            {
                Sources = { @"\G`", @"G`" },
                TestCases = { { "Hello, World!", "Hello, World!\nHello, World!" } }
            });
            AssertProgram(new TestSuite
            {
                Sources = { @"G`", @"\G`" },
                TestCases = { { "Hello, World!", "Hello, World!\n" } }
            });

            // >
            AssertProgram(new TestSuite
            {
                Sources = { "\n>G`", @"G`" },
                TestCases = { { "Hello, World!", "Hello, World!\nHello, World!" } }
            });
            AssertProgram(new TestSuite
            {
                Sources = { @"G`", "\n>G`" },
                TestCases = { { "Hello, World!", "Hello, World!\n" } }
            });

            // <
            AssertProgram(new TestSuite
            {
                Sources = { "\n<`\\W", "" },
                TestCases = { { "Hello, World!", "Hello, World!\nHelloWorld" } }
            });


            // ;
            AssertProgram(new TestSuite
            {
                Sources = { @"G`", "\n;G`" },
                TestCases = { { "Hello, World!", "" } }
            });
            AssertProgram(new TestSuite
            {
                Sources = { @"G`", "\n;O`." },
                TestCases = { { "Hello, World!", " !,HWdellloor\n" } }
            });

            AssertProgram(new TestSuite
            {
                Sources = { "\n;G`", @"G`" },
                TestCases = { { "Hello, World!", "Hello, World!" } }
            });
            AssertProgram(new TestSuite
            {
                Sources = { "\n;O`.", @"G`" },
                TestCases = { { "Hello, World!", " !,HWdellloor\n !,HWdellloor" } }
            });
        }

        [TestMethod]
        public void TestCustomTerminator()
        {
            AssertProgram(new TestSuite
            {
                Sources = { @"'!>G`" },
                TestCases = { { "Hello, World", "Hello, World!" } }
            });

            AssertProgram(new TestSuite
            {
                Sources = { @"'!\G`" },
                TestCases = { { "Hello, World", "Hello, World!" } }
            });

            AssertProgram(new TestSuite
            {
                Sources = { @"""World!""\G`" },
                TestCases = { { "Hello, ", "Hello, World!" } }
            });

            AssertProgram(new TestSuite
            {
                Sources = { @"'!<`.+", "abc" },
                TestCases = { { "Hello, World", "Hello, World!abc" } }
            });
            
            AssertProgram(new TestSuite
            {
                Sources = { @"'!;G`" },
                TestCases = { { "Hello, World", "" } }
            });
            AssertProgram(new TestSuite
            {
                Sources = { @"'!;O`." },
                TestCases = { { "Hello, World", " ,HWdellloor!" } }
            });
        }

        [TestMethod]
        public void TestNestedOutput()
        {
            AssertProgram(new TestSuite
            {
                Sources = { @"G`", @">>G`" },
                TestCases = { { "Hello, World!", "Hello, World!Hello, World!" } }
            });
            AssertProgram(new TestSuite
            {
                Sources = { @">>G`", @"G`" },
                TestCases = { { "Hello, World!", "Hello, World!Hello, World!Hello, World!" } }
            });
            
            // The order of a preprint and a regular output stage shouldn't matter.
            AssertProgram(new TestSuite
            {
                Sources = { @".<>`\W", "" },
                TestCases = { { "Hello, World!", "Hello, World!HelloWorld" } }
            });
            AssertProgram(new TestSuite
            {
                Sources = { @".><`\W", "" },
                TestCases = { { "Hello, World!", "Hello, World!HelloWorld" } }
            });
        }

        [TestMethod]
        public void TestCharacterLimit()
        {
            AssertProgram(new TestSuite
            {
                Sources = { @"1,2,>G`" },
                TestCases = { { "Hello, World!", "el,Wrd" } }
            });
        }

        [TestMethod]
        public void TestRandomOutput()
        {
            AssertRandomProgram(new RandomTestSuite
            {
                Sources = { "?>`." },
                TestCases = { { "abc", new string[] { "", "3" } } }
            });
        }

        [TestMethod]
        public void TestRandomPrePrint()
        {
            AssertRandomProgram(new RandomTestSuite
            {
                Sources = { "?<`." },
                TestCases = { { "abc", new string[] { "3", "abc3" } } }
            });
        }

        [TestMethod]
        public void TestRandomOutputWithLinefeed()
        {
            AssertRandomProgram(new RandomTestSuite
            {
                Sources = { @"?\`." },
                TestCases = { { "abc", new string[] { "", "3\n" } } }
            });
        }

        [TestMethod]
        public void TestRandomOutputIfChanged()
        {
            AssertRandomProgram(new RandomTestSuite
            {
                Sources = { @"?;`." },
                TestCases = {
                    { "abc", new string[] { "", "3" } },
                    { "1", new string[] { "" } },
                }
            });
        }
    }
}
