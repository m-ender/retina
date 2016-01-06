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
        public bool GetCount { get; set; }

        public NamedGroup(string name, bool getCount) {
            Name = name;
            GetCount = getCount;
        }

        public override string Process(string input, Match match)
        {
            return GetCount ? match.Groups[Name].Captures.Count.ToString() : match.Groups[Name].Value;
        }
    }
}
