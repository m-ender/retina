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

        public override string Execute(string input)
        {
            foreach (var part in Pattern.Split(input))
                if (!Options.Silent && !(Options.OmitEmpty && part == ""))
                    Console.WriteLine(part);

            return "";
        }
    }
}
