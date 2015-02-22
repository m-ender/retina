using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Retina.Stages;

namespace Retina
{
    class Plan
    {
        private IList<Stage> Stages;

        public Plan (string[] args)
	    {
            Stages = new List<Stage>();

            if (args.Count() < 1)
                Console.WriteLine("Usage: Retina.exe pattern.rgx [replacement.rpl]\n" +
                                  "Instead of a file names you can also use '-e pattern' or '-e replacement'.");
            else
            {
                int index = 0;
                string pattern, replacement = null;
                if (args[index] == "-e")
                {
                    ++index;
                    pattern = args[index++];
                }
                else
                {
                    pattern = File.ReadAllText(args[index++]);
                }

                if (args.Count() > index)
                {
                    if (args[index] == "-e")
                    {
                        ++index;
                        replacement = args[index++];
                    }
                    else
                    {
                        replacement = File.ReadAllText(args[index++]);
                    }
                }

                string optionString = "";

                // Options can be specified in the regex file in front of the first backtick
                var parts = new List<string>(pattern.Split(new[] { '`' }));
                if (parts.Count > 1)
                {
                    optionString = parts[0];
                    parts.RemoveAt(0);
                }
                Options options = new Options(optionString, replacement != null);

                Regex regex = new Regex(String.Join("`", parts), options.RegexOptions);

                Stage stage = null;
                switch (options.Mode)
                {
                case Modes.Match:
                    stage = new MatchStage(options, regex);
                    break;
                case Modes.Split:
                    stage = new SplitStage(options, regex);
                    break;
                case Modes.Grep:
                case Modes.AntiGrep:
                    stage = new GrepStage(options, regex);
                    break;
                case Modes.Replace:
                    stage = new ReplaceStage(options, regex, replacement);
                    break;
                default:
                    throw new NotImplementedException();
                }

                Stages.Add(stage);
            }
	    }

        public void Execute()
        {
            if (Stages.Count > 0)
            {
                TextReader instrm = new StreamReader(Console.OpenStandardInput());
                string input = instrm.ReadToEnd();
                string result;
                foreach (var stage in Stages)
                    result = stage.Execute(input);
            }
        }
    }
}
