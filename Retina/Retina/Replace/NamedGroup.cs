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
        public bool GetLength { get; set; }

        public NamedGroup(string name, bool getCount, bool getLength)
        {
            Name = name;
            GetCount = getCount;
            GetLength = getLength;
        }

        public override string Process(string input, Match match)
        {
            if (GetCount)
                return match.Groups[Name].Captures.Count.ToString();
            else if (GetLength)
                return match.Groups[Name].Success ? match.Groups[Name].Length.ToString() : "";
            else
                return match.Groups[Name].Value;
        }
    }
}
