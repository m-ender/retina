using System;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Retina;
using Retina.Stages;

namespace RetinaTest
{
    [TestClass]
    public class TransliterateStageTest : RetinaTestBase
    {
        [TestMethod]
        public void TestRegex()
        {
            AssertProgram(new TestSuite { Sources = { @"T`a`b`.\b|\b." }, TestCases = { { "abc aba bab", "bbc bbb bab" } } });
            AssertProgram(new TestSuite { Sources = { @"T`ab`ba`a\b|\ba" }, TestCases = { { "abc aba bab", "bbc bbb bab" } } });
            AssertProgram(new TestSuite { Sources = { @"T`ab`ba`.\b|\b." }, TestCases = { { "abc aba bab", "bbc bbb aaa" } } });
        }

        [TestMethod]
        public void TestShorterTarget()
        {
            AssertTransliteration(@"T`0123456`abc", @" !""#$%&'()*+,-./abccccc789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\]^_`abcdefghijklmnopqrstuvwxyz{|}~");
        }

        [TestMethod]
        public void TestMissingTarget()
        {
            AssertTransliteration(@"T`0123456", @" !""#$%&'()*+,-./789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\]^_`abcdefghijklmnopqrstuvwxyz{|}~");
            AssertTransliteration(@"T`0123456`", @" !""#$%&'()*+,-./789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\]^_`abcdefghijklmnopqrstuvwxyz{|}~");
            AssertTransliteration(@"T`0123456``.+", @" !""#$%&'()*+,-./789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\]^_`abcdefghijklmnopqrstuvwxyz{|}~");
        }

