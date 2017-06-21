using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Retina.Stages
{
    class SplitStage : RegexStage
    {
        public SplitStage(Options options, string pattern) : base(options, pattern) { }

        protected override StringBuilder Process(string input, TextWriter output)
        {
            int i = 0;

            IEnumerable<Match> matches = from Match m in Pattern.Matches(input)
                                         orderby m.Index, m.Length
                                         select m;

            var parts = new List<string>();

            int j = 0;
            foreach (Match m in matches)
            {
                if (Options.IsInRange(0, j++, matches.Count()))
                {
                    if (!(Options.OmitEmpty && i == m.Index))
                        parts.Add(input.Substring(i, m.Index - i));

                    if (!Options.OmitGroups)
                    {
                        var groups = Pattern.GetGroupNumbers().Skip(1);

                        foreach (var num in groups)
                            if (m.Groups[num].Success && Options.IsInRange(1, num - 1, groups.Last()))
                                parts.Add(m.Groups[num].Value);
                    }
                    i = m.Index + m.Length;
                }
            }

            if (!(Options.OmitEmpty && i == input.Length))
                parts.Add(input.Substring(i));

            return new StringBuilder(String.Join("\n", parts));
        }
    }
}
