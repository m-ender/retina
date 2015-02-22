using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Retina.Stages
{
    class GrepStage : Stage
    {
        public GrepStage(Options options, Regex pattern) : base(options, pattern) { }

        public override string Execute(string input)
        {
            string line;
            var stringReader = new StringReader(input);

            while ((line = stringReader.ReadLine()) != null)
            {
                if (Pattern.IsMatch(line) ^ (Options.Mode == Modes.AntiGrep))
                    Console.WriteLine(line);
            }

            return "";
        }
    }
}
