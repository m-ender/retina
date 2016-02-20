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
        public LastGroup(bool getCount)
        {
            GetCount = getCount;
        }

        public override string Process(string input, Match match)
        {
            int num = match.Groups.Count-1;
            return GetCount ? match.Groups[num].Captures.Count.ToString() : match.Groups[num].Value;
        }
    }
}
