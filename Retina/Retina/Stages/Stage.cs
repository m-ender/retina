using Retina.Configuration;
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
        public Config Config { get; set; }

        public Stage(Config config)
        {
            Config = config;
        }

        abstract public string Execute(string input, TextWriter output);
    }
}
