using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Retina.Stages
{
    abstract class RegexStage : Stage
    {
        public Options Options { get; set; }
        public Regex Pattern { get; set; }

        public RegexStage(Options options, Regex pattern)
        {
            Options = options;
            Pattern = pattern;

            Silent = Options.Silent;
        }
    }
}
