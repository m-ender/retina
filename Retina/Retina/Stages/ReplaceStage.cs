using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Retina.Stages
{
    class ReplaceStage : RegexStage
    {
        public string Replacement { get; set; }

        public ReplaceStage(Options options, Regex pattern, string replacement) : base(options, pattern)
        {
            Replacement = replacement;
        }

        public override string Execute(string input)
        {
            string result = input;
            if (!Options.Loop)
            {
                result = Pattern.Replace(input, Replacement);
                if (!Options.Silent)
                    Console.WriteLine(result);
            }
            else
            {
                string lastResult;
                do
                {
                    lastResult = result;
                    result = Pattern.Replace(result, Replacement);
                    if (!Options.Silent && Options.Trace && lastResult != result)
                        Console.WriteLine(result);
                } while (lastResult != result);

                if (!Options.Silent && !Options.Trace)
                    Console.WriteLine(result);
            }

            return result;
        }
    }
}
