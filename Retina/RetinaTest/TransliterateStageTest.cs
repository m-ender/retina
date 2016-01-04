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
            // TODO: Make escapes work at the end of ranges.
        }

        [TestMethod]
        public void TestClasses()
        {
            string printableAscii = @" !""#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\]^_`abcdefghijklmnopqrstuvwxyz{|}~";
            AssertTransliteration(@"d`a-j", RegexOptions.None, printableAscii,
                                  @" !""#$%&'()*+,-./abcdefghij:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\]^_`abcdefghijklmnopqrstuvwxyz{|}~");
            AssertTransliteration(@"w`dA-Za-z_", RegexOptions.None, printableAscii,
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
                                  @"!""#$%&'()*+,-./ABCDEFGHIJ:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\]^_`KLMNOPghijklmnopqrstuvwxyz{|}~ ");
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
