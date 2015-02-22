using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Retina.Stages
{
    class ReplaceStage : Stage
    {
        public string Replacement { get; set; }

        public ReplaceStage(Options options, Regex pattern, string replacement) : base(options, pattern)
        {
            Replacement = replacement;
        }

        public override string Execute(string input)
        {
            if (!Options.Loop)
                Console.Write(Pattern.Replace(input, Replacement));
            else
            {
                string lastInput;
                do
                {
                    lastInput = input;
                    input = Pattern.Replace(input, Replacement);
                    if (Options.Trace && lastInput != input)
                        Console.WriteLine(input);
                } while (lastInput != input);

                if (!Options.Trace)
                    Console.Write(input);
            }

            return input;
        }
    }
}
