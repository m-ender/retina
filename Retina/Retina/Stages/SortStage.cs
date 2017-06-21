using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Retina.Replace;
using System.IO;

namespace Retina.Stages
{
    public class SortStage : RegexStage
    {
        string ReplacementString { get; set; }

        public SortStage(Options options, string pattern, string replacement)
            : base(options, pattern == "" ? ".+" : pattern)
        {
            ReplacementString = replacement;
        }

        protected override StringBuilder Process(string input, TextWriter output)
        {
            var replacer = new Replacer(Pattern, ReplacementString);

            int i = 0;

            IEnumerable<Match> matches = from Match m in Pattern.Matches(input)
                                         orderby m.Index, m.Length
                                         select m;

            var delimiters = new List<string>();
            var sortableMatches = new List<Match>();

            int j = 0;
            foreach (Match m in matches)
            {
                if (Options.IsInRange(0, j++, matches.Count()))
                {
                    delimiters.Add(input.Substring(i, m.Index - i));

                    sortableMatches.Add(m);

                    i = m.Index + m.Length;
                }
            }

            delimiters.Add(input.Substring(i));

            IEnumerable<Match> sortedMatches;

            if (Options.SortNumerically)
                sortedMatches = from Match m in sortableMatches
                                let numberMatch = new Regex(@"-?\d+").Match(replacer.Process(input, m))
                                orderby numberMatch.Success ? int.Parse(numberMatch.Value) : 0
                                select m;
            else
                sortedMatches = sortableMatches.OrderBy(m => replacer.Process(input, m), StringComparer.Ordinal);

            if (Options.SortReverse)
                sortedMatches = sortedMatches.Reverse();

            var builder = new StringBuilder(delimiters[0]);

            j = 0;
            foreach (Match m in sortedMatches)
            {
                builder.Append(m.Value);
                builder.Append(delimiters[++j]);
            }

            return builder;
        }
    }
}
