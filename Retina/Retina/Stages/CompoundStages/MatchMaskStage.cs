using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Retina.Configuration;
using System.Text.RegularExpressions;
using Retina.Extensions;

namespace Retina.Stages
{
    public class MatchMaskStage : Stage
    {
        public Stage ChildStage { get; set; }

        private int HistoryIndex;

        public MatchMaskStage(Config config, Stage childStage)
            : base(config)
        {
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
                splitRegex = new Regex("^.*$", RegexOptions.Multiline);

            List<Match> matches = splitRegex.Matches(input).Cast<Match>().ToList();
            
            Limit matchLimit = Config.GetLimit(0);
            int matchCount = matches.Count;
            for (int i = matchCount - 1; i >= 0; --i)
                if (!matchLimit.IsInRange(i, matchCount))
                    matches.RemoveAt(i);

            if (Config.SingleRandomMatch && matches.Count > 0)
            {
                var chosenMatch = matches[Random.RNG.Next(matches.Count)];
                matches = new List<Match>();
                matches.Add(chosenMatch);
            }

            var separators = new List<string>();

            int lastEnd = 0;
            foreach (var m in matches)
            {
                separators.Add(input.Substring(lastEnd, m.Index - lastEnd));
                lastEnd = m.Index + m.Length;
            }
            separators.Add(input.Substring(lastEnd));

            IEnumerable<string> results = matches.Select((m, i) => ChildStage.Execute(m.Value, output));

            string result = separators.Riffle(results);
            History.RegisterResult(HistoryIndex, result);

            return result;
        }
    }
}