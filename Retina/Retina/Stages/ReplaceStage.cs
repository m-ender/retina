using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Retina.Replace;

namespace Retina.Stages
{
    public class ReplaceStage : RegexStage
    {
        public List<Token> Tokens { get; set; }

        public ReplaceStage(Options options, string pattern, string replacement)
            : base(options, pattern)
        {
            ParseReplacement(replacement);
        }

        private void ParseReplacement(string replacement)
        {
            Tokens = new List<Token>();

            var tokenizer = new Regex(@"\G(?: # Use \G to ensure that the tokens cover the entire string.
              (?<literal>\d+)          # Numbers are literals but are treated as single tokens for the purpose of other
                                       # like $*.
            |
              (?<literal>[^$])         # All special substitution elements start with $, so everything else is a literal.
            |
              (?<input>[$]_)           # $_ includes the entire input string.
            |
              (?<prefix>[$]`)          # $` includes everything before the match.
            |
              (?<suffix>[$]')          # $' includes everything after the match.
            |
              (?<match>[$]&)           # $& includes the entire match and is synonymous with $0.
            |
              (?<last>[$][+])          # $+ includes the capture group with the largest number.
            |
              (?<group>[$]             # Match a group reference.
              (?<count>[#])?           # Custom addition: by inserting a #, we get the capture count instead of the result.
              (?:
                (?<number>\d+)         # Either an integer.
              |
                {(?:                   # Or it's wrapped in braces, where it's either...
                  (?<number>\d+)       #   still an integer.
                |                      # or
                  (?<name>[^\W\d]\w*)  #   a valid group name.
                )}
              ))
            |
              [$](?<literal>[$])       # $$ is an escape sequence for a single $.
            | # Apart from the last option, all remaining tokens are custom additions to what .NET would provide as well.
              (?<linefeed>[$]n)        # $n is an escape sequence for a linefeed character.
            |
              (?!^)                    # Cannot be the first token.
              [$][*](?<repeat>.)       # Repeats the matched character by the first decimal number found in the result of
                                       # of the preceding token.
            |
              (?<literal>[$])          # If none of the above special elements matched, we treat the $ as a literal, too.
            )", RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline);

            MatchCollection tokens = tokenizer.Matches(replacement);

            foreach (Match t in tokens)
            {
                if (t.Groups["literal"].Success)
                    Tokens.Add(new Literal(t.Groups["literal"].Value));
                else if (t.Groups["input"].Success)
                    Tokens.Add(new EntireInput());
                else if (t.Groups["prefix"].Success)
                    Tokens.Add(new Prefix());
                else if (t.Groups["suffix"].Success)
                    Tokens.Add(new Suffix());
                else if (t.Groups["match"].Success)
                    Tokens.Add(new EntireMatch());
                else if (t.Groups["last"].Success)
                    Tokens.Add(new LastGroup());
                else if (t.Groups["group"].Success)
                {
                    bool getCount = t.Groups["count"].Success;
                    string raw = t.Groups["group"].Value;
                    if (t.Groups["number"].Success)
                    {
                        int number = int.Parse(t.Groups["number"].Value);
                        if (Pattern.GetGroupNumbers().Contains(number))
                            Tokens.Add(new NumberedGroup(number, getCount));
                        else
                            Tokens.Add(new Literal(raw));
                    }
                    else if (t.Groups["name"].Success)
                    {
                        string name = t.Groups["name"].Value;
                        if (Pattern.GetGroupNames().Contains(name))
                            Tokens.Add(new NamedGroup(name, getCount));
                        else
                            Tokens.Add(new Literal(raw));
                    }
                }
                else if (t.Groups["linefeed"].Success)
                    Tokens.Add(new Literal("\n"));
                else if (t.Groups["repeat"].Success)
                {
                    Token lastToken = Tokens.Last();
                    Tokens.RemoveAt(Tokens.Count - 1);
                    Tokens.Add(new Repetition(t.Groups["repeat"].Value[0], lastToken));
                }
                else
                    throw new Exception("This shouldn't happen...");
            }
        }

        protected override StringBuilder Process(string input)
        {
            var builder = new StringBuilder();

            int i = 0;

            IEnumerable<Match> matches = from Match m in Pattern.Matches(input)
                                         orderby m.Index, m.Length
                                         select m;

            foreach (Match m in matches)
            {
                builder.Append(input.Substring(i, m.Index - i));
                foreach (Token t in Tokens)
                    builder.Append(t.Process(input, m));
                i = m.Index + m.Length;
            }

            builder.Append(input.Substring(i));

            return builder;
        }
    }
}
