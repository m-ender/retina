using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Retina.Replace
{
    public class NumberedGroup : Token
    {
        public int Number { get; set; }

        public NumberedGroup(int number) {
            Number = number;
        }

        public override string Process(string input, Match match)
        {
            return match.Groups[Number].Value;
        }
    }
}
