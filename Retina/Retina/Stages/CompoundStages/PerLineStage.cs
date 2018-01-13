using Retina.Configuration;
using Retina.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Retina.Stages
{
    public class PerLineStage : Stage
    {
        public Stage ChildStage { get; set; }
        private History History;
        private int HistoryIndex;

        public PerLineStage(Config config, History history, Stage childStage)
            : base(config)
        {
            History = history;
            HistoryIndex = History.RegisterStage();
            ChildStage = childStage;
        }

        public override string Execute(string input, TextWriter output)
        {
            Regex splitRegex;

            if (Config.RegexParam != null)
                splitRegex = Config.RegexParam;
            else if (Config.StringParam != null)
                splitRegex = new Regex(Regex.Escape(Config.StringParam));
            else
                splitRegex = new Regex("\n");

            List<Match> matches = splitRegex.Matches(input).Cast<Match>().ToList();

            if (Config.SingleRandomMatch && matches.Count > 0)
            {
                var chosenMatch = matches[Random.RNG.Next(matches.Count)];
                matches = new List<Match>();
                matches.Add(chosenMatch);
            }

            var lines = new List<string>();
            int lastEnd = 0;
            foreach (var m in matches)
            {
                lines.Add(input.Substring(lastEnd, m.Index - lastEnd));
                lastEnd = m.Index + m.Length;
            }
            lines.Add(input.Substring(lastEnd));

            List<int> linesToProcess = lines.Select((_, i) => i).Where(i => Config.GetLimit(0).IsInRange(i, lines.Count)).ToList();
            
            if (Config.Random && linesToProcess.Count > 0)
            {
                var chosenLine = linesToProcess[Random.RNG.Next(linesToProcess.Count)];
                linesToProcess = new List<int>();
                linesToProcess.Add(chosenLine);
            }

            if (Config.Reverse)
                linesToProcess.Reverse();

            foreach (int i in linesToProcess)
                lines[i] = ChildStage.Execute(lines[i], output);
            
            string result = lines.Riffle(matches.Select(m => m.Value));
            History.RegisterResult(HistoryIndex, result);

            return result;
        }
    }
}