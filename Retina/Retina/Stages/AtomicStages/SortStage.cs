using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Retina.Configuration;
using Retina.Extensions;
using System.Text.RegularExpressions;

namespace Retina.Stages
{
    public class SortStage : AtomicStage
    {
        public SortStage(Config config, List<string> patterns, List<string> substitutions, string separatorSubstitutionSource)
            : base(config, patterns, substitutions, separatorSubstitutionSource) { }

        protected override string Process(string input, TextWriter output)
        {
            IEnumerable<string> sortedMatches;

            if (Config.Random)
            {
                var matchStrings = Matches.Select(m => m.Match.Value).ToList();
                matchStrings.Shuffle();
                sortedMatches = matchStrings;
            }
            else if (Config.SortNumerically)
                sortedMatches = from MatchContext m in Matches
                                let numberMatch = new Regex(@"-?\d+").Match(m.Replacement)
                                orderby numberMatch.Success ? int.Parse(numberMatch.Value) : 0
                                select m.Match.Value;
            else
                sortedMatches = Matches.OrderBy(m => m.Replacement, StringComparer.Ordinal).Select(m => m.Match.Value);

            var separators = Separators.Select(s => s.Match.Value);

            if (Config.Reverse)
                sortedMatches = sortedMatches.Reverse();
            
            return separators.Riffle(sortedMatches);
        }
    }
}
