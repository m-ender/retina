using System;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Retina;
using Retina.Stages;

namespace RetinaTest
{
    [TestClass]
    public class ReplaceStageTest
    {
        [TestMethod]
        public void TestBasicReplacement()
        {
            AssertReplacement("a*ab", "x", RegexOptions.None, "", "");
            AssertReplacement("a*ab", "x", RegexOptions.None, "baaaaabbabaaabaa", "bxbxxaa");
            AssertReplacement(".+", "ab", RegexOptions.None, "ccc", "ab");
        }

        [TestMethod]
        public void TestEmptyMatch()
        {
            AssertReplacement("", "x", RegexOptions.None, "abc", "xaxbxcx");
            AssertReplacement("^", "abc", RegexOptions.None, "", "abc");
            AssertReplacement("^", "abc", RegexOptions.None, "a", "abca");
            AssertReplacement("(?=a)", "b", RegexOptions.None, "abaca", "babbacba");
            AssertReplacement(".*", "ab", RegexOptions.None, "", "ab");
            AssertReplacement(".*", "ab", RegexOptions.None, "ccc", "abab");
            AssertReplacement("a*", "x", RegexOptions.None, "aacaccaa", "xxcxxcxcxx");
        }

        [TestMethod]
        public void TestRTLMatching()
        {
            AssertReplacement("a+", "x", RegexOptions.RightToLeft, "aacaccaa", "xcxccx");
            AssertReplacement("a*", "x", RegexOptions.RightToLeft, "aacaccaa", "xxcxxcxcxx");
            AssertReplacement("(.)+", "$1", RegexOptions.RightToLeft, "abc", "a");
            AssertReplacement("(.)*", "$1x", RegexOptions.RightToLeft, "abc", "xax");
            AssertReplacement("(.)(.)", "$2$1", RegexOptions.RightToLeft, "abcde", "acbed");
        }

        [TestMethod]
        public void TestNumberedGroups()
        {
            AssertReplacement("(.)", "$1$1", RegexOptions.None, "abc", "aabbcc");
            AssertReplacement("(.)", "$2", RegexOptions.None, "abc", "$2$2$2");
            AssertReplacement("(.)|(a)", "$2", RegexOptions.None, "abc", "");
            AssertReplacement("(a)|(.)", "$2", RegexOptions.None, "abc", "bc");
            AssertReplacement("(.)", "${1}$1", RegexOptions.None, "abc", "aabbcc");
            AssertReplacement("(.)(.)(.)(.)(.)(.)(.)(.)(.)(.)(.)(.)",
                              "$111${1}11${11}1${111}$000011",
                              RegexOptions.None,
                              "abcdefghijklABCDEFGHIJKL",
                              "$111a11k1${111}k$111A11K1${111}K");
            AssertReplacement("(.)+", "$1", RegexOptions.None, "abc", "c");
            AssertReplacement("(.)*", "$1x", RegexOptions.None, "abc", "cxx");
            AssertReplacement("(.)+(?<-1>.)", "$1", RegexOptions.None, "abcd", "b");
            AssertReplacement("(?<2>.)(.)(.)", "$1$2$3", RegexOptions.None, "abc", "bc$3");
            AssertReplacement("(?<1>.)(.)(.)", "$1$2$3", RegexOptions.None, "abc", "bc$3");
            AssertReplacement("(?<001>.)(?<01>.)(?<1>.)(.)", "$1$2$3$4", RegexOptions.None, "abcd", "d$2$3$4");
            AssertReplacement("(?<a>.)(.)(?<b>.)(.)", "$1$2$3$4$5", RegexOptions.None, "abcd", "bdac$5");
        }

