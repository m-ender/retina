using Retina.Configuration;
using Retina.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Retina.Stages
{
    class GrepStage : AtomicStage
    {
        public GrepStage(Config config, List<string> patterns, List<string> substitutions, string separatorSubstitutionSource)
            : base(config, patterns, substitutions, separatorSubstitutionSource) { }

        protected override string Process(string input, TextWriter output)
        {
            // TODO:
            // - Random?
            // - Maybe an option to use a different line separator.

            var lines = new Regex(@"(?m:^).*").Matches(input).Cast<Match>().Select(m => new
            {
                line = m.Value,
                start = m.Index,
                end = m.Index + m.Length
            }).ToList();

            var linesToKeep = new HashSet<int>();

            foreach (var m in Matches)
            {
                int i = 0;
                while (lines[i].end < m.Match.Index)
                    ++i;
                while (i < lines.Count && lines[i].start <= m.Match.Index + m.Match.Length)
                {
                    linesToKeep.Add(i);
                    ++i;
                }
            }

            lines = linesToKeep.OrderBy(i => i).Select(i => lines[i]).ToList();
            
            lines = lines.Where((_, i) => Config.GetLimit(1).IsInRange(i, lines.Count)).ToList();

            if (Config.Reverse)
                lines.Reverse();

            return Config.FormatAsList(lines.Select(l => l.line));
        }
    }
}
