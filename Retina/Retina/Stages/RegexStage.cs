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
        public string PatternString { get; set; }
        public Regex Pattern { get; set; }

        protected RegexStage(Options options) : base(options) { }

        public RegexStage(Options options, string pattern) : this(options)
        {
            PatternString = pattern;
        }

        public override string Execute(string input)
        {
            Pattern = new Regex(PatternString, Options.RegexOptions);
            return base.Execute(input);
        }
    }
}
