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
    public class ReplacerTest : RetinaTestBase
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
            AssertProgram(new TestSuite { Sources = { "(.)", "$2" }, TestCases = { { "abc", "" } } });
            AssertProgram(new TestSuite { Sources = { "(.)|(a)", "$2" }, TestCases = { { "abc", "" } } });
            AssertProgram(new TestSuite { Sources = { "(a)|(.)", "$2" }, TestCases = { { "abc", "bc" } } });
            AssertProgram(new TestSuite { Sources = { "(.)", "${1}$1" }, TestCases = { { "abc", "aabbcc" } } });
            AssertProgram(new TestSuite {
                Sources = {
                    "(.)(.)(.)(.)(.)(.)(.)(.)(.)(.)(.)(.)",
                    "$111${1}11${11}1${111}$000011" },
                TestCases = {
                    { "abcdefghijklABCDEFGHIJKL", "a11k1kA11K1K" }
                }
            });
            AssertProgram(new TestSuite { Sources = { "(.)+", "$1" }, TestCases = { { "abc", "c" } } });
            AssertProgram(new TestSuite { Sources = { "(.)*", "$1x" }, TestCases = { { "abc", "cxx" } } });
            AssertProgram(new TestSuite { Sources = { "(.)+(?<-1>.)", "$1" }, TestCases = { { "abcd", "b" } } });
            AssertProgram(new TestSuite { Sources = { "(?<2>.)(.)(.)", "$1$2$3" }, TestCases = { { "abc", "bc" } } });
            AssertProgram(new TestSuite { Sources = { "(?<1>.)(.)(.)", "$1$2$3" }, TestCases = { { "abc", "bc" } } });
            AssertProgram(new TestSuite { Sources = { "(?<001>.)(?<01>.)(?<1>.)(.)", "$1$2$3$4" }, TestCases = { { "abcd", "d" } } });
            AssertProgram(new TestSuite { Sources = { "(?<a>.)(.)(?<b>.)(.)", "$1$2$3$4$5" }, TestCases = { { "abcd", "bdac" } } });
        }

        [TestMethod]
        public void TestNamedGroups()
        {
            AssertProgram(new TestSuite { Sources = { "(?<a>.)", "${a}${a}" }, TestCases = { { "abc", "aabbcc" } } });
            AssertProgram(new TestSuite { Sources = { "(?<a>.)", "${b}${b}" }, TestCases = { { "abc", "" } } });
            AssertProgram(new TestSuite { Sources = { "(?<a>.)(.)", "$1$2${a}" }, TestCases = { { "abcd", "baadcc" } } });
            AssertProgram(new TestSuite { Sources = { "(?<_a1>.)", "${_a1}$1" }, TestCases = { { "abc", "aabbcc" } } });
            AssertProgram(new TestSuite { Sources = { "(?<a>.)(?<A>.)", "${A}${a}" }, TestCases = { { "abcd", "badc" } } });
            AssertProgram(new TestSuite { Sources = { "(?<_>.)", "${_}${_}" }, TestCases = { { "abc", "aabbcc" } } });
        }

        [TestMethod]
        public void TestTrailingDollar()
        {
            AssertProgram(new TestSuite { Sources = { "(.)", "$1$" }, TestCases = { { "abc", "a$b$c$" } } });
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
        public void TestContext()
        {
            AssertProgram(new TestSuite { Sources = { ".", "$`" }, TestCases = { { "abcd", "aababc" } } });
            AssertProgram(new TestSuite { Sources = { ".", "$'" }, TestCases = { { "abcd", "bcdcdd" } } });
            AssertProgram(new TestSuite { Sources = { "", "$`$'" }, TestCases = { { "abcd", "abcdaabcdbabcdcabcddabcd" } } });
            AssertProgram(new TestSuite { Sources = { ".", "$=" }, TestCases = { { "abcd", "abcdabcdabcdabcd" } } });
            AssertProgram(new TestSuite { Sources = { "", "$=" }, TestCases = { { "abcd", "abcdaabcdbabcdcabcddabcd" } } });

            // $" is a shorthand for $'$n$`
            AssertProgram(new TestSuite { Sources = { ",", "<$\">" }, TestCases = { { "ab,cd", "ab<cd\nab>cd" } } });
        }

        [TestMethod]
        public void TestLineOnly()
        {
            AssertProgram(new TestSuite { Sources = { "a", "$%=" }, TestCases = { { "abc\nbab\naba", "abcbc\nbbabb\nabababa" } } });
            AssertProgram(new TestSuite { Sources = { "a", "$%`" }, TestCases = { { "abc\nbab\naba", "bc\nbbb\nbab" } } });
            AssertProgram(new TestSuite { Sources = { "a", "$%'" }, TestCases = { { "abc\nbab\naba", "bcbc\nbbb\nbab" } } });
            AssertProgram(new TestSuite { Sources = { ".", "$.%`" }, TestCases = { { "abc\ndefh\nhijlk", "012\n0123\n01234" } } });
            AssertProgram(new TestSuite { Sources = { ".", "$.%'" }, TestCases = { { "abc\ndefh\nhijlk", "210\n3210\n43210" } } });

            // $%" is a shorthand for $%'$n$%`
            AssertProgram(new TestSuite { Sources = { ",", "<$%\">" }, TestCases = { { "ab,cd\nef,gh", "ab<cd\nab>cd\nef<gh\nef>gh" } } });
        }

        [TestMethod]
        public void TestInvalidSyntax()
        {
            AssertProgram(new TestSuite { Sources = { "(?<a>.)(.)", "$a" }, TestCases = { { "abcd", "$a$a" } } });
        }

        [TestMethod]
        public void TestDynamicElements()
        {
            AssertProgram(new TestSuite { Sources = { "(.)(.)", "${}" }, TestCases = { { "abcd", "" } } });
            AssertProgram(new TestSuite { Sources = { ".", "${`}" }, TestCases = { { "abcd", "aababc" } } });
            AssertProgram(new TestSuite { Sources = { ".", "${=}" }, TestCases = { { "abcd", "abcdabcdabcdabcd" } } });
            AssertProgram(new TestSuite { Sources = { "(.)(.)(.)", "${$1}" }, TestCases = { { "2ab3cd", "ad" } } });
            AssertProgram(new TestSuite { Sources = { ".", "${$&}" }, TestCases = { { "'`=0", "`=0''`=00" } } });
            AssertProgram(new TestSuite { Sources = { @"(\d)(\d)(\d)", "${${$1}}" }, TestCases = { { "123,132,231,213,312,321", "1,1,1,2,1,3" } } });
        }

        [TestMethod]
        public void TestUnbalancedPatterns()
        {
            AssertProgram(new TestSuite { Sources = { "(.)(.)", "${" }, TestCases = { { "abcd", "" } } });
            AssertProgram(new TestSuite { Sources = { "(.)(.)", "${1" }, TestCases = { { "abcd", "ac" } } });
            
            AssertProgram(new TestSuite { Sources = { "(.)(.)", "}" }, TestCases = { { "abcd", "}}" } } });

            AssertProgram(new TestSuite { Sources = { "(.)(.)", "$(" }, TestCases = { { "abcd", "" } } });
            AssertProgram(new TestSuite { Sources = { "(.)(.)", "3*$(12" }, TestCases = { { "abcd", "121212121212" } } });

            AssertProgram(new TestSuite { Sources = { "(.)(.)", ")" }, TestCases = { { "abcd", "))" } } });

            AssertProgram(new TestSuite { Sources = { "(.)(.)", "${$.(x}z" }, TestCases = { { "abcd", "azcz" } } });
            AssertProgram(new TestSuite { Sources = { "(.)(.)", "2*$(x${1)z" }, TestCases = { { "abcd", "xaxazxcxcz" } } });
        }

        [TestMethod]
        public void TestEscapeSequences()
        {
            AssertProgram(new TestSuite { Sources = { "(.)", "$$1" }, TestCases = { { "abc", "$1$1$1" } } });
            AssertProgram(new TestSuite { Sources = { "(?<a>.)", "${a}$${a}" }, TestCases = { { "abc", "a${a}b${a}c${a}" } } });

            AssertProgram(new TestSuite { Sources = { "(.)(?<n>.)", "n$n${n}" }, TestCases = { { "abcd", "n\nbn\nd" } } });
            AssertProgram(new TestSuite { Sources = { "(.)(.)", "n$n${n}" }, TestCases = { { "abcd", "n\nn\n" } } });

            AssertProgram(new TestSuite { Sources = { "(.)(.)", "$}" }, TestCases = { { "abcd", "}}" } } });
            AssertProgram(new TestSuite { Sources = { "(.)(.)", "${$.($})}" }, TestCases = { { "abcd", "ac" } } });
            AssertProgram(new TestSuite { Sources = { "(.)(.)", "$)" }, TestCases = { { "abcd", "))" } } });
            AssertProgram(new TestSuite { Sources = { "(.)(.)", "3*$($))" }, TestCases = { { "abcd", "))))))" } } });

            AssertProgram(new TestSuite { Sources = { "(.)(.)", "$*" }, TestCases = { { "abcd", "**" } } });

            AssertProgram(new TestSuite { Sources = { "(.)(.)", "$\n" }, TestCases = { { "abcd", "¶¶" } } });
        }

        [TestMethod]
        public void TestCaptureCount()
        {
            AssertProgram(new TestSuite { Sources = { "(.)+", "#$#1" }, TestCases = { { "abcd\ndef", "#4\n#3" } } });
            AssertProgram(new TestSuite { Sources = { "(..)+", "#$#1" }, TestCases = { { "abcd\ndef", "#2\n#1f" } } });
            AssertProgram(new TestSuite { Sources = { "()(.)*", "$#2" }, TestCases = { { "abcd\ndef", "40\n30" } } });
            AssertProgram(new TestSuite { Sources = { "(a)+(?<1>b)+", "$#1" }, TestCases = { { "aaabbcabbb", "5c4" } } });

            // Curly braces should also work:
            AssertProgram(new TestSuite { Sources = { "(.)+", "#${#1}" }, TestCases = { { "abcd\ndef", "#4\n#3" } } });
            AssertProgram(new TestSuite { Sources = { "()(.)*", "${#2}" }, TestCases = { { "abcd\ndef", "40\n30" } } });
            AssertProgram(new TestSuite { Sources = { "(a)+(?<1>b)+", "${#1}${#01}${#001}" }, TestCases = { { "aaabbcabbb", "555c444" } } });
            AssertProgram(new TestSuite { Sources = { "(?<foo>a)+(?<bar>b)+", "${#foo}${#bar}" }, TestCases = { { "aaabbcabbb", "32c13" } } });

            // $# not followed by a valid group should be removed.
            AssertProgram(new TestSuite { Sources = { "(.)+", "$#2${#2}$#$#}" }, TestCases = { { "abcd", "$#$#}" } } });
            AssertProgram(new TestSuite { Sources = { "(?<a>.)+", "$#a${#b}$#{" }, TestCases = { { "abcd", "$#a$#{" } } });

            // $#0 returns the number of successful groups
            AssertProgram(new TestSuite { Sources = { "(a|b)+(cd)|d(e)f", "$#0${#0}${#00}" }, TestCases = { { "abcd\ndef", "222\n111" } } });
        }

        [TestMethod]
        public void TestRandomCapture()
        {
            AssertRandomProgram(new RandomTestSuite
            {
                Sources = { @"(..)+", "$?1" },
                TestCases = { { "abcdefgh", new string[] { "ab", "cd", "ef", "gh" } } }
            });

            AssertRandomProgram(new RandomTestSuite
            {
                Sources = { @"(.)+", "$?1" },
                TestCases = { { "ab\ncde", new string[] { "a\nc", "a\nd", "a\ne", "b\nc", "b\nd", "b\ne" } } }
            });
        }


        [TestMethod]
        public void TestRandomGroup()
        {
            AssertRandomProgram(new RandomTestSuite
            {
                Sources = { @"(\d)+(\D)+(\d+)", "$?&" },
                TestCases = { { "123abc456", new string[] { "3", "c", "456" } } }
            });

            AssertRandomProgram(new RandomTestSuite
            {
                Sources = { @"(\d)+(\D)+(\d+)", "$?0" },
                TestCases = { { "123abc456", new string[] { "3", "c", "456" } } }
            });
        }

        [TestMethod]
        public void TestAdjacentSeparator()
        {
            AssertProgram(new TestSuite { Sources = { @"\w+", "$<&" }, TestCases = { { "123,456;789!", ",,;;!" } } });
            AssertProgram(new TestSuite { Sources = { @"\w+", "$>&" }, TestCases = { { "123,456;789!", ",,;;!!" } } });
            AssertProgram(new TestSuite { Sources = { @"\w+", "$<'" }, TestCases = { { "123,456;789!", "123,456;789!,456;789!;789!!" } } });
        }

        [TestMethod]
        public void TestAdjacentMatch()
        {
            AssertProgram(new TestSuite { Sources = { @"\w+", "$[&" }, TestCases = { { "123,456;789!", ",123;456!" } } });
            AssertProgram(new TestSuite { Sources = { @"\w+", "$]&" }, TestCases = { { "123,456;789!", "456,789;!" } } });
            AssertProgram(new TestSuite { Sources = { @"(\w)+", "$[1" }, TestCases = { { "123,456;789!", ",3;6!" } } });
            AssertProgram(new TestSuite { Sources = { @"(\w)+", "$]1" }, TestCases = { { "123,456;789!", "6,9;!" } } });

            // The y modifer makes the matches cyclically adjacent.
            AssertProgram(new TestSuite { Sources = { @"y`\w+", "$[&" }, TestCases = { { "123,456;789!", "789,123;456!" } } });
            AssertProgram(new TestSuite { Sources = { @"y`\w+", "$]&" }, TestCases = { { "123,456;789!", "456,789;123!" } } });
            AssertProgram(new TestSuite { Sources = { @"y`(\w)+", "$[1" }, TestCases = { { "123,456;789!", "9,3;6!" } } });
            AssertProgram(new TestSuite { Sources = { @"y`(\w)+", "$]1" }, TestCases = { { "123,456;789!", "6,9;3!" } } });
        }

        [TestMethod]
        public void TestCaptureLength()
        {
            AssertProgram(new TestSuite { Sources = { ".+", ".$.0" }, TestCases = { { "abcd\ndef", ".4\n.3" } } });
            AssertProgram(new TestSuite { Sources = { "(.)+", ".$.1" }, TestCases = { { "abcd\ndef", ".1\n.1" } } });
            AssertProgram(new TestSuite { Sources = { "(.+)", ".$.1" }, TestCases = { { "abcd\ndef", ".4\n.3" } } });
            AssertProgram(new TestSuite { Sources = { "()(.*)", "$.2" }, TestCases = { { "abcd\ndef", "40\n30" } } });

            // Curly braces should also work:
            AssertProgram(new TestSuite { Sources = { ".+", ".${.0}" }, TestCases = { { "abcd\ndef", ".4\n.3" } } });
            AssertProgram(new TestSuite { Sources = { "(.+)", ".${.1}" }, TestCases = { { "abcd\ndef", ".4\n.3" } } });
            AssertProgram(new TestSuite { Sources = { "()(.*)", "${.2}" }, TestCases = { { "abcd\ndef", "40\n30" } } });
            AssertProgram(new TestSuite { Sources = { "(?<foo>a+)(?<bar>b+)", "${.foo}${.bar}" }, TestCases = { { "aaabbcabbb", "32c13" } } });

            // Make sure $`, $', $_, $& and $+ work:
            AssertProgram(new TestSuite { Sources = { "a", "$.`" }, TestCases = { { ";!~&a@#", ";!~&4@#" } } });
            AssertProgram(new TestSuite { Sources = { "a", "$.'" }, TestCases = { { ";!~&a@#", ";!~&2@#" } } });
            AssertProgram(new TestSuite { Sources = { "a", "$.=" }, TestCases = { { ";!~&a@#", ";!~&7@#" } } });
            AssertProgram(new TestSuite { Sources = { "a+", "$.&" }, TestCases = { { ";!~&aaa@#", ";!~&3@#" } } });

            // An entirely unmatched group should result in an empty string, not 0:
            AssertProgram(new TestSuite { Sources = { "a|(b)", "$.1" }, TestCases = { { "::a::b::", "::::1::" } } });
            AssertProgram(new TestSuite { Sources = { "()(.)*", "$.2" }, TestCases = { { "abcd\ndef", "1\n1" } } });

            // $. not followed by a valid group be taken literally:
            AssertProgram(new TestSuite { Sources = { "(.+)", "$.2${.2}$.$.}" }, TestCases = { { "abcd", "$.$.}" } } });
            AssertProgram(new TestSuite { Sources = { "(?<a>.+)", "$.a${.b}$.{" }, TestCases = { { "abcd", "$.a$.{" } } });
        }

        [TestMethod]
        public void TestRepetition()
        {
            AssertProgram(new TestSuite { Sources = { "(.)+", "15*_" }, TestCases = { { "abc", "_______________" } } });
            AssertProgram(new TestSuite { Sources = { "(.)+", "xyz*_" }, TestCases = { { "abc", "xy" } } });
            AssertProgram(new TestSuite { Sources = { @"\d+", "$0*1" }, TestCases = { { "3 12 4", "111 111111111111 1111" } } });
            AssertProgram(new TestSuite { Sources = { ".+", "$0*1" }, TestCases = { { "3 12 4", "111" } } });
            AssertProgram(new TestSuite { Sources = { ".+", "$0*1" }, TestCases = { { "abc10def", "1111111111" } } });
            AssertProgram(new TestSuite { Sources = { ".+", "$0*1" }, TestCases = { { "abc0def", "" } } });
            AssertProgram(new TestSuite { Sources = { "(.)+", "$#1*1" }, TestCases = { { "abcd\ndef", "1111\n111" } } });
            AssertProgram(new TestSuite { Sources = { "(..)+", "$#1*1" }, TestCases = { { "abcd\ndef", "11\n1f" } } });
            AssertProgram(new TestSuite { Sources = { "(?<a>.)+", "${#a}*1" }, TestCases = { { "abcd\ndef", "1111\n111" } } });
            AssertProgram(new TestSuite { Sources = { "$", " $`*1" }, TestCases = { { "12 123", "12 123 111111111111" } } });
            AssertProgram(new TestSuite { Sources = { "$", " $=*1" }, TestCases = { { "12 123", "12 123 111111111111" } } });
            AssertProgram(new TestSuite { Sources = { "^", "$'*1 " }, TestCases = { { "12 123", "111111111111 12 123" } } });

            AssertProgram(new TestSuite { Sources = { ".+", "$0*$" }, TestCases = { { "5", "$$$$$" } } });
            AssertProgram(new TestSuite { Sources = { ".+", @"$0*\" }, TestCases = { { "5", @"\\\\\" } } });

            // Right-hand arguments can be any other element
            AssertProgram(new TestSuite { Sources = { ".+", "5*$&" }, TestCases = { { "abc", "abcabcabcabcabc" } } });
            AssertProgram(new TestSuite { Sources = { ".+", "5*$*" }, TestCases = { { "abc", "*****" } } });
            AssertProgram(new TestSuite { Sources = { ".+", "5*12" }, TestCases = { { "abc", "1212121212" } } });

            // Only grabs the next character if it's not a special element
            AssertProgram(new TestSuite { Sources = { ".+", "*abc" }, TestCases = { { "5", "aaaaabc" } } });

            // * is right-associative:
            AssertProgram(new TestSuite { Sources = { ".+", "2*3*_" }, TestCases = { { "abc", "______" } } });
            AssertProgram(new TestSuite { Sources = { ".+", "2*_*1" }, TestCases = { { "abc", "" } } });

            // * as the first token implies $&. * as the last token implies _.
            AssertProgram(new TestSuite { Sources = { "(.).+", "*:" }, TestCases = { { "10!", "::::::::::" } } });
            AssertProgram(new TestSuite { Sources = { ".+", "5*" }, TestCases = { { "a3c", "_____" } } });
            AssertProgram(new TestSuite { Sources = { "(.).+", "*" }, TestCases = { { "10!", "__________" } } });
            AssertProgram(new TestSuite { Sources = { ".+", "**" }, TestCases = { { "3!", "_________" } } });
            AssertProgram(new TestSuite { Sources = { ".+", "5**" }, TestCases = { { "3!", "_______________" } } });
            AssertProgram(new TestSuite { Sources = { ".+", "*5*" }, TestCases = { { "3!", "_______________" } } });
            AssertProgram(new TestSuite { Sources = { ".+", "$.(**)" }, TestCases = { { "25", "625" } } });
            AssertProgram(new TestSuite { Sources = { ".+", "$.(**)" }, TestCases = { { "46330", "2146468900" } } });
            AssertProgram(new TestSuite { Sources = { ".+", "$.(**)" }, TestCases = { { "65526", "4293656676" } } });
            AssertProgram(new TestSuite { Sources = { ".+", "$.(**)" }, TestCases = { { "123123123", "15159303417273129" } } });
        }

        [TestMethod]
        public void TestReverse()
        {
            AssertProgram(new TestSuite { Sources = { ".+", "$^$&" }, TestCases = { { "abcd", "dcba" } } });
            AssertProgram(new TestSuite { Sources = { ".+", "$^$.&" }, TestCases = { { "abcdefghij", "01" } } });

            // The implicit argument is $&
            AssertProgram(new TestSuite { Sources = { ".+", "$^" }, TestCases = { { "abcd", "dcba" } } });

            // Repetition has higher precedence
            AssertProgram(new TestSuite { Sources = { ".+", "$^*$(abcd)" }, TestCases = { { "3", "dcbadcbadcba" } } });
            AssertProgram(new TestSuite { Sources = { ".+", "*$^$(abcd)" }, TestCases = { { "3", "dcbadcbadcba" } } });
        }

        [TestMethod]
        public void TestRegexEscape()
        {
            // Uses Regex.Escape but also escapes slashes
            AssertProgram(new TestSuite { Sources = { ".+", @"$\$&" }, TestCases = { { "(?<_>./)", @"\(\?<_>\.\/\)" } } });

            // The implicit argument is $&
            AssertProgram(new TestSuite { Sources = { ".+", @"$\" }, TestCases = { { "(?<_>./)", @"\(\?<_>\.\/\)" } } });
        }

        [TestMethod]
        public void TestCaseChanges()
        {
            AssertProgram(new TestSuite { Sources = { ".+", @"$L$&" }, TestCases = { { "ABCDÄ", @"abcdä" } } });
            AssertProgram(new TestSuite { Sources = { ".+", @"$l$&" }, TestCases = { { "ABCDÄ", @"aBCDÄ" } } });
            AssertProgram(new TestSuite { Sources = { ".+", @"$U$&" }, TestCases = { { "abcdä", @"ABCDÄ" } } });
            AssertProgram(new TestSuite { Sources = { ".+", @"$u$&" }, TestCases = { { "abcdä", @"Abcdä" } } });
            AssertProgram(new TestSuite { Sources = { ".+", @"$T$&" }, TestCases = { { "abc1DEF_äÖü", @"Abc1Def_Äöü" } } });

            // The implicit argument is $&
            AssertProgram(new TestSuite { Sources = { ".+", @"$L" }, TestCases = { { "ABCDÄ", @"abcdä" } } });
            AssertProgram(new TestSuite { Sources = { ".+", @"$l" }, TestCases = { { "ABCDÄ", @"aBCDÄ" } } });
            AssertProgram(new TestSuite { Sources = { ".+", @"$U" }, TestCases = { { "abcdä", @"ABCDÄ" } } });
            AssertProgram(new TestSuite { Sources = { ".+", @"$u" }, TestCases = { { "abcdä", @"Abcdä" } } });
            AssertProgram(new TestSuite { Sources = { ".+", @"$T" }, TestCases = { { "abc1DEF_äÖü", @"Abc1Def_Äöü" } } });
        }

        [TestMethod]
        public void TestConcatenation()
        {
            AssertProgram(new TestSuite { Sources = { ".+", "$(a$&b)" }, TestCases = { { "abcd", "aabcdb" } } });

            // Concatenation can be used to feed entire strings as arguments to other operators.
            AssertProgram(new TestSuite { Sources = { ".+", "*$(abc)" }, TestCases = { { "5", "abcabcabcabcabc" } } });
            AssertProgram(new TestSuite { Sources = { ".+", "$(1$&)*" }, TestCases = { { "5", "_______________" } } });

            // . can be inserted to get the length of the result. This is computed lazily when possible.
            AssertProgram(new TestSuite { Sources = { ".+", "$.($&$&)" }, TestCases = { { "abc\ndefg", "6\n8" } } });
            AssertProgram(new TestSuite { Sources = { ".+", "$.(**)" }, TestCases = { { "46340", "2147395600" } } });
            AssertProgram(new TestSuite { Sources = { ".+", "$.(**_abc)" }, TestCases = { { "46340", "2147395603" } } });
        }

        [TestMethod]
        public void TestHistory()
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

            // Ensure result log gets activated by dynamic elements:
            AssertProgram(new TestSuite { Sources = { "^", @"${1*-}" }, TestCases = { { "abc", "abcabc" } } });

            // Ensure result log gets activated by substitutions given as string options:
            AssertProgram(new TestSuite { Sources = { "\"<$.->\"+`^", @"1" }, TestCases = { { "abc", "111abc" } } });

            // Test result log limit
            AssertProgram(new TestSuite { Sources = { "!#`.+", @"$-" }, TestCases = { { "abc", "" } } });
            AssertProgram(new TestSuite {
                Sources = {
                    "!#3G`",
                    "G`",
                    "G`",
                    "G`",
                    ".+",
                    @"$-0,$-1,$-2,$-3,$-4"
                },
                TestCases = { { "abc", "abc,abc,abc,," } }
            });
        }
    }
}
