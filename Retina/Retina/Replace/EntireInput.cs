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

        public EntireInput(bool getLength)
        {
            GetLength = getLength;
        }

        public override string Process(string input, Match match)
        {
            return GetLength ? input.Length.ToString() : input;
        }
    }
}
