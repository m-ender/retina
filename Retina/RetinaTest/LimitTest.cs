using System;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Retina;
using Retina.Stages;
using System.Collections.Generic;

namespace RetinaTest
{
    [TestClass]
    public class LimitTest : RetinaTestBase
    {
        [TestMethod]
        public void TestSpecificPositiveLimit()
        {
            AssertProgram(new TestSuite { Sources = { @"L0`." }, TestCases = { { "0123456789", "0" } } });
            AssertProgram(new TestSuite { Sources = { @"L1`." }, TestCases = { { "0123456789", "1" } } });
            AssertProgram(new TestSuite { Sources = { @"L3`." }, TestCases = { { "0123456789", "3" } } });
            AssertProgram(new TestSuite { Sources = { @"L9`." }, TestCases = { { "0123456789", "9" } } });
            AssertProgram(new TestSuite { Sources = { @"L10`." }, TestCases = { { "0123456789", "" } } });
        }

        [TestMethod]
        public void TestSpecificNegativeLimit()
        {
            AssertProgram(new TestSuite { Sources = { @"L-1`." }, TestCases = { { "0123456789", "9" } } });
            AssertProgram(new TestSuite { Sources = { @"L-2`." }, TestCases = { { "0123456789", "8" } } });
            AssertProgram(new TestSuite { Sources = { @"L-4`." }, TestCases = { { "0123456789", "6" } } });
            AssertProgram(new TestSuite { Sources = { @"L-10`." }, TestCases = { { "0123456789", "0" } } });
            AssertProgram(new TestSuite { Sources = { @"L-11`." }, TestCases = { { "0123456789", "" } } });
        }

        [TestMethod]
        public void TestSimpleRange()
        {
            // +,+
            AssertProgram(new TestSuite { Sources = { @"L3,5`." }, TestCases = { { "0123456789", "3\n4\n5" } } });
            AssertProgram(new TestSuite { Sources = { @"L7,7`." }, TestCases = { { "0123456789", "7" } } });
            AssertProgram(new TestSuite { Sources = { @"L6,5`." }, TestCases = { { "0123456789", "" } } });

            // +,-
            AssertProgram(new TestSuite { Sources = { @"L4,-4`." }, TestCases = { { "0123456789", "4\n5\n6" } } });
            AssertProgram(new TestSuite { Sources = { @"L5,-5`." }, TestCases = { { "0123456789", "5" } } });
            AssertProgram(new TestSuite { Sources = { @"L6,-5`." }, TestCases = { { "0123456789", "" } } });

            // -,+
            AssertProgram(new TestSuite { Sources = { @"L-6,6`." }, TestCases = { { "0123456789", "4\n5\n6" } } });
            AssertProgram(new TestSuite { Sources = { @"L-5,5`." }, TestCases = { { "0123456789", "5" } } });
            AssertProgram(new TestSuite { Sources = { @"L-4,5`." }, TestCases = { { "0123456789", "" } } });
            
            // -,-
            AssertProgram(new TestSuite { Sources = { @"L-5,-3`." }, TestCases = { { "0123456789", "5\n6\n7" } } });
            AssertProgram(new TestSuite { Sources = { @"L-7,-7`." }, TestCases = { { "0123456789", "3" } } });
            AssertProgram(new TestSuite { Sources = { @"L-5,-6`." }, TestCases = { { "0123456789", "" } } });

            // Going out of bounds.
            AssertProgram(new TestSuite { Sources = { @"L7,12`." }, TestCases = { { "0123456789", "7\n8\n9" } } });
            AssertProgram(new TestSuite { Sources = { @"L10,12`." }, TestCases = { { "0123456789", "" } } });
            AssertProgram(new TestSuite { Sources = { @"L-12,2`." }, TestCases = { { "0123456789", "0\n1\n2" } } });
            AssertProgram(new TestSuite { Sources = { @"L-12,-11`." }, TestCases = { { "0123456789", "" } } });
            AssertProgram(new TestSuite { Sources = { @"L-12,12`." }, TestCases = { { "0123456789", "0\n1\n2\n3\n4\n5\n6\n7\n8\n9" } } });

            // Implicit range ends.
            AssertProgram(new TestSuite { Sources = { @"L7,`." }, TestCases = { { "0123456789", "7\n8\n9" } } });
            AssertProgram(new TestSuite { Sources = { @"L,2`." }, TestCases = { { "0123456789", "0\n1\n2" } } });
            AssertProgram(new TestSuite { Sources = { @"L,`." }, TestCases = { { "0123456789", "0\n1\n2\n3\n4\n5\n6\n7\n8\n9" } } });
        }

