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
        string ReplacementString { get; set; }

        public ReplaceStage(Options options, string pattern, string replacement)
            : base(options, pattern)
        {
            ReplacementString = replacement;
        }

        protected override StringBuilder Process(string input)
        {
            var replacer = new Replacer(Pattern, ReplacementString);

            var builder = new StringBuilder();

            int i = 0;

            IEnumerable<Match> matches = from Match m in Pattern.Matches(input)
                                         orderby m.Index, m.Length
                                         select m;

            int j = 0;
            foreach (Match m in matches)
            {
                builder.Append(input.Substring(i, m.Index - i));

                if (!Options.IsInRange(0, j++, matches.Count()))
                    builder.Append(m.Value);
                else
                    builder.Append(replacer.Process(input, m));
                i = m.Index + m.Length;
            }

            builder.Append(input.Substring(i));

            return builder;
        }
    }
}
