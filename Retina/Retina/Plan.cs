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
            var groupOptions = new Stack<string>();
            groupOptions.Push("");

            if (args.Count() < 1)
                Console.WriteLine("Usage: ./Retina source.ret\n"+
                                  "You can also split the code over multiple files using the '-m' flag, in which case each\n" +
                                  "file corresponds to one line in a single source file (but may contain linefeeds.)\n" +
                                  "Usage: ./Retina -m pattern.rgx [replacement.rpl [pattern2.rgx replacement2.rpl [...]]]\n" +
                                  "Instead of a file name you can also use '-e pattern' or '-e replacement'.\n");
            else
            {
                List<string> sources = ReadSources(args);

                int i = 0;
                while (i < sources.Count)
                {
                    string pattern = sources[i++];

                    string optionString = "";

                    // Options can be specified in the regex file in front of the first backtick
                    var parts = new List<string>(pattern.Split(new[] { '`' }));
                    if (parts.Count > 1)
                    {
                        optionString = parts[0];
                        parts.RemoveAt(0);
                    }

                    string regex = String.Join("`", parts);

                    optionString = optionString.Replace("{", "+(").Replace("}", "+)");

                    if (optionString.Contains('(') && optionString.Contains(')'))
                        throw new Exception("Stage configuration cannot contain both '(' and ')'.");


                    if (optionString.Contains('('))
                    {
                        var openGroups = optionString.Split(new[] { '(' });
                        for (int j = 0; j < openGroups.Length - 1; ++j)
                        {
                            groupOptions.Push(openGroups[j]);
                            stageTree.Push(new List<Stage>());
                        }
                        optionString = openGroups.Last();
                    }

                    var groups = optionString.Split(new[] { ')' });

                    optionString = groups.Last();

                    Options options = new Options(optionString, i < sources.Count ? Modes.Replace : Modes.Match);

                    Stage stage = null;
                    string replacement;
                    switch (options.Mode)
                    {
                    case Modes.Match:
                        stage = new MatchStage(options, regex);
                        break;
                    case Modes.Replace:
                        replacement = i < sources.Count ? sources[i++] : "";
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
                    case Modes.Sort:
                        if (options.UseSubstitution)
                            replacement = i < sources.Count ? sources[i++] : "";
                        else
                            replacement = "$&";
                        stage = new SortStage(options, regex, replacement);
                        break;
                    default:
                        throw new NotImplementedException();
                    }

                    stageTree.Peek().Add(stage);

                    for (int j = 0; j < groups.Length - 1; ++j)
                    {
                        optionString = groupOptions.Pop() + groups[j];
                        if (groupOptions.Count == 0)
                            groupOptions.Push("");

                        options = new Options(optionString, Modes.Match);

                        InheritOptions(stageTree.Peek(), options);
                        stage = new GroupStage(options, stageTree.Pop());
                        if (stageTree.Count == 0)
                            stageTree.Push(new List<Stage>());
                        stageTree.Peek().Add(stage);
                    }
                }

                while (stageTree.Count > 1)
                {
                    var options = new Options(groupOptions.Pop(), Modes.Match);

                    InheritOptions(stageTree.Peek(), options);
                    var stage = new GroupStage(options, stageTree.Pop());
                    stageTree.Peek().Add(stage);
                }

                Stages = stageTree.Pop();

                if (Stages.Last().Options.Silent == null)
                    Stages.Last().Options.Silent = false;
            }
	    }

        private static void InheritOptions(IList<Stage> stages, Options options)
        {
            foreach (var stage in stages)
            {
                stage.Options.Inherit(options);
                if (stage is GroupStage)
                    InheritOptions(((GroupStage)stage).Stages, options);
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

            if (args[0] == "-m")
            {
                int i = 1;
                while (i < args.Length)
                {
                    if (args[i] == "-e")
                        result.Add(args[++i]);
                    else
                    {
                        string contents = File.ReadAllText(args[i]);
                        // Character code 65533 is used for characters that weren't valid UTF-8.
                        // If we find such a character, we re-read the file as ISO 8859-1.
                        if (contents.Contains((char)65533))
                            contents = File.ReadAllText(args[i], Encoding.GetEncoding("iso-8859-1"));

                        result.Add(contents);
                    }

                    ++i;
                }
            }
            else
            {
                int i = 0;
                bool pilcrows = true;
                if (args[0] == "-P")
                {
                    pilcrows = false;
                    ++i;
                }
                string contents = File.ReadAllText(args[i]);
                // Character code 65533 is used for characters that weren't valid UTF-8.
                // If we find such a character, we re-read the file as ISO 8859-1.
                if (contents.Contains((char)65533)) 
                    contents = File.ReadAllText(args[i], Encoding.GetEncoding("iso-8859-1"));

                if (pilcrows)
                    result.AddRange(contents.Split(new[] { '\n' }).Select(line => line.Replace('¶', '\n')));
                else
                    result.AddRange(contents.Split(new[] { '\n' }));
            }

            return result;
        }

        public void Execute()
        {
            if (Stages.Count > 0)
            {
                string input = "";
                if (Console.IsInputRedirected)
                {
                    TextReader instrm = new StreamReader(Console.OpenStandardInput());
                    input = instrm.ReadToEnd();
                }
                foreach (var stage in Stages)
                    input = stage.Execute(input);
            }
        }
    }
}