        [TestMethod]
        public void TestRangeWithStep()
        {
            // +,,+
            AssertProgram(new TestSuite { Sources = { @"L1,3,8`." }, TestCases = { { "0123456789", "1\n4\n7" } } });
            AssertProgram(new TestSuite { Sources = { @"L1,-3,8`." }, TestCases = { { "0123456789", "2\n5\n8" } } });
            AssertProgram(new TestSuite { Sources = { @"L1,3,11`." }, TestCases = { { "0123456789", "1\n4\n7" } } });
            AssertProgram(new TestSuite { Sources = { @"L1,-3,11`." }, TestCases = { { "0123456789", "2\n5\n8" } } });
            
            // +,,-
            AssertProgram(new TestSuite { Sources = { @"L1,3,-2`." }, TestCases = { { "0123456789", "1\n4\n7" } } });
            AssertProgram(new TestSuite { Sources = { @"L1,-3,-2`." }, TestCases = { { "0123456789", "2\n5\n8" } } });

            // -,,+
            AssertProgram(new TestSuite { Sources = { @"L-9,3,8`." }, TestCases = { { "0123456789", "1\n4\n7" } } });
            AssertProgram(new TestSuite { Sources = { @"L-9,-3,8`." }, TestCases = { { "0123456789", "2\n5\n8" } } });
            AssertProgram(new TestSuite { Sources = { @"L-12,3,8`." }, TestCases = { { "0123456789", "1\n4\n7" } } });
            AssertProgram(new TestSuite { Sources = { @"L-12,-3,8`." }, TestCases = { { "0123456789", "2\n5\n8" } } });
            AssertProgram(new TestSuite { Sources = { @"L-12,3,11`." }, TestCases = { { "0123456789", "1\n4\n7" } } });
            AssertProgram(new TestSuite { Sources = { @"L-12,-3,11`." }, TestCases = { { "0123456789", "2\n5\n8" } } });

            // -,,-
            AssertProgram(new TestSuite { Sources = { @"L-9,3,-2`." }, TestCases = { { "0123456789", "1\n4\n7" } } });
            AssertProgram(new TestSuite { Sources = { @"L-9,-3,-2`." }, TestCases = { { "0123456789", "2\n5\n8" } } });
            AssertProgram(new TestSuite { Sources = { @"L-12,3,-2`." }, TestCases = { { "0123456789", "1\n4\n7" } } });
            AssertProgram(new TestSuite { Sources = { @"L-12,-3,-2`." }, TestCases = { { "0123456789", "2\n5\n8" } } });

            // Implicit step size (= 0)
            AssertProgram(new TestSuite { Sources = { @"L1,,8`." }, TestCases = { { "0123456789", "1\n8" } } });
            AssertProgram(new TestSuite { Sources = { @"L1,,-2`." }, TestCases = { { "0123456789", "1\n8" } } });
            AssertProgram(new TestSuite { Sources = { @"L-9,,8`." }, TestCases = { { "0123456789", "1\n8" } } });
            AssertProgram(new TestSuite { Sources = { @"L-9,,-2`." }, TestCases = { { "0123456789", "1\n8" } } });

            AssertProgram(new TestSuite { Sources = { @"L4,,4`." }, TestCases = { { "0123456789", "4" } } });
            AssertProgram(new TestSuite { Sources = { @"L4,,-6`." }, TestCases = { { "0123456789", "4" } } });
            AssertProgram(new TestSuite { Sources = { @"L-6,,4`." }, TestCases = { { "0123456789", "4" } } });
            AssertProgram(new TestSuite { Sources = { @"L-6,,-6`." }, TestCases = { { "0123456789", "4" } } });

            AssertProgram(new TestSuite { Sources = { @"L4,,12`." }, TestCases = { { "0123456789", "4" } } });
            AssertProgram(new TestSuite { Sources = { @"L-12,,4`." }, TestCases = { { "0123456789", "4" } } });

            AssertProgram(new TestSuite { Sources = { @"L,,`." }, TestCases = { { "0123456789", "0\n9" } } });
        }

        [TestMethod]
        public void TestNegatedLimit()
        {
            AssertProgram(new TestSuite { Sources = { @"L~3`." }, TestCases = { { "0123456789", "0\n1\n2\n4\n5\n6\n7\n8\n9" } } });
            AssertProgram(new TestSuite { Sources = { @"L~10`." }, TestCases = { { "0123456789", "0\n1\n2\n3\n4\n5\n6\n7\n8\n9" } } });
            AssertProgram(new TestSuite { Sources = { @"L~-4`." }, TestCases = { { "0123456789", "0\n1\n2\n3\n4\n5\n7\n8\n9" } } });

            AssertProgram(new TestSuite { Sources = { @"L~2,7`." }, TestCases = { { "0123456789", "0\n1\n8\n9" } } });
            AssertProgram(new TestSuite { Sources = { @"L~2,-3`." }, TestCases = { { "0123456789", "0\n1\n8\n9" } } });
            AssertProgram(new TestSuite { Sources = { @"L~-8,7`." }, TestCases = { { "0123456789", "0\n1\n8\n9" } } });
            AssertProgram(new TestSuite { Sources = { @"L~-8,-3`." }, TestCases = { { "0123456789", "0\n1\n8\n9" } } });

            AssertProgram(new TestSuite { Sources = { @"L~1,3,8`." }, TestCases = { { "0123456789", "0\n2\n3\n5\n6\n8\n9" } } });
            AssertProgram(new TestSuite { Sources = { @"L~1,-3,8`." }, TestCases = { { "0123456789", "0\n1\n3\n4\n6\n7\n9" } } });
            AssertProgram(new TestSuite { Sources = { @"L~1,,8`." }, TestCases = { { "0123456789", "0\n2\n3\n4\n5\n6\n7\n9" } } });
            AssertProgram(new TestSuite { Sources = { @"L~,,`." }, TestCases = { { "0123456789", "1\n2\n3\n4\n5\n6\n7\n8" } } });
        }

    }
}
