using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Retina.Configuration;
using System.Text.RegularExpressions;

namespace Retina.Stages
{
    public class MatchMaskStage : Stage
    {
        public Stage ChildStage { get; set; }
        public Regex Pattern { get; set; }

        public MatchMaskStage(Config config, Stage childStage, Regex pattern)
            : base(config)
        {
            ChildStage = childStage;
            Pattern = pattern;
        }

        public override string Execute(string input, TextWriter output)
        {
            var builder = new StringBuilder();


            IEnumerable<Match> matches = from Match m in Pattern.Matches(input)
                                         orderby m.Index, m.Length
                                         select m;

            int i = 0;
            foreach (Match m in matches)
            {
                builder.Append(input.Substring(i, m.Index - i));

                builder.Append(ChildStage.Execute(m.Value, output));
                
                i = m.Index + m.Length;
            }

            builder.Append(input.Substring(i));

            return builder.ToString();
        }
    }
}