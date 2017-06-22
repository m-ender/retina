using System;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Retina;
using Retina.Stages;
using System.Collections.Generic;

namespace RetinaTest
{
    [TestClass]
    public class ReplaceStageTest : RetinaTestBase
    {
        [TestMethod]
        public void TestBasicReplacement()
        {
            AssertProgram(new TestSuite
            {
                Sources = { "a*ab", "x" },
                TestCases = {
                    { "", "" },
                    { "baaaaabbabaaabaa", "bxbxxaa" },
                }
            });

            AssertProgram(new TestSuite { Sources = { ".+", "ab" }, TestCases = {{ "ccc", "ab" }} });
        }

        [TestMethod]
        public void TestEmptyMatch()
        {
            AssertProgram(new TestSuite { Sources = { "", "x" }, TestCases = { { "abc", "xaxbxcx" } } });

            AssertProgram(new TestSuite
            {
                Sources = { "^", "abc" },
                TestCases = {
                    { "", "abc" },
                    { "a", "abca" },
                }
            });

            AssertProgram(new TestSuite { Sources = { "(?=a)", "b" }, TestCases = { { "abaca", "babbacba" } } });

            AssertProgram(new TestSuite
            {
                Sources = { ".*", "ab" },
                TestCases = {
                    { "", "ab" },
                    { "ccc", "abab" },
                }
            });


            AssertProgram(new TestSuite { Sources = { "a*", "x" }, TestCases = { { "aacaccaa", "xxcxxcxcxx" } } });
        }

        [TestMethod]
        public void TestRTLMatching()
        {
            AssertProgram(new TestSuite { Sources = { "r`a+", "x" }, TestCases = { { "aacaccaa", "xcxccx" } } });
            AssertProgram(new TestSuite { Sources = { "r`a*", "x" }, TestCases = { { "aacaccaa", "xxcxxcxcxx" } } });
            AssertProgram(new TestSuite { Sources = { "r`(.)+", "$1" }, TestCases = { { "abc", "a" } } });
            AssertProgram(new TestSuite { Sources = { "r`(.)*", "$1x" }, TestCases = { { "abc", "xax" } } });
            AssertProgram(new TestSuite { Sources = { "r`(.)(.)", "$2$1" }, TestCases = { { "abcde", "acbed" } } });
        }

        [TestMethod]
        public void TestNumberedGroups()
        {
            AssertProgram(new TestSuite { Sources = { "(.)", "$1$1" }, TestCases = { { "abc", "aabbcc" } } });
            AssertProgram(new TestSuite { Sources = { "(.)", "$2" }, TestCases = { { "abc", "$2$2$2" } } });
            AssertProgram(new TestSuite { Sources = { "(.)|(a)", "$2" }, TestCases = { { "abc", "" } } });
            AssertProgram(new TestSuite { Sources = { "(a)|(.)", "$2" }, TestCases = { { "abc", "bc" } } });
            AssertProgram(new TestSuite { Sources = { "(.)", "${1}$1" }, TestCases = { { "abc", "aabbcc" } } });
            AssertProgram(new TestSuite {
                Sources = {
                    "(.)(.)(.)(.)(.)(.)(.)(.)(.)(.)(.)(.)",
                    "$111${1}11${11}1${111}$000011" },
                TestCases = {
                    { "abcdefghijklABCDEFGHIJKL", "$111a11k1${111}k$111A11K1${111}K" }
                }
            });
            AssertProgram(new TestSuite { Sources = { "(.)+", "$1" }, TestCases = { { "abc", "c" } } });
            AssertProgram(new TestSuite { Sources = { "(.)*", "$1x" }, TestCases = { { "abc", "cxx" } } });
            AssertProgram(new TestSuite { Sources = { "(.)+(?<-1>.)", "$1" }, TestCases = { { "abcd", "b" } } });
            AssertProgram(new TestSuite { Sources = { "(?<2>.)(.)(.)", "$1$2$3" }, TestCases = { { "abc", "bc$3" } } });
            AssertProgram(new TestSuite { Sources = { "(?<1>.)(.)(.)", "$1$2$3" }, TestCases = { { "abc", "bc$3" } } });
            AssertProgram(new TestSuite { Sources = { "(?<001>.)(?<01>.)(?<1>.)(.)", "$1$2$3$4" }, TestCases = { { "abcd", "d$2$3$4" } } });
            AssertProgram(new TestSuite { Sources = { "(?<a>.)(.)(?<b>.)(.)", "$1$2$3$4$5" }, TestCases = { { "abcd", "bdac$5" } } });
        }

