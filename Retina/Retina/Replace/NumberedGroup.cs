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
        public bool GetLength { get; set; }

        public NumberedGroup(int number, bool getCount, bool getLength)
        {
            Number = number;
            GetCount = getCount;
            GetLength = getLength;
        }

        public override string Process(string input, Match match)
        {
            if (GetCount)
                return match.Groups[Number].Captures.Count.ToString();
            else if (GetLength)
                return match.Groups[Number].Success ? match.Groups[Number].Length.ToString() : "";
            else
                return match.Groups[Number].Value;
        }
    }
}
