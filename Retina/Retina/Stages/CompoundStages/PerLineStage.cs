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

        public PerLineStage(Config config, Stage childStage)
            : base(config)
        {
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
            var lines = new List<string>();
            
            int lastEnd = 0;
            foreach (var m in matches)
            {
                lines.Add(input.Substring(lastEnd, m.Index - lastEnd));
                lastEnd = m.Index + m.Length;
            }
            lines.Add(input.Substring(lastEnd));
            
            IEnumerable<string> resultLines = lines.Select((line, i) => {
                if (Config.GetLimit(0).IsInRange(i, lines.Count))
                    return ChildStage.Execute(line, output);
                else
                    return line;
            });

            return resultLines.Riffle(matches.Select(m => m.Value));
        }
    }
}