        [TestMethod]
        public void TestNamedGroups()
        {
            AssertProgram(new TestSuite { Sources = { "(?<a>.)", "${a}${a}" }, TestCases = { { "abc", "aabbcc" } } });
            AssertProgram(new TestSuite { Sources = { "(?<a>.)", "${b}${b}" }, TestCases = { { "abc", "${b}${b}${b}${b}${b}${b}" } } });
            AssertProgram(new TestSuite { Sources = { "(?<a>.)(.)", "$1$2${a}" }, TestCases = { { "abcd", "baadcc" } } });
            AssertProgram(new TestSuite { Sources = { "(?<_a1>.)", "${_a1}$1" }, TestCases = { { "abc", "aabbcc" } } });
            AssertProgram(new TestSuite { Sources = { "(?<a>.)(?<A>.)", "${A}${a}" }, TestCases = { { "abcd", "badc" } } });
        }

        [TestMethod]
        public void TestDollar()
        {
            AssertProgram(new TestSuite { Sources = { "(.)", "$1$" }, TestCases = { { "abc", "a$b$c$" } } });
            AssertProgram(new TestSuite { Sources = { "(.)", "$$1" }, TestCases = { { "abc", "$1$1$1" } } });
            AssertProgram(new TestSuite { Sources = { "(?<a>.)", "${a}$${a}" }, TestCases = { { "abc", "a${a}b${a}c${a}" } } });
        }

        [TestMethod]
        public void TestEntireMatch()
        {
            AssertProgram(new TestSuite { Sources = { "..", "$0$&" }, TestCases = { { "abcd", "ababcdcd" } } });
            AssertProgram(new TestSuite { Sources = { "(.)(.)", "$0$&" }, TestCases = { { "abcd", "ababcdcd" } } });
            AssertProgram(new TestSuite { Sources = { "(.)(.)", "${0}1$&" }, TestCases = { { "abcd", "ab1abcd1cd" } } });
            AssertProgram(new TestSuite { Sources = { "(.)(.)", "$01$&" }, TestCases = { { "abcd", "aabccd" } } });
            AssertProgram(new TestSuite { Sources = { "(?=(.))", "$0$&$1" }, TestCases = { { "abc", "aabbcc" } } });
            AssertProgram(new TestSuite { Sources = { "(.)(.)", "$0$00$000${0}${00}${000}" }, TestCases = { { "abcd", "ababababababcdcdcdcdcdcd" } } });
        }

        [TestMethod]
        public void TestSurroundingMatch()
        {
            AssertProgram(new TestSuite { Sources = { ".", "$`" }, TestCases = { { "abcd", "aababc" } } });
            AssertProgram(new TestSuite { Sources = { ".", "$'" }, TestCases = { { "abcd", "bcdcdd" } } });
            AssertProgram(new TestSuite { Sources = { "", "$`$'" }, TestCases = { { "abcd", "abcdaabcdbabcdcabcddabcd" } } });
        }

