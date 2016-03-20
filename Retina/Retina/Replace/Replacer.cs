using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Retina.Replace
{
    class Replacer
    {
        public Regex Pattern { get; set; }
        public List<Token> Tokens { get; set; }

        public Replacer(Regex pattern, string replacement)
        {
            Pattern = pattern;
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
              (?<raw>                  # We use this <raw> group to capture the exact prefix that introduced the
                                       # substitution element, in case it needs to be printed literally.
                [$]
                (?:                    # There are certain modifiers that can come after the $ which change which property
                                       # of the chosen substitution element is going to be inserted.
                  (?<count>            # Custom addition: by inserting a #, we get the capture count instead of the result.
                    [#]
                    (?![$n_`'&])       # This shouldn't work with all substitution elements.
                  )
                |
                  (?<length>
                    [.]
                    (?![$n])           # This shouldn't work with all substitution elements.
                  )
                )?
              )                       
              (?:
                (?<input>_)            # $_ includes the entire input string.
              |
                (?<prefix>`)           # $` includes everything before the match.
              |
                (?<suffix>')           # $' includes everything after the match.
              |
                (?<match>&)            # $& includes the entire match and is synonymous with $0.
              |
                (?<last>               # $+ includes the capture group with the largest number.
                  [+]
                )
              |
                (?<group>              # Match a group reference.
                (?:
                  (?<number>\d+)       # Either an integer.
                |
                  {(?:                   # Or it's wrapped in braces, where it's either...
                    (?<number>\d+)       #   still an integer.
                  |                      # or
                    (?<name>[^\W\d]\w*)  #   a valid group name.
                  )}
                ))
              |
                (?<literal>[$])        # $$ is an escape sequence for a single $.
              | # Apart from the last option, all remaining tokens are custom additions to what .NET would provide as well.
                (?<linefeed>n)         # $n is an escape sequence for a linefeed character.
              |
                [*](?<repeat>.|$)      # Repeats the matched character by the first decimal number found in the result of
                                       # of the preceding token. If there is no preceding token, assume $&. If there is no
                                       # character following $*, assume '1'.
              )
            |
              (?<literal>[$])          # If none of the above special elements matched, we treat the $ as a literal, too.
            )", RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline);

            MatchCollection tokens = tokenizer.Matches(replacement);

            foreach (Match t in tokens)
            {
                bool getLength = t.Groups["length"].Success;

                if (t.Groups["literal"].Success)
                    Tokens.Add(new Literal(t.Groups["literal"].Value));
                else if (t.Groups["input"].Success)
                    Tokens.Add(new EntireInput(getLength));
                else if (t.Groups["prefix"].Success)
                    Tokens.Add(new Prefix(getLength));
                else if (t.Groups["suffix"].Success)
                    Tokens.Add(new Suffix(getLength));
                else if (t.Groups["match"].Success)
                    Tokens.Add(new EntireMatch(getLength));
                else if (t.Groups["last"].Success)
                    Tokens.Add(new LastGroup(t.Groups["count"].Success, getLength));
                else if (t.Groups["group"].Success)
                {
                    bool getCount = t.Groups["count"].Success;
                    string raw = t.Groups["raw"].Value + t.Groups["group"].Value;
                    if (t.Groups["number"].Success)
                    {
                        int number = int.Parse(t.Groups["number"].Value);
                        if (Pattern.GetGroupNumbers().Contains(number))
                            Tokens.Add(new NumberedGroup(number, getCount, getLength));
                        else
                            Tokens.Add(new Literal(raw));
                    }
                    else if (t.Groups["name"].Success)
                    {
                        string name = t.Groups["name"].Value;
                        if (Pattern.GetGroupNames().Contains(name))
                            Tokens.Add(new NamedGroup(name, getCount, getLength));
                        else
                            Tokens.Add(new Literal(raw));
                    }
                }
                else if (t.Groups["linefeed"].Success)
                    Tokens.Add(new Literal("\n"));
                else if (t.Groups["repeat"].Success)
                {
                    Token lastToken;
                    if (Tokens.Count > 0)
                    {
                        lastToken = Tokens.Last();
                        Tokens.RemoveAt(Tokens.Count - 1);
                    }
                    else
                        lastToken = new EntireMatch();

                    char character = t.Groups["repeat"].Length > 0 ? t.Groups["repeat"].Value[0] : '1';
                    Tokens.Add(new Repetition(character, lastToken));
                }
                else
                    throw new Exception("This shouldn't happen...");
            }
        }

        public string Process(string input, Match match)
        {
            var builder = new StringBuilder();

            foreach (Token t in Tokens)
                builder.Append(t.Process(input, match));

            return builder.ToString();
        }
    }
}
