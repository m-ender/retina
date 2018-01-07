using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace RetinaTest
{
    [TestClass]
    public class GeneralOptionsTest : RetinaTestBase
    {
        [TestMethod]
        public void TestAnchoring()
        {
            AssertProgram(new TestSuite
            {
                Sources = { @"La`\d+" },
                TestCases = {
                    { "123", "123" },
                    { "123\n456", "" },
                    { "a123b", "" },
                }
            });

            AssertProgram(new TestSuite
            {
                Sources = { @"Lma`\d+" },
                TestCases = {
                    { "123", "123" },
                    { "123\n456", "" },
                    { "a123b", "" },
                }
            });


            AssertProgram(new TestSuite
            {
                Sources = { @"Ll`\d+" },
                TestCases = {
                    { "123", "123" },
                    { "123\na789b\n456", "123\n456" },
                    { "a123b", "" },
                }
            });

            AssertProgram(new TestSuite
            {
                Sources = { @"Lml`\d+" },
                TestCases = {
                    { "123", "123" },
                    { "123\na789b\n456", "123\n456" },
                    { "a123b", "" },
                }
            });
        }

        [TestMethod]
        public void TestUniqueMatches()
        {
            AssertProgram(new TestSuite { Sources = { @"Lq`." }, TestCases = { { "abacbcedef", "a\nb\nc\ne\nd\nf" } } });
            AssertProgram(new TestSuite { Sources = { @"Lp`." }, TestCases = { { "abacbcedef", "a\nb\nc\nd\ne\nf" } } });
            AssertProgram(new TestSuite { Sources = { @"Lqr`." }, TestCases = { { "abacbcedef", "a\nb\nc\nd\ne\nf" } } });
            AssertProgram(new TestSuite { Sources = { @"Lpr`." }, TestCases = { { "abacbcedef", "a\nb\nc\ne\nd\nf" } } });
        }

        [TestMethod]
        public void TestOverlappingMatches()
        {
            AssertProgram(new TestSuite { Sources = { @"Lv`.+" }, TestCases = { { "abcd", "abcd\nbcd\ncd\nd" } } });
            AssertProgram(new TestSuite { Sources = { @"Lrv`.+" }, TestCases = { { "abcd", "a\nab\nabc\nabcd" } } });
            AssertProgram(new TestSuite { Sources = { @"Lw`.+" }, TestCases = { { "abcd", "a\nab\nabc\nabcd\nb\nbc\nbcd\nc\ncd\nd" } } });
            AssertProgram(new TestSuite { Sources = { @"Lrw`.+" }, TestCases = { { "abcd", "a\nab\nb\nabc\nbc\nc\nabcd\nbcd\ncd\nd" } } });
            AssertProgram(new TestSuite { Sources = { @"Lw`(?<=\d).+(?=\d)" }, TestCases = { { "ab1cd2ef3gh4ij", "cd\ncd2ef\ncd2ef3gh\nef\nef3gh\ngh" } } });
            AssertProgram(new TestSuite { Sources = { @"Lrw`(?<=\d).+(?=\d)" }, TestCases = { { "ab1cd2ef3gh4ij", "cd\ncd2ef\nef\ncd2ef3gh\nef3gh\ngh" } } });
        }

        [TestMethod]
        public void TestInputAsRegex()
        {
            AssertProgram(new TestSuite
            {
                Sources = { @"L:`ab(12\pq" },
                TestCases = {
                    { @"\w+", "ab\n12\npq" },
                    { @"\W", "(\n\\" },
                    { @"\d+", "12" },
                }
            });
        }

        [TestMethod]
        public void TestMultiplePatternsCyclic()
        {
            AssertProgram(new TestSuite {
                Sources = { @"L#3`[a-z]+", @"\d+", @"\W+" },
                TestCases = { { "ab12()cd34[]ef56{}gh", "ab\n12\n()\ncd\n34\n[]\nef\n56\n{}\ngh" } }
            });

            AssertProgram(new TestSuite
            {
                Sources = { @"L#3`[a-z]+", @"\W+", @"\d+" },
                TestCases = { { "ab12()cd34[]ef56{}gh", "ab\n()\n34\nef\n{}" } }
            });

            AssertProgram(new TestSuite
            {
                Sources = { @"Lr#3`[a-z]+", @"\d+", @"\W+" },
                TestCases = { { "ab12()cd34[]ef56{}gh", "12\ncd\n[]\n56\ngh" } }
            });

            AssertProgram(new TestSuite
            {
                Sources = { @"Lr#3`[a-z]+", @"\W+", @"\d+" },
                TestCases = { { "ab12()cd34[]ef56{}gh", "ab\n12\n()\ncd\n34\n[]\nef\n56\n{}\ngh" } }
            });

            AssertProgram(new TestSuite
            {
                Sources = { @"Lv#2`.", @".." },
                TestCases = { { "abcd", "a\nbc\nc" } }
            });

            AssertProgram(new TestSuite
            {
                Sources = { @"Lv#2`..", @"." },
                TestCases = { { "abcd", "ab\nb\ncd\nd" } }
            });

            AssertProgram(new TestSuite
            {
                Sources = { @"Lw#2`.", @".." },
                TestCases = { { "abcd", "a\nab\nb\nbc\nc\ncd\nd" } }
            });

            AssertProgram(new TestSuite
            {
                Sources = { @"Lw#2`..", @"." },
                TestCases = { { "abcd", "ab\nb\nbc\nc\ncd\nd" } }
            });
        }

        [TestMethod]
        public void TestMultiplePatternsGreedy()
        {
            AssertProgram(new TestSuite
            {
                Sources = { @"L#-3`[a-z]+", @"\d+", @"\W+" },
                TestCases = { { "ab12()cd34[]ef56{}gh", "ab\n12\n()\ncd\n34\n[]\nef\n56\n{}\ngh" } }
            });

            AssertProgram(new TestSuite
            {
                Sources = { @"L#-3`\d+", @"[a-z]+", @"\W+" },
                TestCases = { { "ab12()cd34[]ef56{}gh", "ab\n12\n()\ncd\n34\n[]\nef\n56\n{}\ngh" } }
            });

            AssertProgram(new TestSuite
            {
                Sources = { @"Lr#-3`[a-z]+", @"\d+", @"\W+" },
                TestCases = { { "ab12()cd34[]ef56{}gh", "ab\n12\n()\ncd\n34\n[]\nef\n56\n{}\ngh" } }
            });

            AssertProgram(new TestSuite
            {
                Sources = { @"Lr#-3`\d+", @"[a-z]+", @"\W+" },
                TestCases = { { "ab12()cd34[]ef56{}gh", "ab\n12\n()\ncd\n34\n[]\nef\n56\n{}\ngh" } }
            });

            AssertProgram(new TestSuite
            {
                Sources = { @"Lv#-2`.", @".." },
                TestCases = { { "abcd", "a\nb\nc\nd" } }
            });

            AssertProgram(new TestSuite
            {
                Sources = { @"Lv#-2`..", @"." },
                TestCases = { { "abcd", "ab\nbc\ncd\nd" } }
            });

            AssertProgram(new TestSuite
            {
                Sources = { @"Lw#-2`.", @".." },
                TestCases = { { "abcd", "a\nab\nb\nbc\nc\ncd\nd" } }
            });

            AssertProgram(new TestSuite
            {
                Sources = { @"Lw#-2`..", @"." },
                TestCases = { { "abcd", "a\nab\nb\nbc\nc\ncd\nd" } }
            });
        }

        [TestMethod]
        public void TestMultiplePatternsModifiers()
        {
            AssertProgram(new TestSuite {
                Sources = { @"L#2i`[a-m]+", @"[n-z]+" },
                TestCases = { { "HelloWorld", "Hell\noWor\nld" } }
            });
        }

        [TestMethod]
        public void TestMultiplePatternsReplacement()
        {
            AssertProgram(new TestSuite
            {
                Sources = { @"L#2$`[a-z]+", "$&$&", @"\d+", "$*" },
                TestCases = { { "ab12cd21ef", "abab\n____________\ncdcd\n_____________________\nefef" } }
            });
        }

        [TestMethod]
        public void TestInvertMatches()
        {
            AssertProgram(new TestSuite
            {
                Sources = { @"L=`[a-z]+" },
                TestCases = { { "ab12cd34ef", "\n12\n34\n" } }
            });

            AssertProgram(new TestSuite
            {
                Sources = { @"L=$`[a-z]+", "$.`,$.&" },
                TestCases = { { "ab12cd34ef", "0,0\n2,2\n6,2\n10,0" } }
            });

            AssertProgram(new TestSuite
            {
                Sources = { @"L#3=`[a-z]+", @"\W+", @"\d+" },
                TestCases = { { "ab12()cd34[]ef56{}gh", "\n12\ncd\n[]\n56\ngh" } }
            });

            AssertProgram(new TestSuite
            {
                Sources = { @"L#3=$`[a-z]+", @"\W+", @"\d+", "$.`,$.&" },
                TestCases = { { "ab12()cd34[]ef56{}gh", "0,0\n2,2\n6,2\n10,2\n14,2\n18,2" } }
            });
        }

        [TestMethod]
        public void TestListFormatting()
        {
            AssertProgram(new TestSuite { Sources = { @"L[{`." }, TestCases = { { "abcd", "{a\nb\nc\nd" } } });
            AssertProgram(new TestSuite { Sources = { @"L]}`." }, TestCases = { { "abcd", "a\nb\nc\nd}" } } });
            AssertProgram(new TestSuite { Sources = { @"L|``." }, TestCases = { { "abcd", "a`b`c`d" } } });

            AssertProgram(new TestSuite { Sources = { @"L[[|,]]`." }, TestCases = { { "abcd", "[a,b,c,d]" } } });

            AssertProgram(new TestSuite { Sources = { @"L['{`." }, TestCases = { { "abcd", "{a\nb\nc\nd" } } });
            AssertProgram(new TestSuite { Sources = { @"L]'}`." }, TestCases = { { "abcd", "a\nb\nc\nd}" } } });
            AssertProgram(new TestSuite { Sources = { @"L|'``." }, TestCases = { { "abcd", "a`b`c`d" } } });

            AssertProgram(new TestSuite { Sources = { @"L['[|',]']`." }, TestCases = { { "abcd", "[a,b,c,d]" } } });

            AssertProgram(new TestSuite { Sources = { @"L[""{""""{""`." }, TestCases = { { "abcd", "{\"{a\nb\nc\nd" } } });
            AssertProgram(new TestSuite { Sources = { @"L]""}""""}""`." }, TestCases = { { "abcd", "a\nb\nc\nd}\"}" } } });
            AssertProgram(new TestSuite { Sources = { @"L|""`""""`""`." }, TestCases = { { "abcd", "a`\"`b`\"`c`\"`d" } } });

            AssertProgram(new TestSuite { Sources = { @"L['[|"", ""]']`." }, TestCases = { { "abcd", "[a, b, c, d]" } } });
        }

        [TestMethod]
        public void TestRandomMatch()
        {
            AssertRandomProgram(new RandomTestSuite
            {
                Sources = { @"L@`.+" },
                TestCases = { { "abc\ndef\nghi\njkl\nmno", new List<string>
                {
                    "abc",
                    "def",
                    "ghi",
                    "jkl",
                    "mno",
                } } }
            });

            AssertRandomProgram(new RandomTestSuite
            {
                Sources = { @"L~1,2@`.+" },
                TestCases = { { "abc\ndef\nghi\njkl\nmno", new List<string>
                {
                    "abc",
                    "jkl",
                    "mno",
                } } }
            });
            
            AssertRandomProgram(new RandomTestSuite
            {
                Sources = { @"L@`\d+" },
                TestCases = { { "abc\ndef\nghi\njkl\nmno", new List<string> { "" } } }
            });
        }
    }
}