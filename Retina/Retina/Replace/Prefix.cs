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
        public bool LineOnly { get; set; }

        public Prefix(bool getLength, bool lineOnly)
        {
            GetLength = getLength;
            LineOnly = lineOnly;
        }

        public override string Process(string input, Match match)
        {
            string result = input.Substring(0, match.Index);

            if (LineOnly)
            {
                int start = result.LastIndexOf('\n') + 1;
                result = start >= result.Length ? "" : result.Substring(start);
            }

            return GetLength ? result.Length.ToString() : result;
        }
    }
}
