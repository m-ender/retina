using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Retina.Replace
{
    public class Suffix : Token
    {
        public Suffix() { }

        public override string Process(string input, Match match)
        {
            return input.Substring(match.Index + match.Length);
        }
    }
}
