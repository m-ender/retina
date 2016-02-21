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
        public bool GetCount { get; set; }
        public bool GetLength { get; set; }

        public LastGroup(bool getCount, bool getLength)
        {
            GetCount = getCount;
            GetLength = getLength;
        }

        public override string Process(string input, Match match)
        {
            int num = match.Groups.Count-1;
            if (GetCount)
                return match.Groups[num].Captures.Count.ToString();
            else if (GetLength)
                return match.Groups[num].Success ? match.Groups[num].Length.ToString() : "";
            else
                return match.Groups[num].Value;
        }
    }
}
