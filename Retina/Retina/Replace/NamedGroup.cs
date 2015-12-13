using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Retina.Replace
{
    public class NamedGroup : Token
    {
        public string Name { get; set; }

        public NamedGroup(string name) {
            Name = name;
        }

        public override string Process(string input, Match match)
        {
            return match.Groups[Name].Value;
        }
    }
}
