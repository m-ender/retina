using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Retina.Replace
{
    public class Suffix : Token
    {
        public bool GetLength { get; set; }
        public bool LineOnly { get; set; }

        public Suffix(bool getLength, bool lineOnly)
        {
            GetLength = getLength;
            LineOnly = lineOnly;
        }

        public override string Process(string input, Match match)
        {
            string result = input.Substring(match.Index + match.Length);

            if (LineOnly)
            {
                int length = result.IndexOf('\n');
                if (length >= 0) result = result.Substring(0, length);
            }

            return GetLength ? result.Length.ToString() : result;
        }
    }
}
