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
        public bool GetLength { get; set; }

        public EntireMatch(bool getLength = false)
        {
            GetLength = getLength;
        }

        public override string Process(string input, Match match)
        {
            return GetLength ? match.Length.ToString() : match.Value;
        }
    }
}
