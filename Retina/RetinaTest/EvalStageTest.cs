using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace RetinaTest
{
    [TestClass]
    public class EvalStageTest : RetinaTestBase
    {
        [TestMethod]
        public void TestEval()
        {
            AssertProgram(new TestSuite
            {
                Sources =
                {
                    @"^",
                    @"abc",
                    @"""$-0""~`.+",
                    @".+$n$$&$$&$n{$*`.+$n$$&$$&$n\`^$n$$.-0$$.-1$$.+2$$.+3$$.+4$n)`^.{0,7}$n$n^$$$n$$+0,$$+1,$$+2,$$+3,$$+4,$$+5,$$+6,$$+7",
                    @"*\L`^..",
                },
                TestCases = { { "", "12\n12612abcabc\n848114cabc\n636103abc\n42492bc\n00070\nabc,abcabc,,00070,,,," } }
            });
        }
    }
}