        [TestMethod]
        public void TestEscapeSequences()
        {
            AssertProgram(new TestSuite { Sources = { @"T`\a\b\f\n`\r\t\v\" }, TestCases = { { "\a\b\f\n\r\t\v\\", "\r\t\v\\\r\t\v\\" } } });
            AssertProgram(new TestSuite { Sources = { @"T`\A\``\`B" }, TestCases = { { "AB`BA", "`BBB`" } } });
            AssertProgram(new TestSuite { Sources = { @"T`a\-z`012" }, TestCases = { { "abc-xyz", "0bc1xy2" } } });
        }

        [TestMethod]
        public void TestRanges()
        {
            AssertTransliteration(@"T`a-z`A-Z", @" !""#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\]^_`ABCDEFGHIJKLMNOPQRSTUVWXYZ{|}~");
            AssertTransliteration(@"T`a-z`z-a", @" !""#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\]^_`zyxwvutsrqponmlkjihgfedcba{|}~");
            AssertTransliteration(@"T`z-a`a-z", @" !""#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\]^_`zyxwvutsrqponmlkjihgfedcba{|}~");
            AssertTransliteration(@"T`--0`123", @" !""#$%&'()*+,1233123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\]^_`abcdefghijklmnopqrstuvwxyz{|}~");
            AssertTransliteration(@"T`0--`321", @" !""#$%&'()*+,1123123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\]^_`abcdefghijklmnopqrstuvwxyz{|}~");
            AssertProgram(new TestSuite { Sources = { @"T`\n- `a-z" }, TestCases = { { "\n ", "aw" } } });
            AssertProgram(new TestSuite { Sources = { @"T` -\n`z-a" }, TestCases = { { "\n ", "dz" } } });

            // Hyphens at the start or end are literals:
            AssertTransliteration(@"T`-xy`123", @" !""#$%&'()*+,1./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\]^_`abcdefghijklmnopqrstuvw23z{|}~");
            AssertTransliteration(@"T`bc-`123", @" !""#$%&'()*+,3./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\]^_`a12defghijklmnopqrstuvwxyz{|}~");
        }

        [TestMethod]
        public void TestClasses()
        {
            AssertTransliteration(@"T`d`a-j", @" !""#$%&'()*+,-./abcdefghij:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\]^_`abcdefghijklmnopqrstuvwxyz{|}~");
            AssertTransliteration(@"T`w`dA-Za-z\_", @" !""#$%&'()*+,-./123456789A:;<=>?@BCDEFGHIJKLMNOPQRSTUVWXYZa[\]^0`bcdefghijklmnopqrstuvwxyz_{|}~");
            AssertTransliteration(@"T`H`a-p", @" !""#$%&'()*+,-./abcdefghij:;<=>?@klmnopGHIJKLMNOPQRSTUVWXYZ[\]^_`abcdefghijklmnopqrstuvwxyz{|}~");
            AssertTransliteration(@"T`h`A-P", @" !""#$%&'()*+,-./ABCDEFGHIJ:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\]^_`KLMNOPghijklmnopqrstuvwxyz{|}~");
            AssertTransliteration(@"T`L`a-z", @" !""#$%&'()*+,-./0123456789:;<=>?@abcdefghijklmnopqrstuvwxyz[\]^_`abcdefghijklmnopqrstuvwxyz{|}~");
            AssertTransliteration(@"T`l`A-Z", @" !""#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\]^_`ABCDEFGHIJKLMNOPQRSTUVWXYZ{|}~");
            AssertTransliteration(@"T`p`!-~ ", @"!""#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\]^_`abcdefghijklmnopqrstuvwxyz{|}~ ");
            AssertTransliteration(@"T`E`a-e", @" !""#$%&'()*+,-./a1b3c5d7e9:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\]^_`abcdefghijklmnopqrstuvwxyz{|}~");
            AssertTransliteration(@"T`O`a-e", @" !""#$%&'()*+,-./0a2b4c6d8e:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\]^_`abcdefghijklmnopqrstuvwxyz{|}~");

            // Classes in ranges are ignored:
            AssertTransliteration(@"T`d-f`123", @" !""#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\]^_`abc123ghijklmnopqrstuvwxyz{|}~");
            AssertTransliteration(@"T`f-d`321", @" !""#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\]^_`abc123ghijklmnopqrstuvwxyz{|}~");
        }

        [TestMethod]
        public void TestReverseRange()
        {
            AssertTransliteration(@"T`Rd`a-j", @" !""#$%&'()*+,-./jihgfedcba:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\]^_`abcdefghijklmnopqrstuvwxyz{|}~");
            AssertTransliteration(@"T`RRd`a-j", @" !""#$%&'()*+,-./abcdefghij:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\]^_`abcdefghijklmnopqrstuvwxyz{|}~");
            AssertTransliteration(@"T`RRRd`a-j", @" !""#$%&'()*+,-./jihgfedcba:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\]^_`abcdefghijklmnopqrstuvwxyz{|}~");
            AssertTransliteration(@"T`Rw`w", @" !""#$%&'()*+,-./yxwvutsrqp:;<=>?@onmlkjihgfedcbaZYXWVUTSRQP[\]^z`ONMLKJIHGFEDCBA9876543210_{|}~");
            AssertTransliteration(@"T`a-z`Ra-z", @" !""#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\]^_`zyxwvutsrqponmlkjihgfedcba{|}~");
            AssertTransliteration(@"T`Ra-z`a-z", @" !""#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\]^_`zyxwvutsrqponmlkjihgfedcba{|}~");
            AssertTransliteration(@"T`z-a`Rz-a", @" !""#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\]^_`zyxwvutsrqponmlkjihgfedcba{|}~");
            AssertTransliteration(@"T`Rz-a`z-a", @" !""#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\]^_`zyxwvutsrqponmlkjihgfedcba{|}~");
            AssertTransliteration(@"T`R\\-Z`\\-Z", @" !""#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXY\[Z]^_`abcdefghijklmnopqrstuvwxyz{|}~");

            // R not followed by a range or class should be literal R, as should R as part of a range.
            AssertTransliteration(@"T`a-zR`a-z", @" !""#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQzSTUVWXYZ[\]^_`abcdefghijklmnopqrstuvwxyz{|}~");
            AssertTransliteration(@"T`Rab-z`Qa-z", @" !""#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQQSTUVWXYZ[\]^_`abcdefghijklmnopqrstuvwxyz{|}~");
            AssertTransliteration(@"T`R-Z`Z-R", @" !""#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQZYXWVUTSR[\]^_`abcdefghijklmnopqrstuvwxyz{|}~");
            AssertTransliteration(@"T`RR-Z`R-Z", @" !""#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQZYXWVUTSR[\]^_`abcdefghijklmnopqrstuvwxyz{|}~");
            AssertTransliteration(@"T`RRR-Z`R-Z", @" !""#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\]^_`abcdefghijklmnopqrstuvwxyz{|}~");

        }

        [TestMethod]
        public void TestReferencingOtherSet()
        {
            AssertTransliteration(@"T`#$%&'()*+,-./`/o", @" !""/#$%&'()*+,-.0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\]^_`abcdefghijklmnopqrstuvwxyz{|}~");
            AssertTransliteration(@"T`/o`#$%&'()*+,-./", @" !""$%&'()*+,-./#0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\]^_`abcdefghijklmnopqrstuvwxyz{|}~");
            AssertTransliteration(@"T`#$%&'()*+,-./`Ro", @" !""/.-,+*)('&%$#0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\]^_`abcdefghijklmnopqrstuvwxyz{|}~");
            AssertTransliteration(@"T`Ro`#$%&'()*+,-./", @" !""/.-,+*)('&%$#0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\]^_`abcdefghijklmnopqrstuvwxyz{|}~");

            // Using "o" twice inserts literal "o"s because inserting the other set twice is pointless
            AssertTransliteration(@"T`#$%&'()*+,-./`/ofoo", @" !""/#$%&'()*+,-.0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\]^_`abcdefghijklmnopqrstuvwxyz{|}~");
            AssertTransliteration(@"T`/ofoo`#$%&'()*+,-./", @" !""$%&'()*+,-./#0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\]^_`abcde/ghijklmn/pqrstuvwxyz{|}~");
            AssertTransliteration(@"T`#$%&'()*+,-./`Rofoo", @" !""/.-,+*)('&%$#0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\]^_`abcdefghijklmnopqrstuvwxyz{|}~");
            AssertTransliteration(@"T`Rofoo`#$%&'()*+,-./", @" !""/.-,+*)('&%$#0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\]^_`abcde/ghijklmn/pqrstuvwxyz{|}~");

            // Using "o" in both parts should make it a literal.
            AssertTransliteration(@"T`no\p`o\pn", @" !""#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\]^_`abcdefghijklmopnqrstuvwxyz{|}~");
            AssertTransliteration(@"T`noo\p`o\pnq", @" !""#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\]^_`abcdefghijklmopqqrstuvwxyz{|}~");
            AssertTransliteration(@"T`no\p`Ro\pn", @" !""#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\]^_`abcdefghijklmopnqrstuvwxyz{|}~");
            AssertTransliteration(@"T`nRo\p`o\pn", @" !""#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\]^_`abcdefghijklmopnqrstuvwxyz{|}~");
        }

        [TestMethod]
        public void TestRemoval()
        {
            AssertTransliteration(@"T`abc`A_C", @" !""#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\]^_`ACdefghijklmnopqrstuvwxyz{|}~");
            AssertTransliteration(@"T`abc`A_", @" !""#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\]^_`Adefghijklmnopqrstuvwxyz{|}~");
            AssertTransliteration(@"T`abc", @" !""#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\]^_`defghijklmnopqrstuvwxyz{|}~");
        }

        [TestMethod]
        public void TestOverlappingMatches()
        {
            AssertTransliteration(@"Tv`!d`@!d`\d+", @" !""#$%&'()*+,-./!!!!!!!!!!:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\]^_`abcdefghijklmnopqrstuvwxyz{|}~");
            AssertTransliteration(@"Trv`_d!`d!@`\d+", @" !""#$%&'()*+,-./!!!!!!!!!!:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\]^_`abcdefghijklmnopqrstuvwxyz{|}~");
            AssertTransliteration(@"Tw`_d`d`[0123]+", @" !""#$%&'()*+,-./4787456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\]^_`abcdefghijklmnopqrstuvwxyz{|}~");
            AssertTransliteration(@"Trw`_d`d`[0123]+", @" !""#$%&'()*+,-./4787456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\]^_`abcdefghijklmnopqrstuvwxyz{|}~");
        }

        // Applies the transliteration to all printable ASCII characters
        private void AssertTransliteration(string source, string expectedOutput)
        {
            string printableAscii = @" !""#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\]^_`abcdefghijklmnopqrstuvwxyz{|}~";

            AssertProgram(new TestSuite { Sources = { source }, TestCases = { { printableAscii, expectedOutput } } });
        }
    }
}
