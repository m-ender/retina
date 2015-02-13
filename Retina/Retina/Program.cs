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
            switch (args.Count())
            {
            case 1:
                PerformMatch(args[0]);
                break;
            case 2:
                PerformReplacement(args[0], args[1]);
                break;
            default:
                Console.WriteLine("Usage: Retina.exe pattern.rgx [replacement.rgx]");
                break;
            }
        }

        private static void PerformMatch(string patternFile)
        {
            TextReader instrm = new StreamReader(Console.OpenStandardInput());
            string input = instrm.ReadToEnd();

            string pattern = File.ReadAllText(patternFile);
            char[] delimiter = {'\n'};
            var lines = new List<string>(pattern.Split(delimiter));
            if (lines.Count() > 1)
            {
                string options = lines[0];
                lines.RemoveAt(0);

                ParseOptions(options);
            }

            Regex regex = new Regex(String.Join("\n", lines));

            MatchCollection matches = regex.Matches(input);

            Console.WriteLine(matches.Count);
        }

        private static void PerformReplacement(string patternFile, string replacementFile)
        {
            throw new NotImplementedException();
        }

        private static void ParseOptions(string options)
        {
            throw new NotImplementedException();
        }
    }
}
