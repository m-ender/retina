using System;
using System.Collections.Generic;
using System.IO;
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

        protected RegexStage(Configuration config) : base(config) { }

        public RegexStage(Configuration config, string pattern) : this(config)
        {
            PatternString = pattern;
        }

        public override string Execute(string input, TextWriter output)
        {
            Pattern = new Regex(PatternString, Config.RegexOptions);

            return Process(input, output).ToString();
        }

        abstract protected StringBuilder Process(string input, TextWriter output);
    }
}
