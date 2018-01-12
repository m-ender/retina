using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace RetinaTest
{
    [TestClass]
    public class LoopStageTest : RetinaTestBase
    {
        [TestMethod]
        public void TestBasicLoop()
        {
            AssertProgram(new TestSuite { Sources = { @"+`." }, TestCases = { { "abcdefghijklmnopqrstuvwxyz", "1" } } });
            AssertProgram(new TestSuite { Sources = { @"+`(.)\1", @"$1" }, TestCases = { { "aaaaaaabbbbbbcccccccddddeeeee", "abcde" } } });
        }

        [TestMethod]
        public void TestGroupLoop()
        {
            AssertProgram(new TestSuite {
                Sources = {
                    @"{`ab",
                    @"c",
                    @")`ac",
                    @"b"
                },
                TestCases = { { "aaaaaaaab", "b" } }
            });

            AssertProgram(new TestSuite
            {
                Sources = {
                    @"(`ab",
                    @"c",
                    @"}`ac",
                    @"b"
                },
                TestCases = { { "aaaaaaaab", "b" } }
            });
        }

        [TestMethod]
        public void TestRandom()
        {
            AssertRandomProgram(new RandomTestSuite
            {
                Sources = {
                    @"&+`^",
                    @"1"
                },
                TestCases = { { "", new List<Tuple<string, double>>()
                {
                    new Tuple<string, double>("", 1/2.0),
                    new Tuple<string, double>("1", 1/4.0),
                    new Tuple<string, double>("11", 1/8.0),
                    new Tuple<string, double>("111", 1/16.0),
                    new Tuple<string, double>("1111", 1/32.0),
                    new Tuple<string, double>("11111", 1/64.0),
                }
                } }
            });


            AssertRandomProgram(new RandomTestSuite
            {
                Sources = {
                    @"&+*>`^",
                    @"1"
                },
                TestCases = { { "", new List<Tuple<string, double>>()
                {
                    new Tuple<string, double>("", 1/2.0),
                    new Tuple<string, double>("1", 1/4.0),
                    new Tuple<string, double>("11", 1/8.0),
                    new Tuple<string, double>("111", 1/16.0),
                    new Tuple<string, double>("1111", 1/32.0),
                    new Tuple<string, double>("11111", 1/64.0),
                }
                } }
            });

            AssertRandomProgram(new RandomTestSuite
            {
                Sources = {
                    @"&+`^1",
                    @""
                },
                TestCases = { { "11", new List<Tuple<string, double>>()
                {
                    new Tuple<string, double>("11", 1/2.0),
                    new Tuple<string, double>("1", 1/4.0),
                    new Tuple<string, double>("", 1/4.0),
                }
                } }
            });
        }
    }
}