        [TestMethod]
        public void TestLastCapture()
        {
            AssertProgram(new TestSuite { Sources = { "(a)|(b)", "$+!" }, TestCases = { { "abab", "!b!!b!" } } });
            AssertProgram(new TestSuite { Sources = { "(?<a>a)|(b)", "$+!" }, TestCases = { { "abab", "a!!a!!" } } });
            AssertProgram(new TestSuite { Sources = { "(a)|(?<b>b)", "$+!" }, TestCases = { { "abab", "!b!!b!" } } });
            AssertProgram(new TestSuite { Sources = { "(?<a>a)|(?<b>b)", "$+!" }, TestCases = { { "abab", "!b!!b!" } } });
        }

        [TestMethod]
        public void TestEntireInput()
        {
            AssertProgram(new TestSuite { Sources = { ".", "$_" }, TestCases = { { "abcd", "abcdabcdabcdabcd" } } });
            AssertProgram(new TestSuite { Sources = { "", "$_" }, TestCases = { { "abcd", "abcdaabcdbabcdcabcddabcd" } } });
        }

        [TestMethod]
        public void TestInvalidSyntax()
        {
            AssertProgram(new TestSuite { Sources = { "(.)(.)", "${" }, TestCases = { { "abcd", "${${" } } });
            AssertProgram(new TestSuite { Sources = { "(.)(.)", "$}" }, TestCases = { { "abcd", "$}$}" } } });
            AssertProgram(new TestSuite { Sources = { "(.)(.)", "${}" }, TestCases = { { "abcd", "${}${}" } } });
            AssertProgram(new TestSuite { Sources = { "(.)(.)", "${$}" }, TestCases = { { "abcd", "${$}${$}" } } });
            AssertProgram(new TestSuite { Sources = { "(.)(.)", "${{1}}" }, TestCases = { { "abcd", "${{1}}${{1}}" } } });
            AssertProgram(new TestSuite { Sources = { "(.)(.)", "${$1}" }, TestCases = { { "abcd", "${a}${c}" } } });
            AssertProgram(new TestSuite { Sources = { "(?<a>.)(.)", "$a" }, TestCases = { { "abcd", "$a$a" } } });
        }

        [TestMethod]
        public void TestLinefeedEscape()
        {
            AssertProgram(new TestSuite { Sources = { "(.)(?<n>.)", "n$n${n}" }, TestCases = { { "abcd", "n\nbn\nd" } } });
            AssertProgram(new TestSuite { Sources = { "(.)(.)", "n$n${n}" }, TestCases = { { "abcd", "n\n${n}n\n${n}" } } });
        }

        [TestMethod]
        public void TestCaptureCount()
        {
            AssertProgram(new TestSuite { Sources = { "(.)+", "#$#1" }, TestCases = { { "abcd\ndef", "#4\n#3" } } });
            AssertProgram(new TestSuite { Sources = { "(..)+", "#$#1" }, TestCases = { { "abcd\ndef", "#2\n#1f" } } });
            AssertProgram(new TestSuite { Sources = { "()(.)*", "$#2" }, TestCases = { { "abcd\ndef", "40\n30" } } });
            AssertProgram(new TestSuite { Sources = { "(a)+(?<1>b)+", "$#1" }, TestCases = { { "aaabbcabbb", "5c4" } } });

            // Curly braces should also work:
            AssertProgram(new TestSuite { Sources = { "(.)+", "#$#{1}" }, TestCases = { { "abcd\ndef", "#4\n#3" } } });
            AssertProgram(new TestSuite { Sources = { "()(.)*", "$#{2}" }, TestCases = { { "abcd\ndef", "40\n30" } } });
            AssertProgram(new TestSuite { Sources = { "(a)+(?<1>b)+", "$#{1}$#{01}$#{001}" }, TestCases = { { "aaabbcabbb", "555c444" } } });
            AssertProgram(new TestSuite { Sources = { "(?<foo>a)+(?<bar>b)+", "$#{foo}$#{bar}" }, TestCases = { { "aaabbcabbb", "32c13" } } });

            // $# not followed by a valid group should be taken literally:
            AssertProgram(new TestSuite { Sources = { "(.)+", "$#2$#{2}$#$#}" }, TestCases = { { "abcd", "$#2$#{2}$#$#}" } } });
            AssertProgram(new TestSuite { Sources = { "(?<a>.)+", "$#a$#{b}$#{" }, TestCases = { { "abcd", "$#a$#{b}$#{" } } });

            // $#0 is useless but should also work:
            AssertProgram(new TestSuite { Sources = { "(.)+", "$#0$#{0}$#{00}" }, TestCases = { { "abcd\ndef", "111\n111" } } });

            // $#+ should work
            AssertProgram(new TestSuite { Sources = { "()(.)*", "$#+" }, TestCases = { { "abcd\ndef", "40\n30" } } });
        }

