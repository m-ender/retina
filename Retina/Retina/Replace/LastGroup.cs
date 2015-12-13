using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Retina.Replace
{
    public class LastGroup : Token
    {
        public LastGroup() { }

        public override string Process(string input, Match match)
        {
            return match.Groups[match.Groups.Count-1].Value;
        }
    }
}
