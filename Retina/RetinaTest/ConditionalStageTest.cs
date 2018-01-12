using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace RetinaTest
{
    [TestClass]
    public class ConditionalStageTest : RetinaTestBase
    {
        [TestMethod]
        public void TestIfMatch()
        {
            AssertProgram(new TestSuite
            {
                Sources =
                {
                    @"/\d/?`.+",
                    @"$&$&",
                },
                TestCases = {
                    { "123", "123123" },
                    { "abc", "abc" },
                }
            });
        }

        [TestMethod]
        public void TestIfNotMatch()
        {
            AssertProgram(new TestSuite
            {
                Sources =
                {
                    @"/\d/^?`.+",
                    @"$&$&",
                },
                TestCases = {
                    { "123", "123" },
                    { "abc", "abcabc" },
                }
            });
        }

        [TestMethod]
        public void TestRandom()
        {
            AssertRandomProgram(new RandomTestSuite
            {
                Sources =
                {
                    @"&?`.+",
                    @"$&$&",
                },
                TestCases = {
                    { "123", new string[] { "123", "123123" } }
                }
            });
        }
    }
}
