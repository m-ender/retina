using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Retina.Stages
{
    class SplitStage : RegexStage
    {
        public SplitStage(Options options, Regex pattern) : base(options, pattern) { }

        protected override StringBuilder Process(string input)
        {
            var builder = new StringBuilder();

            bool first = true;

            foreach (var part in Pattern.Split(input))
            {
                if (!(Options.OmitEmpty && part == ""))
                {
                    if (!first) builder.Append("\n");
                    first = false;
                    builder.Append(part);
                }
            }

            return builder;
        }
    }
}
