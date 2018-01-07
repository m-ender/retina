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
            // - Maybe an option to use a different line separator.

            var lines = new Regex(@"(?m:^).*").Matches(input).Cast<Match>().Select(m => new
            {
                line = m.Value,
                start = m.Index,
                end = m.Index + m.Length
            }).ToList();

            var toDelete = new HashSet<int>();

            foreach (var m in Matches)
            {
                int i = 0;
                while (i < lines.Count && lines[i].end < m.Match.Index)
                    ++i;
                while (i < lines.Count && lines[i].start <= m.Match.Index + m.Match.Length)
                    toDelete.Add(i++);
            }

            var sortedDeletions = toDelete.ToList();
            sortedDeletions.Sort();
            sortedDeletions = sortedDeletions.Where((_, i) => Config.GetLimit(1).IsInRange(i, sortedDeletions.Count)).ToList();
            sortedDeletions.Reverse();

            if (Config.Random && sortedDeletions.Count > 0)
            {
                var chosenLine = sortedDeletions[Random.RNG.Next(sortedDeletions.Count)];
                sortedDeletions = new List<int>();
                sortedDeletions.Add(chosenLine);
            }

            foreach (int i in sortedDeletions)
                lines.RemoveAt(i);
            
            if (Config.Reverse)
                lines.Reverse();

            return Config.FormatAsList(lines.Select(l => l.line));
        }
    }
}
