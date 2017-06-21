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
        public Options Options { get; set; }

        public Stage(Options options)
        {
            Options = options;
        }

        public virtual string Execute(string input, TextWriter output)
        {
            // This whole function and the interaction between per-line mode, loops and output
            // feels massively hacky... maybe one day I'll refactor this...
            string result;
            if (Options.PerLine)
            {
                Options.PerLine = false;
                result = String.Join("\n", input.Split(new[] { '\n' }).Select(x => Execute(x, output)));
                Options.PerLine = true;
                return result;
            }

            if (Options.Loop)
            {
                result = input;
                string lastResult;
                do
                {
                    lastResult = result;
                    if (Options.IterationPerLine)
                        result = String.Join("\n", lastResult.Split(new[] { '\n' }).Select(x => Process(x, output).ToString()));
                    else
                        result = Process(lastResult, output).ToString();

                    if (!Options.IterationSilent)
                        if (Options.IterationTrailingLinefeed)
                        {
                            output.Write(result);
                            output.Write("\n");
                        }
                        else
                            output.Write(result);
                } while (lastResult != result);
            }
            else
                result = Process(input, output).ToString();

            if (!(Options.Silent ?? true))
                if (Options.TrailingLinefeed)
                {
                    output.Write(result);
                    output.Write("\n");
                }
                else
                    output.Write(result);
            
            return Options.DryRun ? input : result;
        }

        abstract protected StringBuilder Process(string input, TextWriter output);
    }
}
