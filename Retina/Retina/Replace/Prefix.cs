using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Retina.Replace
{
    public class Prefix : Token
    {
        public bool GetLength { get; set; }

        public Prefix(bool getLength)
        {
            GetLength = getLength;
        }

        public override string Process(string input, Match match)
        {
            return GetLength ? match.Index.ToString() : input.Substring(0, match.Index);
        }
    }
}