        [TestMethod]
        public void TestNamedGroups()
        {
            AssertReplacement("(?<a>.)", "${a}${a}", RegexOptions.None, "abc", "aabbcc");
            AssertReplacement("(?<a>.)", "${b}${b}", RegexOptions.None, "abc", "${b}${b}${b}${b}${b}${b}");
            AssertReplacement("(?<a>.)(.)", "$1$2${a}", RegexOptions.None, "abcd", "baadcc");
            AssertReplacement("(?<_a1>.)", "${_a1}$1", RegexOptions.None, "abc", "aabbcc");
            AssertReplacement("(?<a>.)(?<A>.)", "${A}${a}", RegexOptions.None, "abcd", "badc");
        }

        [TestMethod]
        public void TestDollar()
        {
            AssertReplacement("(.)", "$1$", RegexOptions.None, "abc", "a$b$c$");
            AssertReplacement("(.)", "$$1", RegexOptions.None, "abc", "$1$1$1");
            AssertReplacement("(?<a>.)", "${a}$${a}", RegexOptions.None, "abc", "a${a}b${a}c${a}");
        }

        [TestMethod]
        public void TestEntireMatch()
        {
            AssertReplacement("..", "$0$&", RegexOptions.None, "abcd", "ababcdcd");
            AssertReplacement("(.)(.)", "$0$&", RegexOptions.None, "abcd", "ababcdcd");
            AssertReplacement("(.)(.)", "${0}1$&", RegexOptions.None, "abcd", "ab1abcd1cd");
            AssertReplacement("(.)(.)", "$01$&", RegexOptions.None, "abcd", "aabccd");
            AssertReplacement("(?=(.))", "$0$&$1", RegexOptions.None, "abc", "aabbcc");
            AssertReplacement("(.)(.)", "$0$00$000${0}${00}${000}", RegexOptions.None, "abcd", "ababababababcdcdcdcdcdcd");
        }

        [TestMethod]
        public void TestSurroundingMatch()
        {
            AssertReplacement(".", "$`", RegexOptions.None, "abcd", "aababc");
            AssertReplacement(".", "$'", RegexOptions.None, "abcd", "bcdcdd");
            AssertReplacement("", "$`$'", RegexOptions.None, "abcd", "abcdaabcdbabcdcabcddabcd");
        }

        [TestMethod]
        public void TestLastCapture()
        {
            AssertReplacement("(a)|(b)", "$+!", RegexOptions.None, "abab", "!b!!b!");
            AssertReplacement("(?<a>a)|(b)", "$+!", RegexOptions.None, "abab", "a!!a!!");
            AssertReplacement("(a)|(?<b>b)", "$+!", RegexOptions.None, "abab", "!b!!b!");
            AssertReplacement("(?<a>a)|(?<b>b)", "$+!", RegexOptions.None, "abab", "!b!!b!");
        }

        [TestMethod]
        public void TestEntireInput()
        {
            AssertReplacement(".", "$_", RegexOptions.None, "abcd", "abcdabcdabcdabcd");
            AssertReplacement("", "$_", RegexOptions.None, "abcd", "abcdaabcdbabcdcabcddabcd");
        }

        [TestMethod]
        public void TestInvalidSyntax()
        {
            AssertReplacement("(.)(.)", "${", RegexOptions.None, "abcd", "${${");
            AssertReplacement("(.)(.)", "$}", RegexOptions.None, "abcd", "$}$}");
            AssertReplacement("(.)(.)", "${}", RegexOptions.None, "abcd", "${}${}");
            AssertReplacement("(.)(.)", "${$}", RegexOptions.None, "abcd", "${$}${$}");
            AssertReplacement("(.)(.)", "${{1}}", RegexOptions.None, "abcd", "${{1}}${{1}}");
            AssertReplacement("(?<a>.)(.)", "$a", RegexOptions.None, "abcd", "$a$a");
        }

        private void AssertReplacement(string regex, string replacement, RegexOptions rgxOptions, string input, string expectedOutput)
        {
            var options = new Options("", Modes.Replace);
            options.RegexOptions = rgxOptions;
            options.Silent = true;

            var stage = new ReplaceStage(options, regex, replacement);

            Assert.AreEqual(expectedOutput, stage.Execute(input).ToString());
        }
    }
}
