using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Retina.Stages
{
    public abstract class RegexStage : Stage
    {
        public Options Options { get; set; }
        public Regex Pattern { get; set; }

        protected RegexStage(Options options)
        {
            Options = options;
            Silent = Options.Silent;
            TrailingLinefeed = Options.TrailingLinefeed;
        }

        public RegexStage(Options options, string pattern) : this(options)
        {
            Pattern = new Regex(pattern, options.RegexOptions);
        }
    }
}
