using Retina.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Retina.Stages
{
    class AntiGrepStage : AtomicStage
    {
        public AntiGrepStage(Config config, List<string> patterns, List<string> substitutions, string separatorSubstitutionSource) 
            : base(config, patterns, substitutions, separatorSubstitutionSource) { }

        protected override string Process(string input, TextWriter output)
        {
            // TODO:
            // - Random?
            // - Maybe limit on final lines.
            // - Maybe an option to use a different line separator.

            var lines = new Regex(@"(?m:^).*").Matches(input).Cast<Match>().Select(m => new
            {
                line = m.Value,
                start = m.Index,
                end = m.Index + m.Length
            }).ToList();
            
            foreach (var m in Matches)
            {
                int i = 0;
                while (lines[i].end < m.Match.Index)
                    ++i;
                while (i < lines.Count && lines[i].start <= m.Match.Index + m.Match.Length)
                    lines.RemoveAt(i);
            }

            {
                int i = 0;
                lines = lines.FindAll(_ => Config.GetLimit(1).IsInRange(i++, lines.Count));
            }

            if (Config.Reverse)
                lines.Reverse();

            return String.Join("\n", lines.Select(l => l.line));
        }
    }
}
