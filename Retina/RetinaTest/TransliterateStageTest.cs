using System;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Retina;
using Retina.Stages;

namespace RetinaTest
{
    [TestClass]
    public class TransliterateStageTest
    {
        [TestMethod]
        public void TestBasicTransliteration()
        {
            AssertTransliteration(@"a`b`.\b|\b.", RegexOptions.None, "abc aba bab", "bbc bbb bab");
            AssertTransliteration(@"ab`ba`a\b|\ba", RegexOptions.None, "abc aba bab", "bbc bbb bab");
            AssertTransliteration(@"ab`ba`.\b|\b.", RegexOptions.None, "abc aba bab", "bbc bbb aaa");
        }

        [TestMethod]
        public void TestRTLMatching()
        {
            AssertTransliteration(@"a`b`a*", RegexOptions.RightToLeft, "abc aaa bab", "bbc bbb bbb"); 
            AssertTransliteration(@"ab`ba`a*", RegexOptions.RightToLeft, "abc aaa bab", "bbc bbb bbb");
            AssertTransliteration(@"ab`ba`.*", RegexOptions.None, "abc aba bab", "bac bab aba");
        }

        [TestMethod]
        public void TestMissingRegex()
        {
            AssertTransliteration(@"a`b", RegexOptions.None, "abc aba bab", "bbc bbb bbb");
            AssertTransliteration(@"ab`ba", RegexOptions.None, "abc aba bab", "bac bab aba");
            AssertTransliteration(@"a`b`", RegexOptions.None, "abc aba bab", "bbc bbb bbb");
            AssertTransliteration(@"ab`ba`", RegexOptions.None, "abc aba bab", "bac bab aba");
        }

        [TestMethod]
        public void TestShorterTarget()
        {
            AssertTransliteration(@"0123456`abc", RegexOptions.None, "0123456789", "abccccc789");
        }

        [TestMethod]
        public void TestMissingTarget()
        {
            AssertTransliteration(@"0123456", RegexOptions.None, "0123456789", "789");
            AssertTransliteration(@"0123456`", RegexOptions.None, "0123456789", "789");
            AssertTransliteration(@"0123456``.+", RegexOptions.None, "0123456789", "789");
        }

