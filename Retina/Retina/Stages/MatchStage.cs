﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Retina.Stages
{
    class MatchStage : RegexStage
    {
        public MatchStage(Options options, string pattern) : base(options, pattern) { }

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

            if (Options.Limits.Count > 0 && Options.Limits[0] > 0)
                matches = new List<Match>(matches.Take(Options.Limits[0]));

            var builder = new StringBuilder();

            if (Options.PrintMatches)
            {
                bool first = true;
                foreach (Match match in matches)
                {
                    if (!first) builder.Append("\n");
                    first = false;
                    if (Options.Limits.Count > 1 && Options.Limits[1] > 0)
                        builder.Append(match.Value.Substring(0,Math.Min(match.Length, Options.Limits[1])));
                    else
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
