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
        public bool GetCount { get; set; }

        public NumberedGroup(int number, bool getCount) {
            Number = number;
            GetCount = getCount;
        }

        public override string Process(string input, Match match)
        {
            return GetCount ? match.Groups[Number].Captures.Count.ToString() : match.Groups[Number].Value;
        }
    }
}