        [TestMethod]
        public void TestCaptureLength()
        {
            AssertProgram(new TestSuite { Sources = { ".+", ".$.0" }, TestCases = { { "abcd\ndef", ".4\n.3" } } });
            AssertProgram(new TestSuite { Sources = { "(.)+", ".$.1" }, TestCases = { { "abcd\ndef", ".1\n.1" } } });
            AssertProgram(new TestSuite { Sources = { "(.+)", ".$.1" }, TestCases = { { "abcd\ndef", ".4\n.3" } } });
            AssertProgram(new TestSuite { Sources = { "()(.*)", "$.2" }, TestCases = { { "abcd\ndef", "40\n30" } } });

            // Curly braces should also work:
            AssertProgram(new TestSuite { Sources = { ".+", ".$.{0}" }, TestCases = { { "abcd\ndef", ".4\n.3" } } });
            AssertProgram(new TestSuite { Sources = { "(.+)", ".$.{1}" }, TestCases = { { "abcd\ndef", ".4\n.3" } } });
            AssertProgram(new TestSuite { Sources = { "()(.*)", "$.{2}" }, TestCases = { { "abcd\ndef", "40\n30" } } });
            AssertProgram(new TestSuite { Sources = { "(?<foo>a+)(?<bar>b+)", "$.{foo}$.{bar}" }, TestCases = { { "aaabbcabbb", "32c13" } } });

            // Make sure $`, $', $_, $& and $+ work:
            AssertProgram(new TestSuite { Sources = { "a", "$.`" }, TestCases = { { ";!~&a@#", ";!~&4@#" } } });
            AssertProgram(new TestSuite { Sources = { "a", "$.'" }, TestCases = { { ";!~&a@#", ";!~&2@#" } } });
            AssertProgram(new TestSuite { Sources = { "a", "$._" }, TestCases = { { ";!~&a@#", ";!~&7@#" } } });
            AssertProgram(new TestSuite { Sources = { "a+", "$.&" }, TestCases = { { ";!~&aaa@#", ";!~&3@#" } } });
            AssertProgram(new TestSuite { Sources = { "()(.*)", "$.+" }, TestCases = { { "abcd\ndef", "40\n30" } } });

            // An entirely unmatched group should result in an empty string, not 0:
            AssertProgram(new TestSuite { Sources = { "a|(b)", "$.1" }, TestCases = { { "::a::b::", "::::1::" } } });
            AssertProgram(new TestSuite { Sources = { "()(.)*", "$.2" }, TestCases = { { "abcd\ndef", "1\n1" } } });
            AssertProgram(new TestSuite { Sources = { "()(.)*", "$.+" }, TestCases = { { "abcd\ndef", "1\n1" } } });

            // $. not followed by a valid group be taken literally:
            AssertProgram(new TestSuite { Sources = { "(.+)", "$.2$.{2}$.$.}" }, TestCases = { { "abcd", "$.2$.{2}$.$.}" } } });
            AssertProgram(new TestSuite { Sources = { "(?<a>.+)", "$.a$.{b}$.{" }, TestCases = { { "abcd", "$.a$.{b}$.{" } } });
        }

