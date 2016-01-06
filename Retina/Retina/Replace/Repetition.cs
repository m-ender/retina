using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Retina.Replace
{
    public class Repetition : Token
    {
        public char Character { get; set; }
        public Token CountToken { get; set;}

        public Repetition(char character, Token token)
        {
            Character = character;
            CountToken = token;
        }

        public override string Process(string input, Match match)
        {
            string source = CountToken.Process(input, match);
            var countPattern = new Regex(@"\d+");
            var countMatch = countPattern.Match(source);
            int count;
            count = countMatch.Success ? int.Parse(countMatch.Value) : 0;
            return new String(Character, count);
        }
    }
}
