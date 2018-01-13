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
        public GrepStage(Config config, History history, List<string> patterns, List<string> substitutions, string separatorSubstitutionSource)
            : base(config, history, patterns, substitutions, separatorSubstitutionSource) { }

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

            var lineStrings = lines.Select(l => l.line).ToList();

            if (Config.Random && lineStrings.Count > 0)
            {
                var chosenLine = lineStrings[Random.RNG.Next(lineStrings.Count)];
                lineStrings = new List<string>();
                lineStrings.Add(chosenLine);
            }

            if (Config.Reverse)
                lineStrings.Reverse();

            return Config.FormatAsList(lineStrings);
        }
    }
}
