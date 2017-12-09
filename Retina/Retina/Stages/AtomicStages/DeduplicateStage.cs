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
    class DeduplicateStage : AtomicStage
    {
        string ReplacementString { get; set; }

        public DeduplicateStage(Configuration config, string pattern, string replacement)
            : base(config, pattern == "" ? ".+" : pattern)
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
            var matchStrings = new List<string>();
            var testStrings = new List<string>();

            int j = 0;
            foreach (Match m in matches)
            {
                if (Config.GetLimit(0).IsInRange(j++, matches.Count()))
                {
                    delimiters.Add(input.Substring(i, m.Index - i));

                    matchStrings.Add(m.Value);
                    testStrings.Add(replacer.Process(input, m));

                    i = m.Index + m.Length;
                }
            }

            delimiters.Add(input.Substring(i));

            var stringSet = new HashSet<string>();

            BitArray keep = new BitArray(testStrings.Count);

            bool rtl = Config.RegexOptions.HasFlag(RegexOptions.RightToLeft);
            int start = rtl ? testStrings.Count - 1 : 0;
            int end = rtl ? -1 : testStrings.Count;
            int step = rtl ? -1 : 1;

            for (int k = start; k != end; k += step)
            {
                keep[k] = !stringSet.Contains(testStrings[k]);
                stringSet.Add(testStrings[k]);
            }

            var builder = new StringBuilder(delimiters[0]);

            j = 0;
            foreach (string m in matchStrings)
            {
                if (keep[j])
                    builder.Append(m);
                builder.Append(delimiters[++j]);
            }

            return builder;
        }
    }
}
