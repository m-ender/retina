using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Retina.Stages
{
    class MatchStage : Stage
    {
        public MatchStage(Options options, Regex pattern) : base(options, pattern) { }

        public override string Execute(string input)
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

            if (!Options.Silent)
            {
                if (Options.PrintMatches)
                {
                    foreach (Match match in matches)
                        Console.WriteLine(match.Value);
                }
                else
                {
                    Console.WriteLine(matches.Count);
                }
            }

            return "";
        }
    }
}
