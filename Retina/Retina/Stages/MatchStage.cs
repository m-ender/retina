using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Retina.Stages
{
    class MatchStage : RegexStage
    {
        public MatchStage(Options options, Regex pattern) : base(options, pattern) { }

        protected override StringBuilder Process(string input)
        {
            IList<Match> matches;

            if (!Options.Overlapping)
            {
                matches = Pattern.Matches(input).Cast<Match>().ToList();
            }
            else
            {
                matches = new List<Match>();
                int start = 0;

                while (start < input.Length)
                {
                    Match match = Pattern.Match(input, start);
                    if (!match.Success) break;
                    matches.Add(match);
                    start = match.Index + 1;
                }
            }

            var builder = new StringBuilder();

            if (Options.PrintMatches)
            {
                bool first = true;
                foreach (Match match in matches)
                {
                    if (!first) builder.Append("\n");
                    first = false;
                    builder.Append(match.Value);
                }
            }
            else
            {
                builder.Append(matches.Count);
            }

            return builder;
        }
    }
}
