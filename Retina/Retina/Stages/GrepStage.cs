using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Retina.Stages
{
    class GrepStage : RegexStage
    {
        public GrepStage(Options options, string pattern) : base(options, pattern) { }

        protected override StringBuilder Process(string input, TextWriter output)
        {
            var stringReader = new StringReader(input);

            var builder = new StringBuilder();

            var lines = input.Split(new[] { '\n' });
            var matches = lines.Select(line => Pattern.IsMatch(line)).ToList();

            var matchCount = matches.Count(isMatch => isMatch);

            bool first = true;
            int i = 0;
            int j = 0;
            foreach (var line in lines)
            {
                if ((matches[i] && Options.IsInRange(0, j++, matchCount)) ^ (Options.Mode == Modes.AntiGrep))
                {
                    if (!first) builder.Append("\n");
                    first = false;
                    builder.Append(line);
                }
                ++i;
            }

            return builder;
        }
    }
}
