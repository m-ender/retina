using System;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Retina;
using Retina.Stages;
using System.Collections.Generic;

namespace RetinaTest
{
    // TODO: Should we be invoking the Replacer class directly instead of going through vanilla Replace stages?
    [TestClass]
    public class HistoryTest : RetinaTestBase
    {
        [TestMethod]
        public void TestBasicHistory()
        {
            AssertProgram(new TestSuite
            {
                Sources =
                {
                    @".+",
                    @"$&$&",
                    @"{*`.+",
                    @"$&$&",
                    @"\`^",
                    @"$.-0$.-1$.+2$.+3$.+4",
                    @")`^.{0,7}",
                    @"",
                    @"^$",
                    @"$+0,$+1,$+2,$+3,$+4,$+5,$+6,$+7",
                },
                TestCases = { { "abc", "12612abcabc\n848114cabc\n636103abc\n42492bc\n00070\nabc,abcabc,,00070,,,," } }
            });

            AssertProgram(new TestSuite
            {
                Sources =
                {
                    @"%`.+",
                    @"$.&",
                    @"s`.+",
                    @"$+,$+0,$+1,$+2,$+3,$-,$-0,$-1,$-2,$-3",
                },
                TestCases = { { "a\nbc\ndef", "a\nbc\ndef,a\nbc\ndef,3,1\n2\n3,,1\n2\n3,1\n2\n3,3,2,1" } }
            });


            AssertProgram(new TestSuite
            {
                Sources =
                {
                    @"_`.+",
                    @"$.&",
                    @"s`.+",
                    @"$+0,$+1,$+2,$+3,$-0,$-1,$-2,$-3",
                },
                TestCases = { { "a\nbc\ndef", "a\nbc\ndef,3,1\n2\n3,,1\n2\n3,3,2,1" } }
            });
        }

        [TestMethod]
        public void TestActivatedLog()
        {
            // Ensure result log gets activated by dynamic elements:
            AssertProgram(new TestSuite { Sources = { "^", @"${1*-}" }, TestCases = { { "abc", "abcabc" } } });

            // Ensure result log gets activated by substitutions given as string options:
            AssertProgram(new TestSuite { Sources = { "\"<$.->\"+`^", @"1" }, TestCases = { { "abc", "111abc" } } });
        }

        [TestMethod]
        public void TestLogLimit()
        {
            // Test result log limit
            AssertProgram(new TestSuite { Sources = { "!#`.+", @"$-,$+" }, TestCases = { { "abc", ",abc" } } });
            AssertProgram(new TestSuite {
                Sources = {
                    "!#3G`",
                    "G`",
                    "G`",
                    "G`",
                    ".+",
                    "$-0,$-1,$-2,$-3,$-4"
                },
                TestCases = { { "abc", "abc,abc,abc,," } }
            });
        }

        [TestMethod]
        public void TestRegisterToggle()
        {
            AssertProgram(new TestSuite
            {
                Sources = {
                    "$",
                    "1",
                    "!,`$",
                    "2",
                    "$",
                    "3",
                    ".+",
                    "$-0,$-1,$-2;$+0,$+1,$+2"
                },
                TestCases = { { "abc", "abc123,abc1,abc;abc,abc1,abc123" } }
            });

            // Output stages never register, even with a toggle
            AssertProgram(new TestSuite
            {
                Sources = {
                    @"!,\`$",
                    "1",
                    ".+",
                    "$-0,$-1,$-2;$+0,$+1,$+2"
                },
                TestCases = { { "abc", "abc1\nabc1,abc,;abc,abc1," } }
            });

            // Dry run stages without a string or regex option never register
            AssertProgram(new TestSuite
            {
                Sources = {
                    "!,*`$",
                    "1",
                    ".+",
                    "$-0,$-1,$-2;$+0,$+1,$+2"
                },
                TestCases = { { "abc", "abc1,abc,;abc,abc1," } }
            });

            // Dry run stages with a string or regex option can be toggled off.
            AssertProgram(new TestSuite
            {
                Sources = {
                    "'0*`$",
                    "1",
                    ".+",
                    "$-0,$-1,$-2;$+0,$+1,$+2"
                },
                TestCases = { { "abc", "abc,abc1,abc;abc,abc1,abc" } }
            });
            AssertProgram(new TestSuite
            {
                Sources = {
                    "!,'0*`$",
                    "1",
                    ".+",
                    "$-0,$-1,$-2;$+0,$+1,$+2"
                },
                TestCases = { { "abc", "abc1,abc,;abc,abc1," } }
            });
        }

        [TestMethod]
        public void TestRegisterByDefaultToggle()
        {
            AssertProgram(new TestSuite
            {
                Sources = {
                    "K`1",
                    "!.K`2",
                    "K`3",
                    "!.K`4",
                    "!.&K`5",
                    "!.&K`6",
                    "&!.K`7",
                    "&!.K`8",
                    "K`9",
                    ".+",
                    "$-0$-1$-2$-3$-4$-5$-6$-7,$-8;$+0$+1$+2$+3$+4$+5$+6$+7,$+8"
                },
                TestCases = { { "abc", "9886641abc,;abc1466889," } }
            });

            AssertProgram(new TestSuite
            {
                Sources = {
                    "!,K`1",
                    "!.!,K`2",
                    "!,K`3",
                    "!,!.K`4",
                    "!.!,&!,K`5",
                    "!.!,&!,K`6",
                    "!,&!,!.K`7",
                    "!,&!,!.K`8",
                    "!,K`9",
                    ".+",
                    "$-0$-1$-2$-3$-4$-5$-6,$-7;$+0$+1$+2$+3$+4$+5$+6,$+7"
                },
                TestCases = { { "abc", "775532abc,;abc235577," } }
            });

            // Default toggle doesn't affect output stages and unconfigured dry run stages
            AssertProgram(new TestSuite
            {
                Sources = {
                    @"!.*\K`1",
                    ".+",
                    "$-0,$-1;$+0,$+1"
                },
                TestCases = { { "abc", "1\nabc,;abc," } }
            });
        }
    }
}
