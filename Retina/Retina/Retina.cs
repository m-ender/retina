using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Retina
{
    class Retina
    {
        static void Main(string[] args)
        {
            if (args.Count() < 1)
                Console.WriteLine("Usage: ./Retina source.ret");
            else
            {
                List<string> sources = ReadSources(args);

                var interpreter = new Interpreter(sources);

                string input = FetchInput();

                interpreter.Execute(input, Console.Out);
            }
        }

        private static string FetchInput()
        {
            string input = "";
            if (Console.IsInputRedirected)
            {
                TextReader instrm = new StreamReader(Console.OpenStandardInput());
                input = instrm.ReadToEnd();
            }

            return input;
        }

        private static List<string> ReadSources(string[] args)
        {
            var result = new List<string>();
            
            string contents = File.ReadAllText(args[0]);
            // Character code 65533 is used for characters that weren't valid UTF-8.
            // If we find such a character, we re-read the file as ISO 8859-1.
            if (contents.Contains((char)65533))
                contents = File.ReadAllText(args[0], Encoding.GetEncoding("iso-8859-1"));

            result.AddRange(contents.Split(new[] { '\n' }).Select(line => line.Replace('¶', '\n')));

            return result;
        }
    }
}
