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
            var stageTree = new Stack<List<Stage>>();
            stageTree.Push(new List<Stage>());

            if (args.Count() < 1)
                Console.WriteLine("Usage: Retina.exe pattern.rgx [replacement.rpl [pattern2.rgx replacement2.rpl [...]]]\n" +
                                  "Instead of file name you can also use '-e pattern' or '-e replacement'.\n" +
                                  "The code can also be read from a single file with the '-s' flag, in which case patterns\n" +
                                  "  and replacements are separated by newlines.");
            else
            {
                List<string> sources = ReadSources(args);

                int i = 0;
                while (i < sources.Count)
                {
                    string pattern = sources[i++];

                    Options options;
                    string regex;
                    ParsePattern(pattern, i < sources.Count, false, out options, out regex);

                    for (int j = 0; j < options.OpenLoops; ++j)
                        stageTree.Push(new List<Stage>());

                    Stage stage = null;
                    switch (options.Mode)
                    {
                    case Modes.Match:
                        stage = new MatchStage(options, regex);
                        break;
                    case Modes.Replace:
                        string replacement = i < sources.Count ? sources[i++] : "";
                        stage = new ReplaceStage(options, regex, replacement);
                        break;
                    case Modes.Split:
                        stage = new SplitStage(options, regex);
                        break;
                    case Modes.Grep:
                    case Modes.AntiGrep:
                        stage = new GrepStage(options, regex);
                        break;
                    case Modes.Transliterate:
                        stage = new TransliterateStage(options, regex);
                        break;
                    default:
                        throw new NotImplementedException();
                    }

                    stageTree.Peek().Add(stage);

                    for (int j = 0; j < options.CloseLoops.Count; ++j)
                    {
                        var loopBody = stageTree.Pop();
                        if (stageTree.Count == 0)
                            stageTree.Push(new List<Stage>());
                        stageTree.Peek().Add(new LoopStage(loopBody, options.CloseLoops[j]));
                    }
                }

                while (stageTree.Count > 1)
                {
                    var loopBody = stageTree.Pop();
                    stageTree.Peek().Add(new LoopStage(loopBody));
                }
                Stages = stageTree.Pop();

                if (Stages.Last().Silent == null)
                    Stages.Last().Silent = false;
            }
	    }

        private static void ParsePattern(string pattern, bool replaceMode, bool last, out Options options, out string regex)
        {
            string optionString = "";

            // Options can be specified in the regex file in front of the first backtick
            var parts = new List<string>(pattern.Split(new[] { '`' }));
            if (parts.Count > 1)
            {
                optionString = parts[0];
                parts.RemoveAt(0);
            }
            options = new Options(optionString, replaceMode ? Modes.Replace : Modes.Match);

            regex = String.Join("`", parts);
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
