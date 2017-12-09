using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Retina.Stages
{
    class MatchStage : AtomicStage
    {
        public MatchStage(Configuration config, string pattern) : base(config, pattern) { }

        protected override StringBuilder Process(string input, TextWriter output)
        {
            IList<Match> matches;

            if (!Config.Overlapping)
            {
                matches = Pattern.Matches(input).Cast<Match>().ToList();
            }
            else
            {
                matches = new List<Match>();

                if (!Pattern.Options.HasFlag(RegexOptions.RightToLeft))
                {
                    int start = 0;

                    while (start <= input.Length)
                    {
                        Match match = Pattern.Match(input, start);
                        if (!match.Success) break;
                        matches.Add(match);
                        start = match.Index + 1;
                    }
                }
                else
                {
                    int start = input.Length;

                    while (start >= 0)
                    {
                        Match match = Pattern.Match(input, start);
                        if (!match.Success) break;
                        matches.Add(match);
                        start = match.Index + match.Length - 1;
                    }
                }
            }

            var matchedStrings = new List<string>();

            int i = 0;
            foreach (Match match in matches)
            {
                if (Config.GetLimit(0).IsInRange(i, matches.Count))
                {
                    var matchBuilder = new StringBuilder();

                    int j = 0;
                    foreach (char c in match.Value)
                    {
                        if (Config.GetLimit(1).IsInRange(j, match.Length))
                            matchBuilder.Append(c);
                        ++j;
                    }
                    matchedStrings.Add(matchBuilder.ToString());
                }
                ++i;
            }

            if (Config.Unique)
                matchedStrings = matchedStrings.Distinct().ToList();

            var builder = new StringBuilder();

            if (Config.PrintMatches)
                builder.Append(String.Join("\n", matchedStrings));
            else
                builder.Append(matchedStrings.Count);

            return builder;
        }
    }
}
