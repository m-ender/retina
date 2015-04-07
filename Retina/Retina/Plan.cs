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
                List<string> sources = ReadSources(args);

                if (sources.Count == 1)
                {
                    string pattern = sources[0];

                    Options options;
                    Regex regex;
                    ParsePattern(pattern, false, out options, out regex);

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
                    default:
                        throw new NotImplementedException();
                    }

                    Stages.Add(stage);
                }
                else if (sources.Count % 2 == 0)
                {
                    for (int i = 0; i < sources.Count; i += 2)
                    {
                        string pattern = sources[i];
                        string replacement = sources[i + 1];

                        Options options;
                        Regex regex;
                        ParsePattern(pattern, true, out options, out regex);

                        Stages.Add(new ReplaceStage(options, regex, replacement));
                    }
                }
                else
                    throw new ArgumentException("Retina must be called with a single argument or an even number of arguments.");
            }
	    }

        private static void ParsePattern(string pattern, bool replaceMode, out Options options, out Regex regex)
        {
            string optionString = "";

            // Options can be specified in the regex file in front of the first backtick
            var parts = new List<string>(pattern.Split(new[] { '`' }));
            if (parts.Count > 1)
            {
                optionString = parts[0];
                parts.RemoveAt(0);
            }
            options = new Options(optionString, replaceMode);

            regex = new Regex(String.Join("`", parts), options.RegexOptions);
        }

        private List<string> ReadSources(string[] args)
        {
            var result = new List<string>();

            if (args[0] == "-s")
            {
                result.AddRange(File.ReadAllText(args[1]).Split(new[] { '\n' }));
            }
            else
            {
                int i = 0;
                while (i < args.Length)
                {
                    if (args[i] == "-e")
                        result.Add(args[++i]);
                    else
                        result.Add(File.ReadAllText(args[i]));

                    ++i;
                }
            }

            return result;
        }

        public void Execute()
        {
            if (Stages.Count > 0)
            {
                TextReader instrm = new StreamReader(Console.OpenStandardInput());
                string input = instrm.ReadToEnd();
                string result = input;
                foreach (var stage in Stages)
                    result = stage.Execute(result);
            }
        }
    }
}
