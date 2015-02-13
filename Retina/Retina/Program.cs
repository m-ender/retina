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
                Console.WriteLine("Usage: Retina.exe pattern.rgx [replacement.rgx]");
            else
            {

                string pattern = File.ReadAllText(args[0]);
                string replacement = null;
                if (args.Count() >= 2)
                    replacement = File.ReadAllText(args[1]);

                Process(pattern, replacement);
            }
        }

        private static void Process(string pattern, string replacement)
        {
            TextReader instrm = new StreamReader(Console.OpenStandardInput());
            string input = instrm.ReadToEnd();

            var lines = new List<string>(pattern.Split(new [] {'`'}));

            var options = new Options();
            if (lines.Count() > 1)
            {
                string optionString = lines[0];
                lines.RemoveAt(0);

                options = ParseOptions(optionString, replacement != null);
            }

            Regex regex = new Regex(String.Join("`", lines), options.RegexOptions);

            string line;
            switch (options.Mode)
            {
            case Modes.Match:
                MatchCollection matches = regex.Matches(input);
                Console.WriteLine(matches.Count);
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
                    Console.WriteLine(part);
                break;
            case Modes.Replace:
                Console.Write(regex.Replace(input,replacement));
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
