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
        public AntiGrepStage(Config config, History history, bool registerByDefault, List<string> patterns, List<string> substitutions, string separatorSubstitutionSource) 
            : base(config, history, registerByDefault, patterns, substitutions, separatorSubstitutionSource) { }

        protected override string Process(string input, TextWriter output)
        {
            var lines = new List<int>().Select(t => new { line = "", start = 0, end = 0 }).ToList();

            {
                int start = 0;
                int end;
                string line;

                Regex regex;
                if (Config.RegexParam != null)
                    regex = Config.RegexParam;
                else if (Config.StringParam != null)
                    regex = new Regex(Regex.Escape(Config.StringParam));
                else
                    regex = new Regex(@"\n");

                foreach (var m in regex.Matches(input).Cast<Match>())
                {
                    end = m.Index;
                    line = input.Substring(start, end - start);
                    lines.Add(new { line, start, end });
                    start = m.Index + m.Length;
                }
                end = input.Length;
                line = input.Substring(start, end - start);
                lines.Add(new { line, start, end });
            }

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
