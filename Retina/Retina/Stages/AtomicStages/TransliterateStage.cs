using Retina.Configuration;
using Retina.Replace;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Retina.Stages
{
    public class TransliterateStage : AtomicStage
    {
        public string Replacement { get; set; }

        private List<char?> From { get; set; }
        private List<char?> To { get; set; }

        public TransliterateStage(Config config, History history, List<string> patterns, List<string> substitutionSources, string separatorSubstitutionSource)
            : base(config, history)
        {
            var fromBuilder = new List<char?>();
            var toBuilder = new List<char?>();

            int otherIndexInFrom = -1;
            bool otherReversedInFrom = false;
            string remainder = ParseCharacterSet(fromBuilder, patterns[0], out otherIndexInFrom, out otherReversedInFrom);
            
            int otherIndexInTo = -1;
            bool otherReversedInTo = false;
            remainder = ParseCharacterSet(toBuilder, remainder, out otherIndexInTo, out otherReversedInTo);

            if (otherIndexInFrom > -1 && otherIndexInTo > -1)
            {
                fromBuilder.Insert(otherIndexInFrom, 'o');
                toBuilder.Insert(otherIndexInTo, 'o');
            }
            else if (otherIndexInFrom > -1)
            {
                var otherSet = toBuilder.ToList();
                if (otherReversedInFrom) otherSet.Reverse();
                fromBuilder.InsertRange(otherIndexInFrom, otherSet);
            }
            else if (otherIndexInTo > -1)
            {
                var otherSet = fromBuilder.ToList();
                if (otherReversedInTo) otherSet.Reverse();
                toBuilder.InsertRange(otherIndexInTo, otherSet);
            }

            From = fromBuilder;
            To = toBuilder;
            if (To.Count == 0)
            {
                To = new List<char?>();
                To.Add(null);
            }

            patterns[0] = remainder.Length == 0 ? @"\A(?s:.*)\z" : remainder;

            RegexSources = patterns;
            Replacers = substitutionSources.Select(s => new Replacer(s, History, Config.CyclicMatches)).ToList();
            SeparatorReplacer = new Replacer(separatorSubstitutionSource, History);
        }

        private char ParseCharacterToken(string token)
        {
            if (token.Length == 1 || token[0] != '\\')
                return token[0];
            else
                switch (token[1])
                {
                // Character escapes
                case 'a': return '\a';
                case 'b': return '\b';
                case 'f': return '\f';
                case 'n': return '\n';
                case 'r': return '\r';
                case 't': return '\t';
                case 'v': return '\v';
                case '\n': return '¶';

                // Any other character is treated as a literal.
                default: return token[1];
                }
        }

        private string ParseCharacterSet(List<char?> setBuilder, string source, out int otherIndex, out bool otherReversed)
        {
            otherIndex = -1;
            otherReversed = false;

            var tokenizer = new Regex(@"\G(?: # Use \G to ensure that the tokens cover the entire string.
                `(?<remainder>.*)        # ` Terminates the current part of the pattern and moves to the next one.
            |
                (?<range>                # Match a range:
                  (?<reverse>R*)         #   Each leading 'R' reverses the range.
                  (?:                    #   A range could be either:
                    (?<start>[^\\`]|\\.) #     A non-backslash or an escaped sequence.
                    -                    #     A hyphen to denote a custom range.
                    (?<end>[^\\`]|\\.)   #     A non-backslash or an escaped sequence.
                  |                      #   or:
                    (?<class>[dEOHhLlVvwpo]) #     A built-in character class.
                  )                      #   Priority is given to custom ranges, such that the built-in classes can 
                                         #   appear as the first character in a range without needing escaping.
                )
            |   
                (?<blank>_)              # These are don't affect any character if they appear in the TO set and
                                         # remove the corresponding character if they appear in the FROM set.
            |
                (?<char>\\.|.)           # Backslashes indicate escape sequences similar to normal regex.
                                         # Anything else is just a literal character.
            )", RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline);

            MatchCollection tokens = tokenizer.Matches(source);

            string remainder = "";

            foreach (Match t in tokens)
            {
                if (t.Groups["remainder"].Success)
                {
                    remainder = t.Groups["remainder"].Value;
                    break; // Technically not necessary, but you never know.
                }
                else if (t.Groups["range"].Success)
                {
                    var range = new StringBuilder();
                    if (t.Groups["class"].Success)
                    {
                        switch (t.Groups["class"].Value[0])
                        {
                        case 'd': range.Append("0123456789"); break;
                        case 'E': range.Append("02468"); break;
                        case 'O': range.Append("13579"); break;
                        case 'H': range.Append("0123456789ABCDEF"); break;
                        case 'h': range.Append("0123456789abcdef"); break;
                        case 'L': range.Append("ABCDEFGHIJKLMNOPQRSTUVWXYZ"); break;
                        case 'l': range.Append("abcdefghijklmnopqrstuvwxyz"); break;
                        case 'V': range.Append("AEIOU"); break;
                        case 'v': range.Append("aeiou"); break;
                        case 'w': range.Append("_0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz"); break;
                        // Printable ASCII
                        case 'p': range.Append(@" !""#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\]^_`abcdefghijklmnopqrstuvwxyz{|}~"); break;
                        // Set a marker to insert the other set
                        case 'o':
                            if (otherIndex < 0)
                            {
                                otherIndex = setBuilder.Count;
                                otherReversed = t.Groups["reverse"].Length % 2 == 1;
                            }
                            else
                                setBuilder.Add('o');
                            continue;
                        }
                    }
                    else
                    {
                        char start = ParseCharacterToken(t.Groups["start"].Value);
                        char end = ParseCharacterToken(t.Groups["end"].Value);
                        int step = end > start ? 1 : -1;
                        char c;
                        for (c = start; c != end; c = (char)((int)c + step))
                            range.Append(c);
                        range.Append(c);
                    }
                    string rangeStr = range.ToString();
                    if (t.Groups["reverse"].Length % 2 == 1)
                    {
                        var characters = rangeStr.ToCharArray();
                        Array.Reverse(characters);
                        rangeStr = new string(characters);
                    }
                    foreach (char c in rangeStr)
                        setBuilder.Add(c);
                }
                else if (t.Groups["blank"].Success)
                    setBuilder.Add(null);
                else if (t.Groups["char"].Success)
                    setBuilder.Add(ParseCharacterToken(t.Groups["char"].Value));
                else
                    throw new Exception("This shouldn't happen...");
            }

            return remainder;
        }

        protected override string Process(string input, TextWriter output)
        {
            // TODO:
            // - figure out additional limits

            var mutableInput = new StringBuilder(input);
            var toDelete = new HashSet<int>();

            int[] transliterationCount = new int[input.Length];

            Matches.ForEach(m =>
            {
                for (int i = 0; i < m.Match.Length; ++i)
                    if (Config.GetLimit(1).IsInRange(i, m.Match.Length))
                        ++transliterationCount[i + m.Match.Index];
            });

            if (Config.TransliterateOnce)
            {
                for (int i = 0; i < transliterationCount.Length; ++i)
                    if (transliterationCount[i] > 1)
                        transliterationCount[i] = 1;
            }
            
            List<char?> from;
            List<char?> to;

            if (!Config.CyclicTransliteration)
            {
                from = From;
                to = To;
            }
            else
            {
                int maxCharCount = input.GroupBy(c => c).Select(g => g.Count()).Aggregate(Math.Max);
                from = Enumerable.Range(0, maxCharCount).SelectMany(_ => From).ToList();
                to = Enumerable.Range(0, from.Count / To.Count + 1).SelectMany(_ => To).ToList();
            }


            for (int _i = 0; _i < mutableInput.Length; ++_i)
            {
                // Iterate backwards with reverse option.
                int i = Config.Reverse ?  mutableInput.Length - _i - 1 : _i;

                for (int j = 0; j < transliterationCount[i]; ++j)
                {
                    int k;
                    k = from.IndexOf(mutableInput[i]);
                    
                    if (k < 0)
                        break;

                    char? target;
                    if (!Config.CyclicTransliteration)
                        target = To[Math.Min(To.Count - 1, k)];
                    else
                    {
                        target = to[k];
                        // Remove this occurrence from the source list so that the next occurrence of the
                        // character uses the next mapping.
                        from[k] = null;
                    }

                    if (target == null)
                    {
                        toDelete.Add(i);
                        break;
                    }
                    else
                        mutableInput[i] = (char)target;
                }
            }

            var sortedDeletions = toDelete.ToList();
            sortedDeletions.Sort();
            sortedDeletions.Reverse();

            foreach (int i in sortedDeletions)
                mutableInput.Remove(i, 1);

            return mutableInput.ToString();
        }
    }
}
