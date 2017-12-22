using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
            AssertProgram(new TestSuite { Sources = { @"S!_`(\W)+" }, TestCases = { { "Hello, World!", "Hello\n \nWorld\n!" } } });
            AssertProgram(new TestSuite { Sources = { @"S!_`(.o)(.)" }, TestCases = { { "Hello, World!", "Hel\nlo\n,\n \nWo\nr\nld!" } } });
            AssertProgram(new TestSuite { Sources = { @"S!_`(\b|l)" }, TestCases = { { "Hello, World!", "\nHe\nl\nl\no\n\n, \n\nWor\nl\nd\n\n!" } } });
        }
    }
}
