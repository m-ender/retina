using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Retina.Replace
{
    public class EntireInput : Token
    {
        public bool GetLength { get; set; }
        public bool LineOnly { get; set; }

        public EntireInput(bool getLength, bool lineOnly)
        {
            GetLength = getLength;
            LineOnly = lineOnly;
        }

        public override string Process(string input, Match match)
        {
            string result = input;

            if (LineOnly)
            {
                int start = result.LastIndexOf('\n', match.Index) + 1;
                int end = result.IndexOf('\n', match.Index + match.Length) - 1;
                if (end == -2) end = result.Length - 1;
                if (start >= result.Length || end < 0 || end >= result.Length)
                    result = "";
                else
                    result = result.Substring(start, end - start + 1);
            }

            return GetLength ? result.Length.ToString() : result;
        }
    }
}
