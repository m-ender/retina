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

        protected RegexStage(Options options)
        {
            Options = options;
            Silent = Options.Silent;
        }

        public RegexStage(Options options, string pattern) : this(options)
        {
            Pattern = new Regex(pattern, options.RegexOptions);
        }
    }
}