        [TestMethod]
        public void TestEscapeSequences()
        {
            AssertTransliteration(@"\a\b\f\n`\r\t\v\", RegexOptions.None, "\a\b\f\n\r\t\v\\", "\r\t\v\\\r\t\v\\");
            AssertTransliteration(@"\A\``\`B", RegexOptions.None, "AB`BA", "`BBB`");
            AssertTransliteration(@"a\-z`012", RegexOptions.None, "abc-xyz", "0bc1xy2");
        }

        [TestMethod]
        public void TestRanges()
        {
            AssertTransliteration(@"a-z`A-Z", RegexOptions.None, "Hello, World!", "HELLO, WORLD!");
            AssertTransliteration(@"a-z`z-a", RegexOptions.None, "Hello, World!", "Hvool, Wliow!");
            AssertTransliteration(@"z-a`a-z", RegexOptions.None, "Hello, World!", "Hvool, Wliow!");
            AssertTransliteration(@"--0`123", RegexOptions.None, "-./0", "1233");
            AssertTransliteration(@"0--`321", RegexOptions.None, "-./0", "1123");
            AssertTransliteration(@"\n- `a-z", RegexOptions.None, "\n ", "aw");
            AssertTransliteration(@" -\n`z-a", RegexOptions.None, "\n ", "dz");
            
            // Hyphens at the start or end are literals:
            AssertTransliteration(@"-xy`123", RegexOptions.None, "-vwxyz", "1vw23z");
            AssertTransliteration(@"bc-`123", RegexOptions.None, "-abcde", "3a12de");
        }

        [TestMethod]
        public void TestClasses()
        {
            string printableAscii = @" !""#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\]^_`abcdefghijklmnopqrstuvwxyz{|}~";
            AssertTransliteration(@"d`a-j", RegexOptions.None, printableAscii,
                                  @" !""#$%&'()*+,-./abcdefghij:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\]^_`abcdefghijklmnopqrstuvwxyz{|}~");
            AssertTransliteration(@"w`dA-Za-z\_", RegexOptions.None, printableAscii,
                                  @" !""#$%&'()*+,-./123456789A:;<=>?@BCDEFGHIJKLMNOPQRSTUVWXYZa[\]^0`bcdefghijklmnopqrstuvwxyz_{|}~");
            AssertTransliteration(@"H`a-p", RegexOptions.None, printableAscii,
                                  @" !""#$%&'()*+,-./abcdefghij:;<=>?@klmnopGHIJKLMNOPQRSTUVWXYZ[\]^_`abcdefghijklmnopqrstuvwxyz{|}~");
            AssertTransliteration(@"h`A-P", RegexOptions.None, printableAscii,
                                  @" !""#$%&'()*+,-./ABCDEFGHIJ:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\]^_`KLMNOPghijklmnopqrstuvwxyz{|}~");
            AssertTransliteration(@"L`a-z", RegexOptions.None, printableAscii,
                                  @" !""#$%&'()*+,-./0123456789:;<=>?@abcdefghijklmnopqrstuvwxyz[\]^_`abcdefghijklmnopqrstuvwxyz{|}~");
            AssertTransliteration(@"l`A-Z", RegexOptions.None, printableAscii,
                                  @" !""#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\]^_`ABCDEFGHIJKLMNOPQRSTUVWXYZ{|}~");
            AssertTransliteration(@"p`!-~ ", RegexOptions.None, printableAscii,
                                  @"!""#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\]^_`abcdefghijklmnopqrstuvwxyz{|}~ ");
            AssertTransliteration(@"E`a-e", RegexOptions.None, printableAscii,
                                  @" !""#$%&'()*+,-./a1b3c5d7e9:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\]^_`abcdefghijklmnopqrstuvwxyz{|}~");
            AssertTransliteration(@"O`a-e", RegexOptions.None, printableAscii,
                                  @" !""#$%&'()*+,-./0a2b4c6d8e:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\]^_`abcdefghijklmnopqrstuvwxyz{|}~");

            // Classes in ranges are ignored:
            AssertTransliteration(@"d-f`123", RegexOptions.None, printableAscii,
                                  @" !""#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\]^_`abc123ghijklmnopqrstuvwxyz{|}~");
            AssertTransliteration(@"f-d`321", RegexOptions.None, printableAscii,
                                  @" !""#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\]^_`abc123ghijklmnopqrstuvwxyz{|}~");
        }

        [TestMethod]
        public void TestReverseRange()
        {
            string printableAscii = @" !""#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\]^_`abcdefghijklmnopqrstuvwxyz{|}~";
            AssertTransliteration(@"Rd`a-j", RegexOptions.None, printableAscii,
                                  @" !""#$%&'()*+,-./jihgfedcba:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\]^_`abcdefghijklmnopqrstuvwxyz{|}~");
            AssertTransliteration(@"RRd`a-j", RegexOptions.None, printableAscii,
                                  @" !""#$%&'()*+,-./abcdefghij:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\]^_`abcdefghijklmnopqrstuvwxyz{|}~");
            AssertTransliteration(@"RRRd`a-j", RegexOptions.None, printableAscii,
                                  @" !""#$%&'()*+,-./jihgfedcba:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\]^_`abcdefghijklmnopqrstuvwxyz{|}~");
            AssertTransliteration(@"Rw`w", RegexOptions.None, printableAscii,
                                  @" !""#$%&'()*+,-./yxwvutsrqp:;<=>?@onmlkjihgfedcbaZYXWVUTSRQP[\]^z`ONMLKJIHGFEDCBA9876543210_{|}~");
            AssertTransliteration(@"a-z`Ra-z", RegexOptions.None, "Hello, World!", "Hvool, Wliow!");
            AssertTransliteration(@"Ra-z`a-z", RegexOptions.None, "Hello, World!", "Hvool, Wliow!");
            AssertTransliteration(@"z-a`Rz-a", RegexOptions.None, "Hello, World!", "Hvool, Wliow!");
            AssertTransliteration(@"Rz-a`z-a", RegexOptions.None, "Hello, World!", "Hvool, Wliow!");
            AssertTransliteration(@"R\\-Z`\\-Z", RegexOptions.None, @"RXYZ[\]^", @"RXY\[Z]^");

            // R not followed by a range or class should be literal R, as should R as part of a range.
            AssertTransliteration(@"a-zR`a-z", RegexOptions.None, printableAscii,
                                  @" !""#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQzSTUVWXYZ[\]^_`abcdefghijklmnopqrstuvwxyz{|}~");
            AssertTransliteration(@"Rab-z`Qa-z", RegexOptions.None, printableAscii,
                                  @" !""#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQQSTUVWXYZ[\]^_`abcdefghijklmnopqrstuvwxyz{|}~");
            AssertTransliteration(@"R-Z`Z-R", RegexOptions.None, printableAscii,
                                  @" !""#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQZYXWVUTSR[\]^_`abcdefghijklmnopqrstuvwxyz{|}~");
            AssertTransliteration(@"RR-Z`R-Z", RegexOptions.None, printableAscii,
                                  @" !""#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQZYXWVUTSR[\]^_`abcdefghijklmnopqrstuvwxyz{|}~");
            AssertTransliteration(@"RRR-Z`R-Z", RegexOptions.None, printableAscii,
                                  @" !""#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\]^_`abcdefghijklmnopqrstuvwxyz{|}~");

        }

        [TestMethod]
        public void TestReferencingOtherSet()
        {
            string printableAscii = @" !""#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\]^_`abcdefghijklmnopqrstuvwxyz{|}~";
            AssertTransliteration(@"#$%&'()*+,-./`/o", RegexOptions.None, printableAscii,
                                  @" !""/#$%&'()*+,-.0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\]^_`abcdefghijklmnopqrstuvwxyz{|}~");
            AssertTransliteration(@"/o`#$%&'()*+,-./", RegexOptions.None, printableAscii,
                                  @" !""$%&'()*+,-./#0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\]^_`abcdefghijklmnopqrstuvwxyz{|}~");
            AssertTransliteration(@"#$%&'()*+,-./`Ro", RegexOptions.None, printableAscii,
                                  @" !""/.-,+*)('&%$#0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\]^_`abcdefghijklmnopqrstuvwxyz{|}~");
            AssertTransliteration(@"Ro`#$%&'()*+,-./", RegexOptions.None, printableAscii,
                                  @" !""/.-,+*)('&%$#0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\]^_`abcdefghijklmnopqrstuvwxyz{|}~");

            // Using "o" twice inserts literal "o"s because inserting the other set twice is pointless
            AssertTransliteration(@"#$%&'()*+,-./`/ofoo", RegexOptions.None, printableAscii,
                                  @" !""/#$%&'()*+,-.0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\]^_`abcdefghijklmnopqrstuvwxyz{|}~");
            AssertTransliteration(@"/ofoo`#$%&'()*+,-./", RegexOptions.None, printableAscii,
                                  @" !""$%&'()*+,-./#0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\]^_`abcde/ghijklmn/pqrstuvwxyz{|}~");
            AssertTransliteration(@"#$%&'()*+,-./`Rofoo", RegexOptions.None, printableAscii,
                                  @" !""/.-,+*)('&%$#0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\]^_`abcdefghijklmnopqrstuvwxyz{|}~");
            AssertTransliteration(@"Rofoo`#$%&'()*+,-./", RegexOptions.None, printableAscii,
                                  @" !""/.-,+*)('&%$#0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\]^_`abcde/ghijklmn/pqrstuvwxyz{|}~");

            // Using "o" in both parts should make it a literal.
            AssertTransliteration(@"no\p`o\pn", RegexOptions.None, printableAscii,
                                  @" !""#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\]^_`abcdefghijklmopnqrstuvwxyz{|}~");
            AssertTransliteration(@"noo\p`o\pnq", RegexOptions.None, printableAscii,
                                  @" !""#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\]^_`abcdefghijklmopqqrstuvwxyz{|}~");
            AssertTransliteration(@"no\p`Ro\pn", RegexOptions.None, printableAscii,
                                  @" !""#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\]^_`abcdefghijklmopnqrstuvwxyz{|}~");
            AssertTransliteration(@"nRo\p`o\pn", RegexOptions.None, printableAscii,
                                  @" !""#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\]^_`abcdefghijklmopnqrstuvwxyz{|}~");
        }

        [TestMethod]
        public void TestRemoval()
        {
            AssertTransliteration(@"abc`A_C", RegexOptions.None, "abcdef", @"ACdef");
            AssertTransliteration(@"abc`A_", RegexOptions.None, "abcdef", @"Adef");
            AssertTransliteration(@"abc", RegexOptions.None, "abcdef", @"def");
        }

        private void AssertTransliteration(string pattern, RegexOptions rgxOptions, string input, string expectedOutput)
        {
            var options = new Options("", Modes.Transliterate);
            options.RegexOptions = rgxOptions;
            options.Silent = true;

            var stage = new TransliterateStage(options, pattern);

            Assert.AreEqual(expectedOutput, stage.Execute(input).ToString());
        }
    }
}