        [TestMethod]
        public void TestCharacterRepetition()
        {
            AssertProgram(new TestSuite { Sources = { "(.)+", "15$*_" }, TestCases = { { "abc", "_______________" } } });
            AssertProgram(new TestSuite { Sources = { "(.)+", "xyz$*_" }, TestCases = { { "abc", "xy" } } });
            AssertProgram(new TestSuite { Sources = { @"\d+", "$0$*1" }, TestCases = { { "3 12 4", "111 111111111111 1111" } } });
            AssertProgram(new TestSuite { Sources = { ".+", "$0$*1" }, TestCases = { { "3 12 4", "111" } } });
            AssertProgram(new TestSuite { Sources = { ".+", "$0$*1" }, TestCases = { { "abc10def", "1111111111" } } });
            AssertProgram(new TestSuite { Sources = { ".+", "$0$*1" }, TestCases = { { "abc0def", "" } } });
            AssertProgram(new TestSuite { Sources = { "(.)+", "$#1$*1" }, TestCases = { { "abcd\ndef", "1111\n111" } } });
            AssertProgram(new TestSuite { Sources = { "(..)+", "$#1$*1" }, TestCases = { { "abcd\ndef", "11\n1f" } } });
            AssertProgram(new TestSuite { Sources = { "(?<a>.)+", "$#{a}$*1" }, TestCases = { { "abcd\ndef", "1111\n111" } } });
            AssertProgram(new TestSuite { Sources = { "$", " $`$*1" }, TestCases = { { "12 123", "12 123 111111111111" } } });
            AssertProgram(new TestSuite { Sources = { "$", " $_$*1" }, TestCases = { { "12 123", "12 123 111111111111" } } });
            AssertProgram(new TestSuite { Sources = { "^", "$'$*1 " }, TestCases = { { "12 123", "111111111111 12 123" } } });

            AssertProgram(new TestSuite { Sources = { ".+", "$0$*$" }, TestCases = { { "5", "$$$$$" } } });
            AssertProgram(new TestSuite { Sources = { ".+", @"$0$*\" }, TestCases = { { "5", @"\\\\\" } } });
            AssertProgram(new TestSuite { Sources = { ".+", "$0$**" }, TestCases = { { "5", "*****" } } });

            // $* can be nested:
            AssertProgram(new TestSuite { Sources = { ".+", "2$*3$*_" }, TestCases = { { "abc", "_________________________________" } } }); // 2 --> 33 --> thirty-three underscores
            AssertProgram(new TestSuite { Sources = { ".+", "2$*_$*1" }, TestCases = { { "abc", "" } } });

            // $* as the first token implies $&. $* as the last token implies 1.
            AssertProgram(new TestSuite { Sources = { "(.).+", "$*:" }, TestCases = { { "10!", "::::::::::" } } });
            AssertProgram(new TestSuite { Sources = { ".+", "5$*" }, TestCases = { { "a3c", "11111" } } });
            AssertProgram(new TestSuite { Sources = { "(.).+", "$*" }, TestCases = { { "10!", "1111111111" } } });
        }

        [TestMethod]
        public void TestLineOnly()
        {
            AssertProgram(new TestSuite { Sources = { "a", "$%_" }, TestCases = { { "abc\nbab\naba", "abcbc\nbbabb\nabababa" } } });
            AssertProgram(new TestSuite { Sources = { "a", "$%`" }, TestCases = { { "abc\nbab\naba", "bc\nbbb\nbab" } } });
            AssertProgram(new TestSuite { Sources = { "a", "$%'" }, TestCases = { { "abc\nbab\naba", "bcbc\nbbb\nbab" } } });
            AssertProgram(new TestSuite { Sources = { ".", "$.%`" }, TestCases = { { "abc\ndefh\nhijlk", "012\n0123\n01234" } } });
            AssertProgram(new TestSuite { Sources = { ".", "$.%'" }, TestCases = { { "abc\ndefh\nhijlk", "210\n3210\n43210" } } });
        }
    }
}
