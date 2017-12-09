using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Retina.Stages
{
    public abstract class Stage
    {
        public Configuration Config { get; set; }

        public Stage(Configuration config)
        {
            Config = config;
        }

        abstract public string Execute(string input, TextWriter output);
    }
}
