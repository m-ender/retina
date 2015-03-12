using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Retina.Stages
{
    abstract class Stage
    {
        public Options Options { get; set; }
        public Regex Pattern { get; set; }

        public Stage(Options options, Regex pattern)
        {
            Options = options;
            Pattern = pattern;
        }

        abstract public string Execute(string input);
    }
}
