using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Retina.Stages
{
    public class TransliterateStage : RegexStage
    {
        public string Replacement { get; set; }

        private string From { get; set; }
        private string To { get; set; }

        public TransliterateStage(Options options, string pattern) : base(options)
        {

            var setBuilder = new StringBuilder();

            string remainder = ParseCharacterSet(setBuilder, pattern);
            From = setBuilder.ToString();
            setBuilder.Clear();
            remainder = ParseCharacterSet(setBuilder, remainder);
            To = setBuilder.ToString();

            Pattern = new Regex(remainder.Length == 0 ? @"[\s\S]+" : remainder, options.RegexOptions);
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

                // Any other character is treated as a literal.
                default: return token[1];
                }
        }

        private string ParseCharacterSet(StringBuilder setBuilder, string source)
        {
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
                    (?<class>[dHhLlwp])  #     A built-in character class.
                  )                      #   Priority is given to custom ranges, such that the built-in classes can 
                                         #   appear as the first character in a range without needing escaping.
                )
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
                        case 'H': range.Append("0123456789ABCDEF"); break;
                        case 'h': range.Append("0123456789abcdef"); break;
                        case 'L': range.Append("ABCDEFGHIJKLMNOPQRSTUVWXYZ"); break;
                        case 'l': range.Append("abcdefghijklmnopqrstuvwxyz"); break;
                        case 'w': range.Append("_0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz"); break;
                        // Printable ASCII
                        case 'p': range.Append(@" !""#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\]^_`abcdefghijklmnopqrstuvwxyz{|}~"); break;
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
                    setBuilder.Append(rangeStr);
                }
                else if (t.Groups["char"].Success)
                    setBuilder.Append(ParseCharacterToken(t.Groups["char"].Value));
                else
                    throw new Exception("This shouldn't happen...");
            }

            return remainder;
        }

        protected override StringBuilder Process(string input)
        {
            var builder = new StringBuilder();

            int n = To.Length;
            int i = 0;

            var matches = Pattern.Matches(input).Cast<Match>();
            if (Pattern.Options.HasFlag(RegexOptions.RightToLeft))
                matches = matches.Reverse();

            foreach (Match m in matches)
            {
                builder.Append(input.Substring(i, m.Index-i));
                foreach (char c in m.Value)
                {
                    int j = From.IndexOf(c);
                    if (j < 0)
                        builder.Append(c);
                    else if (n > 0)
                        builder.Append(To[Math.Min(n - 1, j)]);
                }
                i = m.Index + m.Length;
            }

            builder.Append(input.Substring(i));

            return builder;
        }
    }
}
