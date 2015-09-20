using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Retina.Stages
{
    class ReplaceStage : RegexStage
    {
        public string Replacement { get; set; }

        public ReplaceStage(Options options, Regex pattern, string replacement) : base(options, pattern)
        {
            Replacement = replacement;
        }

        protected override StringBuilder Process(string input)
        {
            return new StringBuilder(Pattern.Replace(input, Replacement));
        }
    }
}
