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
            if (args.Count() < 1)
                Console.WriteLine("Usage: Retina.exe pattern.rgx [replacement.rgx]\n"+
                                  "Instead of a file names you can also use '-e pattern' or '-e replacement'.");
            else
            {
                int index = 0;
                string pattern;
                if (args[index] == "-e")
                {
                    ++index;
                    pattern = args[index++];
                }
                else
                {
                    pattern = File.ReadAllText(args[index++]);
                }

                string replacement = null;

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

                Process(pattern, replacement);
            }
        }

        private static void Process(string pattern, string replacement)
        {
            TextReader instrm = new StreamReader(Console.OpenStandardInput());
            string input = instrm.ReadToEnd();

            var lines = new List<string>(pattern.Split(new [] {'`'}));

            
            string optionString = "";
            if (lines.Count() > 1)
            {
                optionString = lines[0];
                lines.RemoveAt(0);
            }
            Options options = ParseOptions(optionString, replacement != null);

            Regex regex = new Regex(String.Join("`", lines), options.RegexOptions);

            string line;
            switch (options.Mode)
            {
            case Modes.Match:
                if (!options.Overlapping)
                {
                    MatchCollection matches = regex.Matches(input);
                    Console.WriteLine(matches.Count);
                }
                else
                {
                    var matches = new List<Match>();
                    int start = 0;
                    
                    while (start < input.Length)
                    {
                        Match match = regex.Match(input, start);
                        if (!match.Success) break;
                        matches.Add(match);
                        start = match.Index+1;
                    }

                    Console.WriteLine(matches.Count);
                }
                break;
            case Modes.Grep:
            case Modes.AntiGrep:
                var stringReader = new StringReader(input);
                
                while ((line = stringReader.ReadLine()) != null)
                {
                    if (regex.IsMatch(line) ^ (options.Mode == Modes.AntiGrep))
                        Console.WriteLine(line);
                }
                break;
            case Modes.Split:
                foreach (var part in regex.Split(input))
                    if (!(options.OmitEmpty && part == ""))
                        Console.WriteLine(part);
                break;
            case Modes.Replace:
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
                break;
            default:
                throw new NotImplementedException();
            }
        }

        private static Options ParseOptions(string optionString, bool replaceMode)
        {

            var options = new Options();

            foreach (char c in optionString)
            {
                switch (c)
                {
                // Parse RegexOptions
                case 'c':
                    options.RegexOptions |= RegexOptions.CultureInvariant;
                    break;
                case 'e':
                    options.RegexOptions |= RegexOptions.ECMAScript;
                    break;
                case 'i':
                    options.RegexOptions |= RegexOptions.IgnoreCase;
                    break;
                case 'm':
                    options.RegexOptions |= RegexOptions.Multiline;
                    break;
                case 'n':
                    options.RegexOptions |= RegexOptions.ExplicitCapture;
                    break;
                case 'r':
                    options.RegexOptions |= RegexOptions.RightToLeft;
                    break;                
                case 's':
                    options.RegexOptions |= RegexOptions.Singleline;
                    break;
                case 'x':
                    options.RegexOptions |= RegexOptions.IgnorePatternWhitespace;
                    break;

                // Parse Mode
                case 'G':
                    options.Mode = Modes.Grep;
                    break;
                case 'A':
                    options.Mode = Modes.AntiGrep;
                    break;
                case 'S':
                    options.Mode = Modes.Split;
                    break;

                // Mode-specific options
                case '&':
                    options.Overlapping = true;
                    break;
                case '_':
                    options.OmitEmpty = true;
                    break;
                case '+':
                    options.Loop = true;
                    break;
                case '?':
                    options.Trace = true;
                    break;
                default:
                    break;
                }
            }

            if (replaceMode)
                options.Mode = Modes.Replace;

            return options;
        }
    }
}
