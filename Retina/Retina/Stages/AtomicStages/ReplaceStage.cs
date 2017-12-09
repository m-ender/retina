using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Retina.Replace;
using System.IO;

namespace Retina.Stages
{
    public class ReplaceStage : AtomicStage
    {
        string ReplacementString { get; set; }

        public ReplaceStage(Configuration config, string pattern, string replacement)
            : base(config, pattern)
        {
            ReplacementString = replacement;
        }

        protected override StringBuilder Process(string input, TextWriter output)
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

                if (!Config.GetLimit(0).IsInRange(j++, matches.Count()))
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
