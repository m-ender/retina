using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Retina
{
    class Program
    {
        static void Main(string[] args)
        {
            List<string> files = ParseArguments(args);
            if (files.Count > 0)
            {
                string pattern = files[0];
                string replacement = null;
                if (files.Count > 1)
                    replacement = files[1];

                Process(pattern, replacement);
            }
        }

        private static List<string> ParseArguments(string[] args)
        {
            var files = new List<string>();

            if (args.Count() < 1)
                Console.WriteLine("Usage: Retina.exe pattern.rgx [replacement.rpl]\n"+
                                  "Instead of a file names you can also use '-e pattern' or '-e replacement'.");
            else
            {
                int index = 0;

                if (args[index] == "-e")
                {
                    ++index;
                    files.Add(args[index++]);
                }
                else
                {
                    files.Add(File.ReadAllText(args[index++]));
                }

                if (args.Count() > index)
                {
                    if (args[index] == "-e")
                    {
                        ++index;
                        files.Add(args[index++]);
                    }
                    else
                    {
                        files.Add(File.ReadAllText(args[index++]));
                    }
                }
            }

            return files;
        }

        private static void Process(string pattern, string replacement)
        {
            TextReader instrm = new StreamReader(Console.OpenStandardInput());
            string input = instrm.ReadToEnd();

            string optionString = "";

            // Options can be specified in the regex file in front of the first backtick
            var parts = new List<string>(pattern.Split(new [] {'`'}));
            if (parts.Count > 1)
            {
                optionString = parts[0];
                parts.RemoveAt(0);
            }
            Options options = new Options(optionString, replacement != null);

            Regex regex = new Regex(String.Join("`", parts), options.RegexOptions);

            switch (options.Mode)
            {
            case Modes.Match:
                ProcessMatch(input, regex, options);
                break;

            case Modes.Grep:
            case Modes.AntiGrep:
                ProcessGrep(input, regex, options);
                break;

            case Modes.Split:
                ProcessSplit(input, regex, options);
                break;

            case Modes.Replace:
                ProcessReplace(input, regex, replacement, options);
                break;

            default:
                throw new NotImplementedException();
            }
        }

        private static void ProcessMatch(string input, Regex regex, Options options)
        {
            IList<Match> matches;

            if (!options.Overlapping)
            {
                matches = regex.Matches(input).Cast<Match>().ToList();
            }
            else
            {
                matches = new List<Match>();
                int start = 0;

                while (start < input.Length)
                {
                    Match match = regex.Match(input, start);
                    if (!match.Success) break;
                    matches.Add(match);
                    start = match.Index + 1;
                }
            }

            if (options.PrintMatches)
            {
                foreach (Match match in matches)
                    Console.WriteLine(match.Value);
            }
            else
            {
                Console.WriteLine(matches.Count);
            }
        }

        private static void ProcessGrep(string input, Regex regex, Options options)
        {
            string line;
            var stringReader = new StringReader(input);

            while ((line = stringReader.ReadLine()) != null)
            {
                if (regex.IsMatch(line) ^ (options.Mode == Modes.AntiGrep))
                    Console.WriteLine(line);
            }
        }

        private static void ProcessSplit(string input, Regex regex, Options options)
        {
            foreach (var part in regex.Split(input))
                if (!(options.OmitEmpty && part == ""))
                    Console.WriteLine(part);
        }

        private static void ProcessReplace(string input, Regex regex, string replacement, Options options)
        {
            if (!options.Loop)
                Console.Write(regex.Replace(input, replacement));
            else
            {
                string lastInput;
                do
                {
                    lastInput = input;
                    input = regex.Replace(input, replacement);
                    if (options.Trace && lastInput != input)
                        Console.WriteLine(input);
                } while (lastInput != input);

                if (!options.Trace)
                    Console.Write(input);
            }
        }
    }
}
