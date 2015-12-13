using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Retina.Replace
{
    public class EntireMatch : Token
    {
        public EntireMatch() { }

        public override string Process(string input, Match match)
        {
            return match.Value;
        }
    }
}
