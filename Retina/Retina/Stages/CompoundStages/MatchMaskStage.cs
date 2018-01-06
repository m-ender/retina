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

        public MatchMaskStage(Config config, Stage childStage)
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
                splitRegex = new Regex("^.*$", RegexOptions.Multiline);

            List<Match> matches = splitRegex.Matches(input).Cast<Match>().ToList();
            var separators = new List<string>();

            int lastEnd = 0;
            foreach (var m in matches)
            {
                separators.Add(input.Substring(lastEnd, m.Index - lastEnd));
                lastEnd = m.Index + m.Length;
            }
            separators.Add(input.Substring(lastEnd));

            IEnumerable<string> results = matches.Select((m, i) => {
                if (Config.GetLimit(0).IsInRange(i, separators.Count))
                    return ChildStage.Execute(m.Value, output);
                else
                    return m.Value;
            });

            return separators.Riffle(results);
        }
    }
}