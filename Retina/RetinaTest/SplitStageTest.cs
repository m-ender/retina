using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace RetinaTest
{
    [TestClass]
    public class SplitStageTest : RetinaTestBase
    {
        [TestMethod]
        public void TestBasicSplitting()
        {
            AssertProgram(new TestSuite { Sources = { @"S`\W+" }, TestCases = { { "Hello, World!", "Hello\nWorld\n" } } });
            AssertProgram(new TestSuite { Sources = { @"S`\W*" }, TestCases = { { "Hello, World!", "\nH\ne\nl\nl\no\n\nW\no\nr\nl\nd\n\n" } } });
            AssertProgram(new TestSuite { Sources = { @"S`\W" }, TestCases = { { "Hello, World!", "Hello\n\nWorld\n" } } });
        }

        [TestMethod]
        public void TestCapturingGroups()
        {
            AssertProgram(new TestSuite { Sources = { @"S`(\W)+" }, TestCases = { { "Hello, World!", "Hello\n \nWorld\n!\n" } } });
            AssertProgram(new TestSuite { Sources = { @"S`(.o)(.)" }, TestCases = { { "Hello, World!", "Hel\nlo\n,\n \nWo\nr\nld!" } } });
            AssertProgram(new TestSuite { Sources = { @"S`(\b|l)" }, TestCases = { { "Hello, World!", "\n\nHe\nl\n\nl\no\n\n, \n\nWor\nl\nd\n\n!" } } });
        }

        [TestMethod]
        public void TestWithoutCapturingGroups()
        {
            AssertProgram(new TestSuite { Sources = { @"S!-`(\W)+" }, TestCases = { { "Hello, World!", "Hello\nWorld\n" } } });
            AssertProgram(new TestSuite { Sources = { @"S!-`(.o)(.)" }, TestCases = { { "Hello, World!", "Hel\n \nld!" } } });
            AssertProgram(new TestSuite { Sources = { @"S!-`(\b|l)" }, TestCases = { { "Hello, World!", "\nHe\n\no\n, \nWor\nd\n!" } } });
        }

        [TestMethod]
        public void TestWithoutEmptySegments()
        {
            AssertProgram(new TestSuite { Sources = { @"S!_`\W+" }, TestCases = { { "Hello, World!", "Hello\nWorld" } } });
            AssertProgram(new TestSuite { Sources = { @"S!_`(\W)+" }, TestCases = { { "Hello, World!", "Hello\n \nWorld\n!" } } });
            AssertProgram(new TestSuite { Sources = { @"S!_`(.o)(.)" }, TestCases = { { "Hello, World!", "Hel\nlo\n,\n \nWo\nr\nld!" } } });
            AssertProgram(new TestSuite { Sources = { @"S!_`(\b|l)" }, TestCases = { { "Hello, World!", "\nHe\nl\nl\no\n\n, \n\nWor\nl\nd\n\n!" } } });
        }

        [TestMethod]
        public void TestOverlappingMatches()
        {
            AssertProgram(new TestSuite { Sources = { @"Sw`(?<=\d).+(?=\d)" }, TestCases = { { "ab1cd2ef3gh4ij", "ab1\n\n\n\n\n\n4ij" } } });
            AssertProgram(new TestSuite { Sources = { @"Srw`(?<=\d).+(?=\d)" }, TestCases = { { "ab1cd2ef3gh4ij", "ab1\n\n\n\n\n\n4ij" } } });
            AssertProgram(new TestSuite { Sources = { @"Sw`\d+" }, TestCases = { { "ab12cd34ef", "ab\n\n\ncd\n\n\nef" } } });
            AssertProgram(new TestSuite { Sources = { @"Sw`1..|2|34" }, TestCases = { { "ab12cd34ef", "ab\n\ncd\nef" } } });
            AssertProgram(new TestSuite { Sources = { @"Srw`1..|2|34" }, TestCases = { { "ab12cd34ef", "ab1\n\nd\nef" } } });
        }

        [TestMethod]
        public void TestReverse()
        {
            AssertProgram(new TestSuite { Sources = { @"S^`\W+" }, TestCases = { { "Hello, World!", "\nWorld\nHello" } } });
            AssertProgram(new TestSuite { Sources = { @"S^`(\W)+" }, TestCases = { { "Hello, World!", "\n!\nWorld\n \nHello" } } });
            AssertProgram(new TestSuite { Sources = { @"S^!-`(\W)+" }, TestCases = { { "Hello, World!", "\nWorld\nHello" } } });
            AssertProgram(new TestSuite { Sources = { @"S^!_`\W+" }, TestCases = { { "Hello, World!", "World\nHello" } } });
            AssertProgram(new TestSuite { Sources = { @"S^w`(?<=\d).+(?=\d)" }, TestCases = { { "ab1cd2ef3gh4ij", "4ij\n\n\n\n\n\nab1" } } });
            AssertProgram(new TestSuite { Sources = { @"S^rw`(?<=\d).+(?=\d)" }, TestCases = { { "ab1cd2ef3gh4ij", "4ij\n\n\n\n\n\nab1" } } });
        }

        [TestMethod]
        public void TestOutputLimit()
        {
            AssertProgram(new TestSuite { Sources = { @"S!_, 1,-2`(\W)+" }, TestCases = { { "Hello, World!", " \nWorld" } } });
            AssertProgram(new TestSuite { Sources = { @"S!_^, 2,`(\W)+" }, TestCases = { { "Hello, World!", "!\nWorld" } } });
        }

        [TestMethod]
        public void TestListFormatting()
        {
            AssertProgram(new TestSuite { Sources = { @"S['[|"", ""]']!_`" }, TestCases = { { "abcd", "[a, b, c, d]" } } });
        }

        [TestMethod]
        public void TestRandom()
        {
            AssertRandomProgram(new RandomTestSuite
            {
                Sources = { @"S&`(\w+)" },
                TestCases = { { "abc,def;ghi", new List<string>
                {
                    "",
                    "abc",
                    ",",
                    "def",
                    ";",
                    "ghi",
                }
                } }
            });
        }
    }
}
